using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class ReportStoreRepostory : BaseRepository<ReportStore>, IReportStoreRepostory
    {
        private readonly CRVSDbContext _DbContext;
        public ReportStoreRepostory(CRVSDbContext dbContext) : base(dbContext)
        {
            _DbContext = dbContext;
        }

    }
}

