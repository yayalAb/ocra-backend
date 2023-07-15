using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IWorkHistoryRepository : IBaseRepository<WorkHistory>
    {
        public DatabaseFacade Database { get; }
        IQueryable<WorkHistory> GetAll(string userId);
    }
}