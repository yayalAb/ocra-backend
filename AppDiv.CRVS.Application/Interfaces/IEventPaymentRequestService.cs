
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IEventPaymentRequestService
    {
        public  Task CreatePaymentRequest(string eventType, Event Event ,CancellationToken cancellationToken);
        
    }
}
