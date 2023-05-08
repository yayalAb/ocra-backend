using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class DeathEventRepository : BaseRepository<DeathEvent>, IDeathEventRepository
    {
        private readonly CRVSDbContext _dbContext;

        public DeathEventRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public virtual async Task<DeathEvent?> GetWithAsync(Guid id)
        {
            return await _dbContext.DeathEvent.Include(d => d.Event).ThenInclude(d => d.EventPaymentExamptionNavigation).Include(d => d.Event).ThenInclude(e => e.EventOwener)
                    .Include(d => d.Facility).Include(d => d.FacilityType).Include(d => d.DeathNotification).FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}