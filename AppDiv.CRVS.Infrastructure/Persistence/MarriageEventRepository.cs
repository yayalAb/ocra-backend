
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class MarriageEventRepository : BaseRepository<MarriageEvent>, IMarriageEventRepository
    {
        public DatabaseFacade Database => dbContext.Database;
        private readonly CRVSDbContext dbContext;
        private readonly ILogger<MarriageEventRepository> logger;

        public MarriageEventRepository(CRVSDbContext dbContext, ILogger<MarriageEventRepository> logger) : base(dbContext)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }
        public IQueryable<MarriageEvent> GetAllQueryableAsync()
        {

            return dbContext.MarriageEvents.AsQueryable();
        }
        public void EFUpdate(MarriageEvent marriageEvent)
        {
            dbContext.MarriageEvents.Update(marriageEvent);
        }
        public bool exists(Guid id)
        {
            return dbContext.MarriageEvents.Where(m => m.Id == id).Any();
        }
        public async Task InsertWitness(List<Witness> witnesses)
        {
            witnesses.ToList().ForEach(witness =>
            {
                if (witness.WitnessPersonalInfo.Id != null && witness.WitnessPersonalInfo.Id != Guid.Empty)
                {
                    dbContext.PersonalInfos.Update(witness.WitnessPersonalInfo);
                    witness.WitnessPersonalInfoId = witness.WitnessPersonalInfo.Id;
                    witness.WitnessPersonalInfo = null!;

                }
            });
            await dbContext.Witnesses.AddRangeAsync(witnesses);
        }
        public async Task InsertOrUpdateAsync(MarriageEvent entity, bool isUpdate, CancellationToken cancellationToken)
        {
            if (entity.Event.EventOwener.Id != null && entity.Event.EventOwener.Id != Guid.Empty)
            {
                //find and update existing personalInfo
                var keyValuePair = new Dictionary<string, object>{
                    {"SexLookupId" ,entity.Event.EventOwener.SexLookupId},
                    {"ReligionLookupId" ,entity.Event.EventOwener.ReligionLookupId},
                    {"EducationalStatusLookupId" ,entity.Event.EventOwener.EducationalStatusLookupId},
                    {"TypeOfWorkLookupId" ,entity.Event.EventOwener.TypeOfWorkLookupId},
                    {"MarriageStatusLookupId" ,entity.Event.EventOwener.MarriageStatusLookupId},
                    {"ResidentAddressId" ,entity.Event.EventOwener.ResidentAddressId},
                    {"NationalId", entity.Event.EventOwener.NationalId}
                };
                await updatePersonalInfo(keyValuePair, entity.Event.EventOwenerId, "Event Owner");
                entity.Event.EventOwenerId = entity.Event.EventOwener.Id;
                entity.Event.EventOwener = null!;
            }
            if (entity.BrideInfo.Id != null && entity.BrideInfo.Id != Guid.Empty)
            {
                var keyValuePair = new Dictionary<string, object>{
                    {"NationalId",entity.BrideInfo.NationalId},
                    {"SexLookupId",entity.BrideInfo.SexLookupId},
                    {"ReligionLookupId", entity.BrideInfo.ReligionLookupId},
                    {"EducationalStatusLookupId", entity.BrideInfo.EducationalStatusLookupId},
                    {"TypeOfWorkLookupId", entity.BrideInfo.TypeOfWorkLookupId},
                    {"MarriageStatusLookupId", entity.BrideInfo.MarriageStatusLookupId},
                    {"ResidentAddressId", entity.BrideInfo.ResidentAddressId}
                };
                await updatePersonalInfo(keyValuePair, entity.BrideInfo.Id, "BrideInfo");
                entity.BrideInfoId = entity.BrideInfo.Id;
                entity.BrideInfo = null!;

            }

            entity.Witnesses?.ToList().ForEach(async witness =>
            {
                if (witness.WitnessPersonalInfo.Id != null && witness.WitnessPersonalInfo.Id != Guid.Empty)
                {
                    var keyValuePair = new Dictionary<string, object>{
                        {"SexLookupId",witness.WitnessPersonalInfo.SexLookupId},
                        {"NationalId",witness.WitnessPersonalInfo.NationalId},
                        {"ResidentAddressId",witness.WitnessPersonalInfo.ResidentAddressId}
                    };
                    await updatePersonalInfo(keyValuePair, witness.WitnessPersonalInfo.Id, "witness");
                    witness.WitnessPersonalInfoId = witness.WitnessPersonalInfo.Id;
                    witness.WitnessPersonalInfo = null!;

                }
            });
            if (isUpdate)
            {
                dbContext.MarriageEvents.Update(entity);
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