using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class DivorceEventRepository : BaseRepository<DivorceEvent>, IDivorceEventRepository
    {
        private readonly CRVSDbContext dbContext;
        public DatabaseFacade Database => dbContext.Database;

        public DivorceEventRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public IQueryable<DivorceEvent> GetAllQueryableAsync()
        {

            return dbContext.DivorceEvents.AsQueryable();
        }
        public void EFUpdate(DivorceEvent DivorceEvent)
        {
            var existingOwner =  dbContext.PersonalInfos.Find(DivorceEvent.Event.EventOwener.Id);
            if (existingOwner == null)
            {
                throw new NotFoundException($"divorce event with the provided id is not found");
            }

            var eventOwnerFeilds = new Dictionary<string, object>{
                    {"NationalId",DivorceEvent.Event.EventOwener.NationalId},
                    {"SexLookupId",DivorceEvent.Event.EventOwener.SexLookupId},
                    {"ReligionLookupId",DivorceEvent.Event.EventOwener.ReligionLookupId},
                    {"EducationalStatusLookupId",DivorceEvent.Event.EventOwener.EducationalStatusLookupId},
                    {"TypeOfWorkLookupId",DivorceEvent.Event.EventOwener.TypeOfWorkLookupId},
                    {"MarriageStatusLookupId",DivorceEvent.Event.EventOwener.MarriageStatusLookupId},
                    {"ResidentAddressId",DivorceEvent.Event.EventOwener.ResidentAddressId}
            };
            DivorceEvent.Event.EventOwener = HelperService.UpdateObjectFeilds<PersonalInfo>(existingOwner, eventOwnerFeilds);
             var existingWife =  dbContext.PersonalInfos.Find(DivorceEvent.DivorcedWife.Id);
            if (existingWife == null)
            {
                throw new NotFoundException($"divorce event owner info with the provided id is not found");
            }
             var divorcedWifeFeilds = new Dictionary<string, object>{
                    {"NationalId",DivorceEvent.DivorcedWife.NationalId},
                    {"SexLookupId",DivorceEvent.DivorcedWife.SexLookupId},
                    {"ReligionLookupId",DivorceEvent.DivorcedWife.ReligionLookupId},
                    {"EducationalStatusLookupId",DivorceEvent.DivorcedWife.EducationalStatusLookupId},
                    {"TypeOfWorkLookupId",DivorceEvent.DivorcedWife.TypeOfWorkLookupId},
                    {"MarriageStatusLookupId",DivorceEvent.DivorcedWife.MarriageStatusLookupId},
                    {"ResidentAddressId",DivorceEvent.DivorcedWife.ResidentAddressId},
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

            



        }
        private async Task updatePersonalInfo(Dictionary<string, object> keyValuePair, Guid id, string feildName)
        {
            var existing = await dbContext.PersonalInfos.FindAsync(id);
            if (existing == null)
            {
                throw new NotFoundException($"{feildName} with the provided id is not found");
            }

            existing = HelperService.UpdateObjectFeilds<PersonalInfo>(existing, keyValuePair);
            dbContext.PersonalInfos.Update(existing);

        }

    }
}