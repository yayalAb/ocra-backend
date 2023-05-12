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

        public virtual async Task<BirthEvent?> GetWithIncludedAsync(Guid id)
        {
            return await _dbContext.BirthEvents
                            .Include(d => d.Event).ThenInclude(d => d.PaymentExamption)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener)
                            .Include(d => d.FacilityLookup)
                            .Include(d => d.FacilityTypeLookup)
                            .Include(d => d.BirthNotification)
                            .Include(d => d.Father)
                            .Include(d => d.Mother)
                            .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task InsertOrUpdateAsync(BirthEvent entity, CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(entity.Event.EventOwener.Id.ToString()))
                {
                    _dbContext.PersonalInfos.Update(entity.Event.EventOwener);
                    entity.Event.EventOwenerId = entity.Event.EventOwener.Id;
                    entity.Event.EventOwener = null;
                }
                if (!string.IsNullOrEmpty(entity.Event.EventRegistrar?.RegistrarInfo.Id.ToString()))
                {
                    _dbContext.PersonalInfos.Update(entity.Event.EventRegistrar.RegistrarInfo);
                    entity.Event.EventRegistrar.RegistrarInfoId = entity.Event.EventRegistrar.RegistrarInfo.Id;
                    entity.Event.EventRegistrar.RegistrarInfo = null;
                }
                if (!string.IsNullOrEmpty(entity.Father.Id.ToString()))
                {
                    _dbContext.PersonalInfos.Update(entity.Father);
                    entity.FatherId = entity.Father.Id;
                    entity.Father = null;
                }
                if (!string.IsNullOrEmpty(entity.Mother.Id.ToString()))
                {
                    _dbContext.PersonalInfos.Update(entity.Mother);
                    entity.MotherId = entity.Mother.Id;
                    entity.Mother = null;
                }


                await base.InsertAsync(entity, cancellationToken);
            }
            catch (System.Exception)
            {

                throw;
            }


        }
    }
}