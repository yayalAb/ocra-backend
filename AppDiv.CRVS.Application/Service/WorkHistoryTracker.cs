using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;

namespace AppDiv.CRVS.Application.Service
{
    public class WorkHistoryTracker : IWorkHistoryTracker
    {
        private readonly IIdentityService _user;
        private readonly IGroupRepository _group;
        private readonly IWorkHistoryRepository _workerHistory;

        public WorkHistoryTracker(IIdentityService user, IGroupRepository group, IWorkHistoryRepository workerHistory)
        {
            this._group = group;
            this._workerHistory = workerHistory;
            this._user = user;
        }
        private bool CheckChangesAsync(ApplicationUser user, Guid newAddress, List<Guid> groups)
        {
            // var user = _user.GetSingle(userId);
            var newGroup = groups.OrderBy(g => g);
            var oldGroup = user?.UserGroups?.Select(g => g.Id)?.OrderBy(x => x);

            return (!user.AddressId.Equals(newAddress) || !oldGroup.SequenceEqual(newGroup));
        }

        public async Task TrackAsync(string userId, Guid addressId, List<Guid> groups, CancellationToken cancellationToken)
        {
            var user = await _user.GetSingleUserAsync(userId);
            if (CheckChangesAsync(user, addressId, groups))
            { 
                var oldHistory = _workerHistory.GetAll().OrderByDescending(h => h.CreatedAt).FirstOrDefault(h => h.UserId == userId);
                // user.UserGroups.Clear();
                var history = new WorkHistory
                {
                    UserId = userId,
                    AddressId = user.AddressId,
                    StartDate = oldHistory != null ? oldHistory.CreatedAt : user.CreatedAt,
                    UserGroups = await _group.GetMultipleUserGroups(groups)
                };
                await _workerHistory.InsertAsync(history, cancellationToken);
                await _workerHistory.SaveChangesAsync(cancellationToken);
            }
        }
    }
}