using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class WorkHistoryRepository : BaseRepository<WorkHistory>, IWorkHistoryRepository
    {
        public DatabaseFacade Database => _dbContext.Database;
        private readonly CRVSDbContext _dbContext;

        public WorkHistoryRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public IQueryable<WorkHistory> GetAll(string userId)
        {
            return _dbContext.WorkHistories
                            .AsNoTracking()
                            .Include(w => w.UserGroups)
                            .Include(w => w.User)
                            .Include(w => w.Address)
                            .AsQueryable();
        }
        public IQueryable<WorkHistory> GetAllGrid()
        {
            return _dbContext.WorkHistories
                            .AsNoTracking()
                            .Include(w => w.UserGroups)
                            .Include(w => w.User)
                            .Include(w => w.Address)
                                .ThenInclude(a => a.ParentAddress)
                                .ThenInclude(a => a.ParentAddress)
                            .AsQueryable();
        }
    }
}