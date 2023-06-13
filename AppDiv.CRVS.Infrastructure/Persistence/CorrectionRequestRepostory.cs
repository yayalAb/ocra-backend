using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class CorrectionRequestRepostory : BaseRepository<CorrectionRequest>, ICorrectionRequestRepostory
    {
        private readonly CRVSDbContext _DbContext;
        public DatabaseFacade Database => _DbContext.Database;
        public CorrectionRequestRepostory(CRVSDbContext dbContext) : base(dbContext)
        {
            _DbContext = dbContext;
            // Database = dbContext.Database;
        }
        async Task<CorrectionRequest> ICorrectionRequestRepostory.GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }

    }
}