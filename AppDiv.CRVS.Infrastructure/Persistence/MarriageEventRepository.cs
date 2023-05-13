using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
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
        public async Task InsertOrUpdateAsync(MarriageEvent entity, CancellationToken cancellationToken)
        {
            logger.LogCritical($"jkjkjk....{entity.Event.EventOwener.Id}");
            if (entity.Event.EventOwener.Id != null && entity.Event.EventOwener.Id != Guid.Empty)
            {
                dbContext.PersonalInfos.Update(entity.Event.EventOwener);
                entity.Event.EventOwener = null!;
            }
            if (entity.Event.EventRegistrar?.RegistrarInfo.Id != null && entity.Event.EventRegistrar.RegistrarInfo.Id != Guid.Empty)
            {
                dbContext.PersonalInfos.Update(entity.Event.EventRegistrar.RegistrarInfo);
                entity.Event.EventRegistrar.RegistrarInfo = null!;
            }
            if (entity.BrideInfo.Id != null && entity.BrideInfo.Id != Guid.Empty)
            {
                dbContext.PersonalInfos.Update(entity.BrideInfo);
                entity.BrideInfo = null!;
            }
            entity.Witnesses?.ToList().ForEach(witness =>
            {
                if (witness.WitnessPersonalInfo.Id != null && witness.WitnessPersonalInfo.Id != Guid.Empty)
                {
                    dbContext.PersonalInfos.Update(witness.WitnessPersonalInfo);
                    witness.WitnessPersonalInfoId = witness.WitnessPersonalInfo.Id;
                    witness.WitnessPersonalInfo = null!;
                }
            });
            var db = dbContext.Database;

            await base.InsertAsync(entity, cancellationToken);


        }
    }
}