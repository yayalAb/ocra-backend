using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IDivorceEventRepository : IBaseRepository<DivorceEvent>
    {
        public IQueryable<DivorceEvent> GetAllQueryableAsync();
        public DatabaseFacade Database { get; }
        public Task EFUpdate(DivorceEvent DivorceEvent, CancellationToken cancellationToken);
        public bool exists(Guid id);
        public Task InsertOrUpdateAsync(DivorceEvent entity, CancellationToken cancellationToken);
    }
}