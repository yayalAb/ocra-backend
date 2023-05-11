using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class MarriageEventRepository : BaseRepository<MarriageEvent>, IMarriageEventRepository
    {
        public DatabaseFacade Database => dbContext.Database ;
        private readonly CRVSDbContext dbContext;

        public MarriageEventRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
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
            if (!string.IsNullOrEmpty(entity.Event.EventOwener.Id.ToString()))
            {
                dbContext.PersonalInfos.Update(entity.Event.EventOwener);
                entity.Event.EventOwener = null!;
            }
            if (!string.IsNullOrEmpty(entity.Event.EventRegistrar?.RegistrarInfo.Id.ToString()))
            {
                dbContext.PersonalInfos.Update(entity.Event.EventRegistrar.RegistrarInfo);
                entity.Event.EventRegistrar.RegistrarInfo = null!;
            }
            if (!string.IsNullOrEmpty(entity.BrideInfo.Id.ToString()))
            {
                dbContext.PersonalInfos.Update(entity.BrideInfo);
                entity.BrideInfo = null!;
            }
            entity.Witnesses?.ToList().ForEach(witness =>
            {
                if (witness.WitnessPersonalInfo.Id != null)
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