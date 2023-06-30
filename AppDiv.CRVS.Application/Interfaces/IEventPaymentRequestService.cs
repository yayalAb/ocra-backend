
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IEventPaymentRequestService
    {
        public Task<(float amount, string code)> CreatePaymentRequest(string eventType, Event Event,
         string paymentType, Guid? RequestId, bool IsUseCamera, CancellationToken cancellationToken);
        public Task<bool> IsActive(string eventType, DateTime EventDate, DateTime EventRegDate);

    }
}
