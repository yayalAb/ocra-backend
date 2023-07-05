using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IWorkerHistoryRepository : IBaseRepository<WorkerHistory>
    {
        public DatabaseFacade Database { get; }
        // Task<WorkerHistory> GetWithIncludedAsync(Guid id);
        // Task InsertOrUpdateAsync(WorkerHistory entity, CancellationToken cancellationToken);
        // public void UpdateAll(WorkerHistory entity);
    }
}