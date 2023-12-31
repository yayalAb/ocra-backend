using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Groups.Query.GetAllGroup

{
    // Customer query with List<Customer> response
    public record GetDropDownGroups : IRequest<List<DropDownDto>>
    {

    }

    public class GetDropDownGroupsHandler : IRequestHandler<GetDropDownGroups, List<DropDownDto>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserResolverService _userResolver;
        private readonly IIdentityService _user;

        public GetDropDownGroupsHandler(IGroupRepository groupRepository, IUserResolverService userResolver, IIdentityService user)
        {
            this._userResolver = userResolver;
            this._user = user;
            _groupRepository = groupRepository;
        }
        public async Task<List<DropDownDto>> Handle(GetDropDownGroups request, CancellationToken cancellationToken)
        {
            // Get User from token
            var user = _userResolver.GetUserId() != null ? await _user.GetSingleUserAsync(_userResolver.GetUserId()!) : null;
            var managedGroups = new List<Guid>(); // managed groups by the user group
            var groupManager = false;   // is he a group manager
            if (user != null)
            {
                foreach (var group in user.UserGroups)
                {
                    if (group.ManageAll)
                    {
                        groupManager = true; // if one of his group is manage all groups set group manager to true
                        break;
                    }
                    var groupsId = group.ManagedGroups.Select(gId => (Guid)gId).ToList();
                    managedGroups.AddRange(groupsId);
                    managedGroups.Add(group.Id);
                }
                
            }
            var groups = groupManager ? _groupRepository.GetAll().OrderByDescending(g => g.CreatedAt).ToList() 
                                      : await _groupRepository.GetMultipleUserGroups(managedGroups);
            return groups.Select(g => new DropDownDto{
                Key= g.Id,
                Value = g.GroupName
            }).ToList();

        }
    }
}