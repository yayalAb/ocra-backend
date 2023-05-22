
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
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
        public async Task EFUpdateAsync(MarriageEvent marriageEvent)
        {
            var existingOwner = await dbContext.PersonalInfos.FindAsync(marriageEvent.Event.EventOwener.Id);
            if (existingOwner == null)
            {
                throw new NotFoundException($"eventOwner with the provided id is not found");
            }
            //find and update existingOwner personalInfo
            var keyValuePair1 = new Dictionary<string, object>{
                    {"SexLookupId" ,marriageEvent.Event.EventOwener.SexLookupId},
                    {"ReligionLookupId" ,marriageEvent.Event.EventOwener.ReligionLookupId},
                    {"EducationalStatusLookupId" ,marriageEvent.Event.EventOwener.EducationalStatusLookupId},
                    {"TypeOfWorkLookupId" ,marriageEvent.Event.EventOwener.TypeOfWorkLookupId},
                    {"MarriageStatusLookupId" ,marriageEvent.Event.EventOwener.MarriageStatusLookupId},
                    {"ResidentAddressId" ,marriageEvent.Event.EventOwener.ResidentAddressId},
                    {"NationalId", marriageEvent.Event.EventOwener.NationalId}
                };
            marriageEvent.Event.EventOwener = HelperService.UpdateObjectFeilds<PersonalInfo>(existingOwner, keyValuePair1);

            var existingBrideInfo = await dbContext.PersonalInfos.FindAsync(marriageEvent.BrideInfo.Id);
            if (existingBrideInfo == null)
            {
                throw new NotFoundException($"bride info with the provided id is not found");
            }
            var keyValuePair2 = new Dictionary<string, object>{
                    {"NationalId",marriageEvent.BrideInfo.NationalId},
                    {"SexLookupId",marriageEvent.BrideInfo.SexLookupId},
                    {"ReligionLookupId", marriageEvent.BrideInfo.ReligionLookupId},
                    {"EducationalStatusLookupId", marriageEvent.BrideInfo.EducationalStatusLookupId},
                    {"TypeOfWorkLookupId", marriageEvent.BrideInfo.TypeOfWorkLookupId},
                    {"MarriageStatusLookupId", marriageEvent.BrideInfo.MarriageStatusLookupId},
                    {"ResidentAddressId", marriageEvent.BrideInfo.ResidentAddressId}
                };
            marriageEvent.BrideInfo = HelperService.UpdateObjectFeilds<PersonalInfo>(existingBrideInfo, keyValuePair2);


        //     var witnessIds = new List<Guid>();

        //     marriageEvent.Witnesses?.ToList().ForEach(async witness =>
        //     {
        //         witnessIds.Add(witness.WitnessPersonalInfo.Id);
        //     });
        //   var existingWitnesses =  await dbContext.PersonalInfos.Where(ug => witnessIds.Contains(ug.Id)).ToListAsync();
        //   marriageEvent.Witnesses?.ToList().ForEach(witness =>{
        //  var keyValuePair3 = new Dictionary<string, object>{
        //                 {"SexLookupId",witness.WitnessPersonalInfo.SexLookupId},
        //                 {"NationalId",witness.WitnessPersonalInfo.NationalId},
        //                 {"ResidentAddressId",witness.WitnessPersonalInfo.ResidentAddressId}
        //             };
        //             witness.WitnessPersonalInfo = HelperService.UpdateObjectFeilds<PersonalInfo>(existingWitness, keyValuePair3);

        //   });
            // marriageEvent.Witnesses?.ToList().ForEach(async witness =>
            // {
            //     if (witness.WitnessPersonalInfo.Id != null && witness.WitnessPersonalInfo.Id != Guid.Empty)
            //     {
            //         var existingWitness = await dbContext.PersonalInfos.FindAsync(witness.WitnessPersonalInfo.Id);
            //         if (existingWitness == null)
            //         {
            //             throw new NotFoundException($"bride info with the provided id is not found");
            //         }
            //         var keyValuePair3 = new Dictionary<string, object>{
            //             {"SexLookupId",witness.WitnessPersonalInfo.SexLookupId},
            //             {"NationalId",witness.WitnessPersonalInfo.NationalId},
            //             {"ResidentAddressId",witness.WitnessPersonalInfo.ResidentAddressId}
            //         };
            //         witness.WitnessPersonalInfo = HelperService.UpdateObjectFeilds<PersonalInfo>(existingWitness, keyValuePair3);

            //     }
            // });

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
        public async Task InsertOrUpdateAsync(MarriageEvent entity, CancellationToken cancellationToken)
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