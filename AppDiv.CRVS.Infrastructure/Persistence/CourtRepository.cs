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
        public CourtRepository(CRVSDbContext dbContext) : base(dbContext)
        {
        }
        async Task<Court> ICourtRepository.GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }
    }
}