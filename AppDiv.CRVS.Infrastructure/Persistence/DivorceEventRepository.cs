using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Features.DivorceEvents.Query;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
// using AppDiv.CRVS.Infrastructure.Service.FireAndForgetJobs;
using AppDiv.CRVS.Infrastructure.Services;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Nest;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class DivorceEventRepository : BaseRepository<DivorceEvent>, IDivorceEventRepository
    {
        private readonly CRVSDbContext dbContext;
        private readonly IMediator _mediator;
        private readonly IElasticClient elasticClient;
        // private readonly IFireAndForgetJobs fireAndForgetJobs;

        public DatabaseFacade Database => dbContext.Database;

        public DivorceEventRepository(CRVSDbContext dbContext, IMediator mediator, IElasticClient elasticClient /*IFireAndForgetJobs fireAndForgetJobs*/) : base(dbContext)
        {
            this.dbContext = dbContext;
            this._mediator = mediator;
            this.elasticClient = elasticClient;
            // this.fireAndForgetJobs = fireAndForgetJobs;
        }
        public IQueryable<DivorceEvent> GetAllQueryableAsync()
        {

            return dbContext.DivorceEvents.AsQueryable();
        }
        // 08db641e-a7a6-44cc-868e-4b3a02249d49--bride
        // 08db641e-a801-4cb6-8778-eaf20f9cdf41 groom
        public async Task EFUpdate(DivorceEvent DivorceEvent, IEventPaymentRequestService paymentRequestService, CancellationToken cancellationToken)
        {
            DivorceEvent.Event.PaymentExamption = await HelperService.UpdatePaymentExamption(DivorceEvent.Event, dbContext, paymentRequestService, cancellationToken);

            var existingOwner = dbContext.PersonalInfos.Find(DivorceEvent.Event.EventOwener.Id);
            if (existingOwner == null)
            {
                throw new NotFoundException($"divorce event with the provided id is not found");
            }
            Guid male = dbContext.Lookups.Where(l => l.Key == "sex")
                                                .Where(l => EF.Functions.Like(l.ValueStr, "%ወንድ%")
                                                    || EF.Functions.Like(l.ValueStr, "%Dhiira%")
                                                    || EF.Functions.Like(l.ValueStr, "%Male%"))
                                                .Select(l => l.Id).FirstOrDefault();

            var eventOwnerFeilds = new Dictionary<string, object>{
                    {"NationalId",DivorceEvent.Event.EventOwener.NationalId},
                    {"SexLookupId",male},
                    {"ReligionLookupId",DivorceEvent.Event.EventOwener.ReligionLookupId},
                    {"EducationalStatusLookupId",DivorceEvent.Event.EventOwener.EducationalStatusLookupId},
                    {"TypeOfWorkLookupId",DivorceEvent.Event.EventOwener.TypeOfWorkLookupId},
                    {"MarriageStatusLookupId",DivorceEvent.Event.EventOwener.MarriageStatusLookupId},
                    {"ResidentAddressId",DivorceEvent.Event.EventOwener.ResidentAddressId},
                    {"BirthAddressId", DivorceEvent.Event.EventOwener.BirthAddressId},
                    {"BirthDateEt", DivorceEvent.Event.EventOwener.BirthDateEt},
                    {"FirstName", DivorceEvent.Event.EventOwener.FirstName},
                    {"MiddleName", DivorceEvent.Event.EventOwener.MiddleName},
                    {"LastName", DivorceEvent.Event.EventOwener.LastName},
                    {"NationalityLookupId", DivorceEvent.Event.EventOwener.NationalityLookupId},
                    {"NationLookupId", DivorceEvent.Event.EventOwener.NationLookupId},
                    {"PhoneNumber", DivorceEvent.Event.EventOwener.PhoneNumber}
            };
            DivorceEvent.Event.EventOwener = HelperService.UpdateObjectFeilds<PersonalInfo>(existingOwner, eventOwnerFeilds);
            var existingWife = dbContext.PersonalInfos.Find(DivorceEvent.DivorcedWife.Id);
            if (existingWife == null)
            {
                throw new NotFoundException($"divorce event owner info with the provided id is not found");
            }
            Guid female = dbContext.Lookups.Where(l => l.Key == "sex")
                                        .Where(l => EF.Functions.Like(l.ValueStr, "%ሴት%")
                                            || EF.Functions.Like(l.ValueStr, "%Dubara%")
                                            || EF.Functions.Like(l.ValueStr, "%Female%"))
                                        .Select(l => l.Id).FirstOrDefault();
            var divorcedWifeFeilds = new Dictionary<string, object>{
                    {"NationalId",DivorceEvent.DivorcedWife.NationalId},
                    {"SexLookupId",female},
                    {"ReligionLookupId",DivorceEvent.DivorcedWife.ReligionLookupId},
                    {"EducationalStatusLookupId",DivorceEvent.DivorcedWife.EducationalStatusLookupId},
                    {"TypeOfWorkLookupId",DivorceEvent.DivorcedWife.TypeOfWorkLookupId},
                    {"MarriageStatusLookupId",DivorceEvent.DivorcedWife.MarriageStatusLookupId},
                    {"ResidentAddressId",DivorceEvent.DivorcedWife.ResidentAddressId},
                    {"BirthAddressId", DivorceEvent.DivorcedWife.BirthAddressId},
                    {"BirthDateEt", DivorceEvent.DivorcedWife.BirthDateEt},
                    {"FirstName", DivorceEvent.DivorcedWife.FirstName},
                    {"MiddleName", DivorceEvent.DivorcedWife.MiddleName},
                    {"LastName", DivorceEvent.DivorcedWife.LastName},
                    {"NationalityLookupId", DivorceEvent.DivorcedWife.NationalityLookupId},
                    {"NationLookupId", DivorceEvent.DivorcedWife.NationLookupId},
                    {"PhoneNumber", DivorceEvent.DivorcedWife.PhoneNumber}
            };
            DivorceEvent.DivorcedWife = HelperService.UpdateObjectFeilds<PersonalInfo>(existingWife, divorcedWifeFeilds);
            base.Update(DivorceEvent);
            await base.SaveChangesAsync(cancellationToken);
        }
        public bool exists(Guid id)
        {
            return dbContext.DivorceEvents.Where(m => m.Id == id).Any();
        }
        public async Task InsertOrUpdateAsync(DivorceEvent entity, CancellationToken cancellationToken)
        {
            entity.Event.EventOwener.SexLookupId = dbContext.Lookups.Where(l => l.Key == "sex")
                                                        .Where(l => EF.Functions.Like(l.ValueStr, "%ወንድ%")
                                                            || EF.Functions.Like(l.ValueStr, "%Dhiira%")
                                                            || EF.Functions.Like(l.ValueStr, "%Male%"))
                                                        .Select(l => l.Id).FirstOrDefault();
            entity.DivorcedWife.SexLookupId = dbContext.Lookups.Where(l => l.Key == "sex")
                                                    .Where(l => EF.Functions.Like(l.ValueStr, "%ሴት%")
                                                        || EF.Functions.Like(l.ValueStr, "%Dubara%")
                                                        || EF.Functions.Like(l.ValueStr, "%Female%"))
                                                    .Select(l => l.Id).FirstOrDefault();
            if (entity.DivorcedWife.Id != null && entity.DivorcedWife.Id != Guid.Empty && entity.Event.EventOwener.Id != null && entity.Event.EventOwener.Id != Guid.Empty)
            {
                var divorcedMarraige = await dbContext.MarriageEvents
                     .Where(m => m.BrideInfoId == entity.DivorcedWife.Id && m.Event.EventOwenerId == entity.Event.EventOwenerId)
                     .FirstOrDefaultAsync();
                if (divorcedMarraige != null)
                {
                    divorcedMarraige.IsDivorced = true;
                }
            }

            if (entity.Event.EventOwener.Id != null && entity.Event.EventOwener.Id != Guid.Empty)
            {
                var keyValuePair = new Dictionary<string, object>{
                    {"NationalId",entity.Event.EventOwener.NationalId},
                    {"SexLookupId",entity.Event.EventOwener.SexLookupId},
                    {"ReligionLookupId",entity.Event.EventOwener.ReligionLookupId},
                    {"EducationalStatusLookupId",entity.Event.EventOwener.EducationalStatusLookupId},
                    {"TypeOfWorkLookupId",entity.Event.EventOwener.TypeOfWorkLookupId},
                    {"MarriageStatusLookupId",entity.Event.EventOwener.MarriageStatusLookupId},
                    {"ResidentAddressId",entity.Event.EventOwener.ResidentAddressId}
            };
                await updatePersonalInfo(keyValuePair, entity.Event.EventOwener.Id, "eventOwner");
                entity.Event.EventOwenerId = entity.Event.EventOwener.Id;
                entity.Event.EventOwener = null;
            }

            if (entity.DivorcedWife.Id != null && entity.DivorcedWife.Id != Guid.Empty)
            {
                var keyValuePair = new Dictionary<string, object>{
                    {"NationalId",entity.DivorcedWife.NationalId},
                    {"SexLookupId",entity.DivorcedWife.SexLookupId},
                    {"ReligionLookupId",entity.DivorcedWife.ReligionLookupId},
                    {"EducationalStatusLookupId",entity.DivorcedWife.EducationalStatusLookupId},
                    {"TypeOfWorkLookupId",entity.DivorcedWife.TypeOfWorkLookupId},
                    {"MarriageStatusLookupId",entity.DivorcedWife.MarriageStatusLookupId},
                    {"ResidentAddressId",entity.DivorcedWife.ResidentAddressId},
            };
                await updatePersonalInfo(keyValuePair, entity.DivorcedWife.Id, "DivorcedWife");
                entity.DivorcedWifeId = entity.DivorcedWife.Id;
                entity.DivorcedWife = null;
            }

            await base.InsertAsync(entity, cancellationToken);
            await base.SaveChangesAsync(cancellationToken);





        }
        // public virtual async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
        // {
        //     var personEntries = dbContext.ChangeTracker
        //        .Entries()
        //        .Where(e => e.Entity is PersonalInfo && (
        //                e.State == EntityState.Added
        //                || e.State == EntityState.Modified
        //                || e.State == EntityState.Deleted)).ToList();
        //     List<PersonalInfoEntry> personalInfoEntries = personEntries.Select(e => new PersonalInfoEntry
        //     {
        //         State = e.State,
        //         PersonalInfoId = ((PersonalInfo)e.Entity).Id
        //     }).ToList();
        //     await base.SaveChangesAsync(cancellationToken);
        //     if (personalInfoEntries.Any())
        //     {

        //         List<object> addedPersons = new List<object>();
        //         List<PersonalInfoIndex> addedPersonIndexes = new List<PersonalInfoIndex>();
        //         List<object> updatedPersons = new List<object>();
        //         List<Guid> deletedPersonIds = new List<Guid>();
        //         personalInfoEntries.ForEach(e =>
        //         {
        //             var p =
        //            dbContext.PersonalInfos
        //                 .Where(p => p.Id == e.PersonalInfoId)
        //                 .Include(p => p.ResidentAddress)
        //                 .Include(p => p.SexLookup)
        //                 .Include(p => p.TypeOfWorkLookup)
        //                 .Include(p => p.TitleLookup)
        //                 .Include(p => p.MarraigeStatusLookup)
        //                 .Include(p => p.Events.Where(e => e.EventType == "Marriage"))
        //                     .ThenInclude(e => e.MarriageEvent)
        //                         .ThenInclude(m => m.MarriageType)
        //                 ;
        //             e.PersonalInfo = p.FirstOrDefault();
        //             if (e.State == EntityState.Added && e.PersonalInfo != null)
        //             {
        //                 addedPersons.Add(e.PersonalInfo);
        //                 if (p != null)
        //                 {

        //                     addedPersonIndexes.Add(p.Select(personalInfo => new PersonalInfoIndex
        //                     {
        //                         Id = personalInfo.Id,
        //                         FirstNameStr = personalInfo.FirstNameStr,
        //                         FirstNameOr = personalInfo.FirstName == null ? null : personalInfo.FirstName.Value<string>("or"),
        //                         FirstNameAm = personalInfo.FirstName == null ? null : personalInfo.FirstName.Value<string>("am"),
        //                         MiddleNameStr = personalInfo.MiddleNameStr,
        //                         MiddleNameOr = personalInfo.MiddleName == null ? null : personalInfo.MiddleName.Value<string>("or"),
        //                         MiddleNameAm = personalInfo.MiddleName == null ? null : personalInfo.MiddleName.Value<string>("am"),
        //                         LastNameStr = personalInfo.LastNameStr,
        //                         LastNameOr = personalInfo.LastName == null ? null : personalInfo.LastName.Value<string>("or"),
        //                         LastNameAm = personalInfo.LastName == null ? null : personalInfo.LastName.Value<string>("am"),
        //                         NationalId = personalInfo.NationalId,
        //                         PhoneNumber = personalInfo.PhoneNumber,
        //                         BirthDate = personalInfo.BirthDate,
        //                         GenderOr = personalInfo.SexLookup.Value == null ? null : personalInfo.SexLookup.Value.Value<string>("or"),
        //                         GenderAm = personalInfo.SexLookup.Value == null ? null : personalInfo.SexLookup.Value.Value<string>("am"),
        //                         GenderStr = personalInfo.SexLookup.ValueStr,
        //                         TypeOfWorkStr = personalInfo.TypeOfWorkLookup.ValueStr,
        //                         TitleStr = personalInfo.TitleLookup.ValueStr,
        //                         MarriageStatusStr = personalInfo.MarraigeStatusLookup.ValueStr,
        //                         AddressOr = personalInfo.ResidentAddress.AddressName == null ? null : personalInfo.ResidentAddress.AddressName.Value<string>("or"),
        //                         AddressAm = personalInfo.ResidentAddress.AddressName == null ? null : personalInfo.ResidentAddress.AddressName.Value<string>("am"),
        //                         DeathStatus = personalInfo.DeathStatus
        //                     }).FirstOrDefault()!);
        //                 }
        //             }
        //             else if (e.State == EntityState.Modified && e.PersonalInfo != null)
        //             {
        //                 updatedPersons.Add(e.PersonalInfo);
        //             }
        //             else if (e.State == EntityState.Deleted && e.PersonalInfo != null)
        //             {
        //                 deletedPersonIds.Add(e.PersonalInfo.Id);
        //             }
        //         });
        //         // BackgroundJob.Schedule<IBackgroundJobs>(x => x.AddPersonIndex(addedPersonIndexes, "personal_info"),TimeSpan.Zero);

        //         // BackgroundJob.Enqueue<IFireAndForgetJobs>(x => x.AddPersonIndex(addedPersonIndexes, "personal_info"));
        //         // var fg = new FireAndForgetJobs();
        //         var bgClient = new BackgroundJobClient();
        //         // var fireAndForgetJobs = new FireAndForgetJobs();
        //         bgClient.Schedule<IFireAndForgetJobs>(x => x.test(),TimeSpan.Zero);
        //         // bgClient.Enqueue<IFireAndForgetJobs>(x => x.test());





        //         // BackgroundJob.Enqueue<IBackgroundJobs>(x => x.RemoveIndex<PersonalInfoIndex>(deletedPersonIds, "personal_info"));

        //         // await fireAndForgetJobs.AddPersonIndex(addedPersonIndexes, "personal_info");
        //     }

        //     return true;
        // }

        private async Task updatePersonalInfo(Dictionary<string, object> keyValuePair, Guid id, string feildName)
        {
            var existing = await dbContext.PersonalInfos.FindAsync(id);
            if (existing == null)
            {
                throw new NotFoundException($"{feildName} with the provided id is not found");
            }
            if (feildName == "eventOwner")
            {
                var wifes = await _mediator.Send(new GetWivesQuery { HusbandId = id });
                if (wifes.Count <= 1)
                {
                    existing.MarriageStatusLookupId = dbContext.Lookups.Where(l => l.Key == "marriage-status")
                                                        .Where(l => EF.Functions.Like(l.ValueStr, "%የተፋታ%")
                                                            || EF.Functions.Like(l.ValueStr, "%kan hiike%"))
                                                        .Select(l => l.Id).FirstOrDefault();
                }
            }
            else if (feildName == "DivorcedWife")
            {
                existing.MarriageStatusLookupId = dbContext.Lookups.Where(l => l.Key == "marriage-status")
                                                    .Where(l => EF.Functions.Like(l.ValueStr, "%የተፋታች%")
                                                        || EF.Functions.Like(l.ValueStr, "%kan hiikte%"))
                                                    .Select(l => l.Id).FirstOrDefault();
            }

            existing = HelperService.UpdateObjectFeilds<PersonalInfo>(existing, keyValuePair);
            dbContext.PersonalInfos.Update(existing);

        }

    }
}
