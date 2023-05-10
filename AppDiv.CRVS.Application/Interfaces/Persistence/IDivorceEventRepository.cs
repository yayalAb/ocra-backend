using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IDivorceEventRepository : IBaseRepository<DivorceEvent>
    {
        public IQueryable<DivorceEvent> GetAllQueryableAsync();
        public void EFUpdate(DivorceEvent DivorceEvent);
        public bool exists(Guid id);
        public Task InsertOrUpdateAsync(DivorceEvent entity, CancellationToken cancellationToken);
    }
}