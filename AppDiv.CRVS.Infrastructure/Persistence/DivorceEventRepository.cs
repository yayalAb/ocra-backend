using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class DivorceEventRepository : BaseRepository<DivorceEvent>, IDivorceEventRepository
    {
        private readonly CRVSDbContext dbContext;

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
        public async Task InsertOrUpdateAsync(DivorceEvent entity, CancellationToken cancellationToken)
        {
            if (entity.Event.EventOwener.Id != null && entity.Event.EventOwener.Id != Guid.Empty)
            {
                dbContext.PersonalInfos.Update(entity.Event.EventOwener);
                entity.Event.EventOwenerId = entity.Event.EventOwener.Id;
                entity.Event.EventOwener = null;
            }
            if (entity.Event.EventRegistrar?.RegistrarInfo.Id != null && entity.Event.EventRegistrar?.RegistrarInfo.Id != Guid.Empty)
            {
                dbContext.PersonalInfos.Update(entity.Event.EventRegistrar?.RegistrarInfo!);
                entity.Event.EventRegistrar!.RegistrarInfoId = entity.Event.EventRegistrar.RegistrarInfo.Id;
                entity.Event.EventRegistrar!.RegistrarInfo = null;
            }
            if (entity.DivorcedWife.Id != null && entity.DivorcedWife.Id != Guid.Empty)
            {
                dbContext.PersonalInfos.Update(entity.DivorcedWife);
                entity.DivorcedWifeId = entity.DivorcedWife.Id;
                entity.DivorcedWife = null;
            }


            await base.InsertAsync(entity, cancellationToken);


        }
    }
}