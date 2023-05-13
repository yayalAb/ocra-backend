
namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IEventPaymentRequestService
    {
        public  Task CreatePaymentRequest(string eventType, Guid eventId ,CancellationToken cancellationToken);
        
    }
}
