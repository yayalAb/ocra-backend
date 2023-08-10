using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IAdoptionEventRepository : IBaseRepository<AdoptionEvent>
    {
        Task<AdoptionEvent> GetWithAsync(Guid id);
        Task InsertOrUpdateAsync(AdoptionEvent entity, CancellationToken cancellationToken);
        Task EFUpdate(AdoptionEvent adoptionEvent, IEventPaymentRequestService paymentRequestService, CancellationToken cancellationToken);
        public DatabaseFacade Database { get; }



    }
}

