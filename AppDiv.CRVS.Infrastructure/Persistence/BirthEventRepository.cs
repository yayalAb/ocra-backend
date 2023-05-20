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
                            .Include(d => d.FacilityLookup)
                            .Include(d => d.FacilityTypeLookup)
                            .Include(d => d.BirthNotification)
                            .Include(d => d.Father)
                            .Include(d => d.Mother)
                            .Include(d => d.Event).ThenInclude(d => d.PaymentExamption)
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener)
                            .Include(d => d.Event).ThenInclude(e => e.EventRegistrar)
                            .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task InsertOrUpdateAsync(BirthEvent entity, CancellationToken cancellationToken)
        {
            try
            {
                entity.Event.EventOwener.MiddleName = entity.Father.FirstName;
                entity.Event.EventOwener.LastName = entity.Father.MiddleName;
                if (!string.IsNullOrEmpty(entity.Event.EventOwener?.Id.ToString()) && entity.Event.EventOwener?.Id != Guid.Empty)
                {
                    PersonalInfo? selectedperson = _dbContext.PersonalInfos.FirstOrDefault(p => p.Id == entity.Event.EventOwener.Id);
                    selectedperson.NationalId = entity.Event?.EventOwener?.NationalId;
                    selectedperson.NationalityLookupId = entity.Event?.EventOwener?.NationalityLookupId;
                    selectedperson.ReligionLookupId = entity.Event?.EventOwener?.ReligionLookupId;
                    selectedperson.EducationalStatusLookupId = entity.Event?.EventOwener?.EducationalStatusLookupId;
                    selectedperson.TypeOfWorkLookupId = entity.Event?.EventOwener?.TypeOfWorkLookupId;
                    selectedperson.MarriageStatusLookupId = entity.Event?.EventOwener?.MarriageStatusLookupId;
                    selectedperson.NationLookupId = entity.Event?.EventOwener?.NationLookupId;

                    _dbContext.PersonalInfos.Update(selectedperson);
                    entity.Event.EventOwenerId = entity.Event.EventOwener.Id;
                    entity.Event.EventOwener = null;
                }
                if (!string.IsNullOrEmpty(entity.Event.EventRegistrar?.RegistrarInfo.Id.ToString()) && entity.Event.EventRegistrar?.RegistrarInfo?.Id != Guid.Empty)
                {
                    PersonalInfo selectedperson = _dbContext.PersonalInfos.FirstOrDefault(p => p.Id == entity.Event.EventRegistrar.RegistrarInfo.Id);
                    selectedperson.NationalId = entity.Event?.EventRegistrar.RegistrarInfo?.NationalId;
                    selectedperson.NationalityLookupId = entity.Event?.EventRegistrar.RegistrarInfo?.NationalityLookupId;
                    selectedperson.ReligionLookupId = entity.Event?.EventRegistrar.RegistrarInfo?.ReligionLookupId;
                    selectedperson.EducationalStatusLookupId = entity.Event?.EventRegistrar.RegistrarInfo?.EducationalStatusLookupId;
                    selectedperson.TypeOfWorkLookupId = entity.Event?.EventRegistrar.RegistrarInfo?.TypeOfWorkLookupId;
                    selectedperson.MarriageStatusLookupId = entity.Event?.EventRegistrar.RegistrarInfo?.MarriageStatusLookupId;
                    selectedperson.NationLookupId = entity.Event?.EventRegistrar.RegistrarInfo?.NationLookupId;

                    _dbContext.PersonalInfos.Update(entity.Event.EventRegistrar.RegistrarInfo);
                    entity.Event.EventRegistrar.RegistrarInfoId = entity.Event.EventRegistrar.RegistrarInfo.Id;
                    entity.Event.EventRegistrar.RegistrarInfo = null;
                }
                if (!string.IsNullOrEmpty(entity.Father?.Id.ToString()) && entity.Father?.Id != Guid.Empty)
                {
                    PersonalInfo selectedperson = _dbContext.PersonalInfos.FirstOrDefault(p => p.Id == entity.Father.Id);
                    selectedperson.NationalId = entity.Father?.NationalId;
                    selectedperson.NationalityLookupId = entity.Father?.NationalityLookupId;
                    selectedperson.ReligionLookupId = entity.Father?.ReligionLookupId;
                    selectedperson.EducationalStatusLookupId = entity.Father?.EducationalStatusLookupId;
                    selectedperson.TypeOfWorkLookupId = entity.Father?.TypeOfWorkLookupId;
                    selectedperson.MarriageStatusLookupId = entity.Father?.MarriageStatusLookupId;
                    selectedperson.NationLookupId = entity.Father?.NationLookupId;

                    _dbContext.PersonalInfos.Update(entity.Father);
                    entity.FatherId = entity.Father.Id;
                    entity.Father = null;
                }
                if (!string.IsNullOrEmpty(entity.Mother?.Id.ToString()) && entity.Mother?.Id != Guid.Empty)
                {
                    PersonalInfo selectedperson = _dbContext.PersonalInfos.FirstOrDefault(p => p.Id == entity.Mother.Id);
                    selectedperson.NationalId = entity.Mother?.NationalId;
                    selectedperson.NationalityLookupId = entity.Mother?.NationalityLookupId;
                    selectedperson.ReligionLookupId = entity.Mother?.ReligionLookupId;
                    selectedperson.EducationalStatusLookupId = entity.Mother?.EducationalStatusLookupId;
                    selectedperson.TypeOfWorkLookupId = entity.Mother?.TypeOfWorkLookupId;
                    selectedperson.MarriageStatusLookupId = entity.Mother?.MarriageStatusLookupId;
                    selectedperson.NationLookupId = entity.Mother?.NationLookupId;

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