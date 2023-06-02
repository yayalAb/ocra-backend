using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class CorrectionRequestRepostory : BaseRepository<CorrectionRequest>, ICorrectionRequestRepostory
    {
        private readonly CRVSDbContext _DbContext;
        public CorrectionRequestRepostory(CRVSDbContext dbContext) : base(dbContext)
        {
            _DbContext = dbContext;
        }

    }
}