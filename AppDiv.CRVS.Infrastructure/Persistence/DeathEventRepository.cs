using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class DeathEventRepository : BaseRepository<DeathEvent>, IDeathEventRepository
    {
        public DatabaseFacade Database => _dbContext.Database;
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
                            .Include(d => d.FacilityLookup)
                            .Include(d => d.FacilityTypeLookup)
                            .Include(d => d.DeathNotification)
                            .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task InsertOrUpdateAsync(DeathEvent entity, CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(entity.Event.EventOwener.Id.ToString()) && entity.Event.EventOwener?.Id != Guid.Empty)
                {

                    PersonalInfo selectedperson = _dbContext.PersonalInfos.FirstOrDefault(p => p.Id == entity.Event.EventOwener.Id);
                    selectedperson.NationalId = entity.Event?.EventOwener?.NationalId;
                    selectedperson.NationalityLookupId = entity.Event?.EventOwener?.NationalityLookupId;
                    selectedperson.ReligionLookupId = entity.Event?.EventOwener?.ReligionLookupId;
                    selectedperson.EducationalStatusLookupId = entity.Event?.EventOwener?.EducationalStatusLookupId;
                    selectedperson.TypeOfWorkLookupId = entity.Event?.EventOwener?.TypeOfWorkLookupId;
                    selectedperson.MarriageStatusLookupId = entity.Event?.EventOwener?.MarriageStatusLookupId;
                    selectedperson.NationLookupId = entity.Event?.EventOwener?.NationLookupId;
                    selectedperson.ResidentAddressId = entity.Event?.EventOwener?.ResidentAddressId;
                    selectedperson.TitleLookupId = entity.Event?.EventOwener?.TitleLookupId;

                    // _dbContext.PersonalInfos.Update(selectedperson);
                    _dbContext.PersonalInfos.Update(selectedperson);
                    entity.Event.EventOwenerId = entity.Event.EventOwener.Id;
                    entity.Event.EventOwener = null;
                }
                if (!string.IsNullOrEmpty(entity.Event.EventRegistrar?.RegistrarInfo.Id.ToString()) && entity.Event.EventRegistrar?.RegistrarInfo.Id != Guid.Empty)
                {
                    PersonalInfo selectedperson = _dbContext.PersonalInfos.FirstOrDefault(p => p.Id == entity.Event.EventRegistrar.RegistrarInfo.Id);
                    selectedperson.NationalId = entity.Event?.EventRegistrar?.RegistrarInfo?.NationalId;
                    selectedperson.NationalityLookupId = entity.Event?.EventRegistrar?.RegistrarInfo?.NationalityLookupId;
                    selectedperson.ReligionLookupId = entity.Event?.EventRegistrar?.RegistrarInfo?.ReligionLookupId;
                    selectedperson.EducationalStatusLookupId = entity.Event?.EventRegistrar?.RegistrarInfo?.EducationalStatusLookupId;
                    selectedperson.TypeOfWorkLookupId = entity.Event?.EventRegistrar?.RegistrarInfo?.TypeOfWorkLookupId;
                    selectedperson.MarriageStatusLookupId = entity.Event?.EventRegistrar?.RegistrarInfo?.MarriageStatusLookupId;
                    selectedperson.NationLookupId = entity.Event?.EventRegistrar?.RegistrarInfo?.NationLookupId;
                    selectedperson.TitleLookupId = entity.Event?.EventOwener?.TitleLookupId;

                    _dbContext.PersonalInfos.Update(selectedperson);
                    entity.Event.EventRegistrar.RegistrarInfoId = entity.Event.EventRegistrar.RegistrarInfo.Id;
                    entity.Event.EventRegistrar.RegistrarInfo = null;
                }
                // await _dbContext.DeathEvents.AddAsync(entity,cancellationToken);
                await base.InsertAsync(entity, cancellationToken);
            }
            catch (System.Exception)
            {
                throw;
            }


        }
    }
}