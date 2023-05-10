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

        public async Task<DeathEvent?> GetIncludedAsync(Guid id)
        {
            return await _dbContext.DeathEvents
                            .Include(d => d.Event).ThenInclude(d => d.PaymentExamption)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener)
                            .Include(d => d.Event).ThenInclude(e => e.EventRegistrar).ThenInclude(r => r.RegistrarInfo)
                            .Include(d => d.Facility)
                            .Include(d => d.FacilityType)
                            .Include(d => d.DeathNotification)
                            .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task InsertOrUpdateAsync(DeathEvent entity, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(entity.Event.EventOwener.Id.ToString()))
            {
                _dbContext.PersonalInfos.Update(entity.Event.EventOwener);
                entity.Event.EventOwener = null;
            }
            if (!string.IsNullOrEmpty(entity.Event.EventRegistrar.RegistrarInfo.Id.ToString()))
            {
                _dbContext.PersonalInfos.Update(entity.Event.EventRegistrar.RegistrarInfo);
                entity.Event.EventRegistrar.RegistrarInfo = null;
            }

            await base.InsertAsync(entity, cancellationToken);


        }
    }
}