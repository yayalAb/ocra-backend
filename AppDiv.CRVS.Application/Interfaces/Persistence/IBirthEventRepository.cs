using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IBirthEventRepository : IBaseRepository<BirthEvent>
    {
        Task<BirthEvent> GetWithIncludedAsync(Guid id);
        Task InsertOrUpdateAsync(BirthEvent entity, CancellationToken cancellationToken);
        public void UpdateAll(BirthEvent entity);
    }
}