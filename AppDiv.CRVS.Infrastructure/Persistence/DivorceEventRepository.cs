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
            dbContext.DivorceEvents.Update(DivorceEvent);
        }
        public bool exists(Guid id)
        {
            return dbContext.DivorceEvents.Where(m => m.Id == id).Any();
        }
        public async Task InsertOrUpdateAsync(DivorceEvent entity, bool isUpdate, CancellationToken cancellationToken)
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
            if (isUpdate)
            {
                dbContext.DivorceEvents.Update(entity);
            }
            else
            {
                
                await base.InsertAsync(entity, cancellationToken);

            }



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