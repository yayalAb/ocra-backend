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
                            .Include(d => d.Event).ThenInclude(e => e.EventOwener)
                            .Include(d => d.Event).ThenInclude(e => e.EventRegistrar).ThenInclude(r => r.RegistrarInfo)
                            .Include(m => m.Event.EventSupportingDocuments.Where(s => s.Type.ToLower() != "webcam"))
                            .Include(m => m.Event.PaymentExamption).ThenInclude(p => p.SupportingDocuments)
                            .Include(d => d.DeathNotification)
                            .FirstOrDefaultAsync(d => d.Id == id);
        }

        private PersonalInfo? UpdatePerson(PersonalInfo oldPerson)
        {
            if (!string.IsNullOrEmpty(oldPerson.Id.ToString()) && oldPerson.Id != Guid.Empty)
            {
                PersonalInfo selectedperson = _dbContext.PersonalInfos.FirstOrDefault(p => p.Id == oldPerson.Id);
                if (selectedperson != null)
                {
                    selectedperson.NationalId = oldPerson?.NationalId;
                    selectedperson.NationalityLookupId = oldPerson?.NationalityLookupId;
                    selectedperson.ReligionLookupId = oldPerson?.ReligionLookupId;
                    selectedperson.EducationalStatusLookupId = oldPerson?.EducationalStatusLookupId;
                    selectedperson.TypeOfWorkLookupId = oldPerson?.TypeOfWorkLookupId;
                    selectedperson.MarriageStatusLookupId = oldPerson?.MarriageStatusLookupId;
                    selectedperson.NationLookupId = oldPerson?.NationLookupId;
                    selectedperson.ResidentAddressId = oldPerson?.ResidentAddressId;
                }

                return selectedperson;
            }
            return null;
        }

        public async Task InsertOrUpdateAsync(DeathEvent entity, CancellationToken cancellationToken)
        {
            try
            {
                entity.Event.EventType = "Death";
                if (!string.IsNullOrEmpty(entity.Event.EventOwener.Id.ToString()) && entity.Event.EventOwener?.Id != Guid.Empty)
                {
                    PersonalInfo selectedperson = this.UpdatePerson(entity.Event.EventOwener);
                    selectedperson.TitleLookupId = entity.Event?.EventOwener?.TitleLookupId;

                    // _dbContext.PersonalInfos.Update(selectedperson);
                    _dbContext.PersonalInfos.Update(selectedperson);
                    entity.Event.EventOwenerId = entity.Event.EventOwener.Id;
                    entity.Event.EventOwener = null;
                }
                if (!string.IsNullOrEmpty(entity.Event.EventRegistrar?.RegistrarInfo.Id.ToString()) && entity.Event.EventRegistrar?.RegistrarInfo.Id != Guid.Empty)
                {
                    PersonalInfo selectedperson = this.UpdatePerson(entity.Event.EventRegistrar?.RegistrarInfo);
                    // selectedperson.TitleLookupId = entity.Event?.EventOwener?.TitleLookupId;

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