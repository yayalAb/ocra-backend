
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IProfileChangeRequestRepository : IBaseRepository<ProfileChangeRequest>
    {
        public DatabaseFacade Database { get; }

    }
}

