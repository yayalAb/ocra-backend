using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Enums;
using AppDiv.CRVS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class AdoptionEventRepository : BaseRepository<AdoptionEvent>, IAdoptionEventRepository
    {
        private readonly CRVSDbContext _dbContext;

        public AdoptionEventRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }
        public DatabaseFacade Database => _dbContext.Database;
        public virtual async Task<AdoptionEvent?> GetWithAsync(Guid id)
        {
            return await _dbContext.AdoptionEvents
                            .Include(d => d.Event).ThenInclude(d => d.PaymentExamption)
                            // .Include(d => d.Event).ThenInclude(e => e.EventOwener)
                            .Include(d => d.AdoptiveFather)
                            .Include(d => d.AdoptiveMother)
                            .Include(d => d.CourtCase)
                            .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task InsertOrUpdateAsync(AdoptionEvent entity, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(entity.Event.EventOwener.Id.ToString()))
            {
                _dbContext.PersonalInfos.Update(entity.Event.EventOwener);
                entity.Event.EventOwener = null;
            }
            if (!string.IsNullOrEmpty(entity.Event.EventRegistrar.RegistrarInfo.Id.ToString()))
            {
                _dbContext.PersonalInfos.Update(entity.Event.EventRegistrar.RegistrarInfo);
                entity.Event.EventRegistrar.RegistrarInfo = null;
            }
            if (!string.IsNullOrEmpty(entity.AdoptiveFather.Id.ToString()))
            {
                _dbContext.PersonalInfos.Update(entity.AdoptiveFather);
                entity.AdoptiveFather = null;
            }
            if (!string.IsNullOrEmpty(entity.AdoptiveMother.Id.ToString()))
            {
                _dbContext.PersonalInfos.Update(entity.AdoptiveMother);
                entity.AdoptiveMother = null;
            }

            await base.InsertAsync(entity, cancellationToken);
            await base.SaveChangesAsync(cancellationToken);
        }


        public async Task EFUpdate(AdoptionEvent adoptionEvent,IEventPaymentRequestService paymentRequestService, CancellationToken cancellationToken)
        {
            adoptionEvent.Event.PaymentExamption = await HelperService.UpdatePaymentExamption(adoptionEvent.Event, _dbContext, paymentRequestService,cancellationToken);
            base.UpdateWithNested(adoptionEvent);
            await base.SaveChangesAsync(cancellationToken);
        }



    }
}
