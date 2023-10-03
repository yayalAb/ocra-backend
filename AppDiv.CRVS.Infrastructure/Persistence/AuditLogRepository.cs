using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities.Audit;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class AuditLogRepository : BaseRepository<AuditLog>, IAuditLogRepository
    {
        private readonly CRVSDbContext _dbContext;

        public AuditLogRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }
        public IQueryable<AuditLog> GetAllGrid()
        {
            return _dbContext.AuditLogs
                        .AsNoTracking()
                        .Include(a => a.AuditUser.PersonalInfo)
                        .Include(a => a.Address)
                        .ThenInclude(a => a.ParentAddress)
                        .ThenInclude(a => a.ParentAddress);
        }

    }
}