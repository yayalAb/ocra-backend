using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IDeathEventRepository : IBaseRepository<DeathEvent>
    {
        public DatabaseFacade Database { get; }

        Task<DeathEvent> GetIncludedAsync(Guid id);
        Task InsertOrUpdateAsync(DeathEvent entity, CancellationToken cancellationToken);
    }
}