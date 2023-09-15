using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Infrastructure.Services;
using AppDiv.CRVS.Utility.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class BirthEventRepository : BaseRepository<BirthEvent>, IBirthEventRepository
    {
        public DatabaseFacade Database => _dbContext.Database;
        private readonly CRVSDbContext _dbContext;

        public BirthEventRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public virtual async Task<BirthEvent?> GetWithIncludedAsync(Guid id)
        {
            return await _dbContext.BirthEvents
                            // .Include(b => b.FacilityLookup)
                            // .Include(b => b.FacilityTypeLookup)
                            .Include(b => b.BirthNotification)
                            .Include(b => b.Father)
                            .Include(b => b.Mother)
                            .Include(b => b.Event).ThenInclude(e => e.PaymentExamption).ThenInclude(p => p.SupportingDocuments)
                            .Include(b => b.Event).ThenInclude(e => e.EventSupportingDocuments)
                            .Include(b => b.Event).ThenInclude(e => e.EventOwener)
                            .Include(b => b.Event).ThenInclude(e => e.EventRegistrar).ThenInclude(r => r.RegistrarInfo)
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
                }

                return selectedperson;
            }
            return null;
        }

        public async Task InsertOrUpdateAsync(BirthEvent entity, CancellationToken cancellationToken)
        {
            try
            {
                entity.Event.EventType = "Birth";
                entity.Event.EventOwener.BirthDateEt = entity.Event.EventDateEt;
                entity.Event.EventOwener.BirthAddress = entity.Event.EventAddress;
                entity.Event.EventOwener.MiddleName = entity.Father?.FirstName;
                entity.Event.EventOwener.LastName = entity.Father?.MiddleName;
                // if (entity.Father != null)
                // {
                //     entity.Father.SexLookupId = _dbContext.Lookups.Where(l => l.Key == "sex")
                //                                     .Where(l => EF.Functions.Like(l.ValueStr, "%ወንድ%")
                //                                         || EF.Functions.Like(l.ValueStr, "%Dhiira%")
                //                                         || EF.Functions.Like(l.ValueStr, "%Male%"))
                //                                     .Select(l => l.Id).FirstOrDefault();
                // }
                // if (entity.Mother != null)
                // {
                //     entity.Mother.SexLookupId = _dbContext.Lookups.Where(l => l.Key == "sex")
                //                             .Where(l => EF.Functions.Like(l.ValueStr, "%ሴት%")
                //                                 || EF.Functions.Like(l.ValueStr, "%Dubara%")
                //                                 || EF.Functions.Like(l.ValueStr, "%Female%"))
                //                             .Select(l => l.Id).FirstOrDefault();
                // }

                if (!string.IsNullOrEmpty(entity.Event.EventOwener?.Id.ToString()) && entity.Event.EventOwener?.Id != Guid.Empty)
                {
                    PersonalInfo? selectedperson = _dbContext.PersonalInfos.FirstOrDefault(p => p.Id == entity.Event.EventOwener.Id);
                    selectedperson.FirstName = entity.Event?.EventOwener?.FirstName;
                    selectedperson.SexLookupId = entity.Event.EventOwener.SexLookupId;
                    selectedperson.BirthDate = entity.Event?.EventOwener?.BirthDate;
                    selectedperson.NationalityLookupId = entity.Event?.EventOwener?.NationalityLookupId;
                    selectedperson.BirthDate = entity.Event?.EventOwener?.BirthDate;
                    selectedperson.BirthAddressId = entity.Event?.EventOwener?.BirthAddressId;
                    selectedperson.ResidentAddressId = entity.Event?.EventOwener?.ResidentAddressId ?? selectedperson.ResidentAddressId;

                    _dbContext.PersonalInfos.Update(selectedperson);
                    entity.Event.EventOwenerId = entity.Event.EventOwener.Id;
                    entity.Event.EventOwener = null;
                }
                if (entity.Event.InformantType == "Police Officer" || entity.Event.InformantType == "Legal Guardian" ||
                  !string.IsNullOrEmpty(entity.Event.EventRegistrar?.RegistrarInfo.Id.ToString()) && entity.Event.EventRegistrar?.RegistrarInfo?.Id != Guid.Empty)
                {
                    PersonalInfo selectedperson = this.UpdatePerson(entity.Event.EventRegistrar?.RegistrarInfo);

                    _dbContext.PersonalInfos.Update(selectedperson);
                    entity.Event.EventRegistrar.RegistrarInfoId = entity.Event.EventRegistrar.RegistrarInfo.Id;
                    entity.Event.EventRegistrar.RegistrarInfo = null;
                }
                if (!string.IsNullOrEmpty(entity.Father?.Id.ToString()) && entity.Father?.Id != Guid.Empty && entity.Father != null)
                {
                    entity.Father.SexLookupId = _dbContext.Lookups.Where(l => l.Key == "sex")
                                                    .Where(l => EF.Functions.Like(l.ValueStr, "%ወንድ%")
                                                        || EF.Functions.Like(l.ValueStr, "%Dhiira%")
                                                        || EF.Functions.Like(l.ValueStr, "%Male%"))
                                                    .Select(l => l.Id).FirstOrDefault();
                    PersonalInfo selectedperson = this.UpdatePerson(entity.Father);
                    _dbContext.PersonalInfos.Update(selectedperson);
                    entity.FatherId = entity.Father.Id;
                    entity.Father = null;
                }
                if (!string.IsNullOrEmpty(entity.Mother?.Id.ToString()) && entity.Mother?.Id != Guid.Empty && entity.Mother != null)
                {
                    entity.Mother.SexLookupId = _dbContext.Lookups.Where(l => l.Key == "sex")
                                            .Where(l => EF.Functions.Like(l.ValueStr, "%ሴት%")
                                                || EF.Functions.Like(l.ValueStr, "%Dubara%")
                                                || EF.Functions.Like(l.ValueStr, "%Female%"))
                                            .Select(l => l.Id).FirstOrDefault();
                    PersonalInfo selectedperson = this.UpdatePerson(entity.Mother);
                    _dbContext.PersonalInfos.Update(selectedperson);
                    entity.MotherId = entity.Mother.Id;
                    entity.Mother = null;
                }


                await base.InsertAsync(entity, cancellationToken);
                await base.SaveChangesAsync(cancellationToken);
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public async Task UpdateAll(BirthEvent entity,IEventPaymentRequestService paymentRequestService, CancellationToken cancellationToken)
        {
            try
            {
                entity.Event.PaymentExamption = await HelperService.UpdatePaymentExamption(entity.Event, _dbContext,paymentRequestService,cancellationToken);
                entity.Event.EventOwener.MiddleName = entity.Father?.FirstName;
                entity.Event.EventOwener.LastName = entity.Father?.MiddleName;
                if(entity.Father!=null){

                entity.Father.SexLookupId = _dbContext.Lookups.Where(l => l.Key == "sex")
                                                .Where(l => EF.Functions.Like(l.ValueStr, "%ወንድ%")
                                                    || EF.Functions.Like(l.ValueStr, "%Dhiira%")
                                                    || EF.Functions.Like(l.ValueStr, "%Male%"))
                                                .Select(l => l.Id).FirstOrDefault();
                }
                if(entity.Mother!=null){
                    
                entity.Mother.SexLookupId = _dbContext.Lookups.Where(l => l.Key == "sex")
                                        .Where(l => EF.Functions.Like(l.ValueStr, "%ሴት%")
                                            || EF.Functions.Like(l.ValueStr, "%Dubara%")
                                            || EF.Functions.Like(l.ValueStr, "%Female%"))
                                        .Select(l => l.Id).FirstOrDefault();
                }
                // if (!string.IsNullOrEmpty(entity.Event.EventOwener?.Id.ToString()) && entity.Event.EventOwener?.Id != Guid.Empty)
                // {
                //     PersonalInfo? selectedperson = _dbContext.PersonalInfos.FirstOrDefault(p => p.Id == entity.Event.EventOwener.Id);
                //     selectedperson.NationalId = entity.Event?.EventOwener?.NationalId;
                //     selectedperson.NationalityLookupId = entity.Event?.EventOwener?.NationalityLookupId;
                //     selectedperson.ReligionLookupId = entity.Event?.EventOwener?.ReligionLookupId;
                //     selectedperson.EducationalStatusLookupId = entity.Event?.EventOwener?.EducationalStatusLookupId;
                //     selectedperson.TypeOfWorkLookupId = entity.Event?.EventOwener?.TypeOfWorkLookupId;
                //     selectedperson.MarriageStatusLookupId = entity.Event?.EventOwener?.MarriageStatusLookupId;
                //     selectedperson.NationLookupId = entity.Event?.EventOwener?.NationLookupId;

                //     entity.Event.EventOwener = selectedperson;
                // }
                // if (!string.IsNullOrEmpty(entity.Event.EventRegistrar?.RegistrarInfo.Id.ToString()) && entity.Event.EventRegistrar?.RegistrarInfo?.Id != Guid.Empty)
                // {
                //     PersonalInfo selectedperson = _dbContext.PersonalInfos.FirstOrDefault(p => p.Id == entity.Event.EventRegistrar.RegistrarInfo.Id);
                //     selectedperson.NationalId = entity.Event?.EventRegistrar.RegistrarInfo?.NationalId;
                //     selectedperson.NationalityLookupId = entity.Event?.EventRegistrar.RegistrarInfo?.NationalityLookupId;
                //     selectedperson.ReligionLookupId = entity.Event?.EventRegistrar.RegistrarInfo?.ReligionLookupId;
                //     selectedperson.EducationalStatusLookupId = entity.Event?.EventRegistrar.RegistrarInfo?.EducationalStatusLookupId;
                //     selectedperson.TypeOfWorkLookupId = entity.Event?.EventRegistrar.RegistrarInfo?.TypeOfWorkLookupId;
                //     selectedperson.MarriageStatusLookupId = entity.Event?.EventRegistrar.RegistrarInfo?.MarriageStatusLookupId;
                //     selectedperson.NationLookupId = entity.Event?.EventRegistrar.RegistrarInfo?.NationLookupId;

                //     entity.Event.EventRegistrar.RegistrarInfo = selectedperson;
                // }
                // if (!string.IsNullOrEmpty(entity.Father?.Id.ToString()) && entity.Father?.Id != Guid.Empty)
                // {
                //     PersonalInfo selectedperson = _dbContext.PersonalInfos.FirstOrDefault(p => p.Id == entity.Father.Id);
                //     selectedperson.NationalId = entity.Father?.NationalId;
                //     selectedperson.NationalityLookupId = entity.Father?.NationalityLookupId;
                //     selectedperson.ReligionLookupId = entity.Father?.ReligionLookupId;
                //     selectedperson.EducationalStatusLookupId = entity.Father?.EducationalStatusLookupId;
                //     selectedperson.TypeOfWorkLookupId = entity.Father?.TypeOfWorkLookupId;
                //     selectedperson.MarriageStatusLookupId = entity.Father?.MarriageStatusLookupId;
                //     selectedperson.NationLookupId = entity.Father?.NationLookupId;

                //     entity.Father = selectedperson;
                // }
                // if (!string.IsNullOrEmpty(entity.Mother?.Id.ToString()) && entity.Mother?.Id != Guid.Empty)
                // {
                //     PersonalInfo selectedperson = _dbContext.PersonalInfos.FirstOrDefault(p => p.Id == entity.Mother.Id);
                //     selectedperson.NationalId = entity.Mother?.NationalId;
                //     selectedperson.NationalityLookupId = entity.Mother?.NationalityLookupId;
                //     selectedperson.ReligionLookupId = entity.Mother?.ReligionLookupId;
                //     selectedperson.EducationalStatusLookupId = entity.Mother?.EducationalStatusLookupId;
                //     selectedperson.TypeOfWorkLookupId = entity.Mother?.TypeOfWorkLookupId;
                //     selectedperson.MarriageStatusLookupId = entity.Mother?.MarriageStatusLookupId;
                //     selectedperson.NationLookupId = entity.Mother?.NationLookupId;

                //     entity.Mother = selectedperson;
                // }
                base.UpdateWithNested(entity);
                await base.SaveChangesAsync(cancellationToken);

                // base.Update(entity);
            }
            catch (System.Exception)
            {

                throw;
            }


        }
    }
}