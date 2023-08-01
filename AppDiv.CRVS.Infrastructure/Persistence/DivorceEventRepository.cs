using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Features.DivorceEvents.Query;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Infrastructure.Services;
using MediatR;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class DivorceEventRepository : BaseRepository<DivorceEvent>, IDivorceEventRepository
    {
        private readonly CRVSDbContext dbContext;
        private readonly IMediator _mediator;

        public DatabaseFacade Database => dbContext.Database;

        public DivorceEventRepository(CRVSDbContext dbContext, IMediator mediator) : base(dbContext)
        {
            this.dbContext = dbContext;
            this._mediator = mediator;
        }
        public IQueryable<DivorceEvent> GetAllQueryableAsync()
        {

            return dbContext.DivorceEvents.AsQueryable();
        }
        // 08db641e-a7a6-44cc-868e-4b3a02249d49--bride
        // 08db641e-a801-4cb6-8778-eaf20f9cdf41 groom
        public void EFUpdate(DivorceEvent DivorceEvent)
        {
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
            dbContext.DivorceEvents.Update(DivorceEvent);
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
                if(wifes.Count <= 1)
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