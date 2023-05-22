using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class CourtRepository : BaseRepository<Court>, ICourtRepository
    {
        private readonly CRVSDbContext dbContext;

        public CourtRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        async Task<Court> ICourtRepository.GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }
        public bool CourtCaseExists(Guid id)
        {
            return dbContext.CourtCases.Where(cc => cc.Id == id).Any();
        }
    }
}