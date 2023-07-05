using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IWorkHistoryTracker
    {
        Task TrackAsync(string userId, Guid addressId, List<Guid> groups, CancellationToken cancellationToken);
    }
}