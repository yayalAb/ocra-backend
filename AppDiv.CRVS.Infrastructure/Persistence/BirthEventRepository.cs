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
    public class BirthEventRepository : BaseRepository<BirthEvent>, IBirthEventRepository
    {
        private readonly CRVSDbContext _dbContext;

        public BirthEventRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public virtual async Task<BirthEvent?> GetWithAsync(Guid id)
        {
            return await _dbContext.birthEvents
                            .Include(d => d.Event).ThenInclude(d => d.PaymentExamption)
                            // .Include(d => d.Event).ThenInclude(e => e.EventOwener)
                            .Include(d => d.Facility)
                            .Include(d => d.FacilityType)
                            .Include(d => d.BirthNotification)
                            .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task InsertOrUpdateAsync(BirthEvent entity, CancellationToken cancellationToken)
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
            if (!string.IsNullOrEmpty(entity.Father.Id.ToString()))
            {
                _dbContext.PersonalInfos.Update(entity.Father);
                entity.Father = null;
            }
            if (!string.IsNullOrEmpty(entity.Mother.Id.ToString()))
            {
                _dbContext.PersonalInfos.Update(entity.Mother);
                entity.Mother = null;
            }


            await base.InsertAsync(entity, cancellationToken);


        }
    }
}