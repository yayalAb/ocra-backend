

using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Service
{
    public class EventPaymentRequestService : IEventPaymentRequestService
    {
        private readonly IPaymentRateRepository _paymentRateRepository;
        private readonly IPaymentRequestRepository _paymentRequestRepository;

        public EventPaymentRequestService(IPaymentRateRepository paymentRateRepository, IPaymentRequestRepository paymentRequestRepository)
        {
            _paymentRateRepository = paymentRateRepository;
            _paymentRequestRepository = paymentRequestRepository;
        }
        public async Task CreatePaymentRequest(string eventType, Guid eventId ,CancellationToken cancellationToken)
        {
            var paymentRate = await _paymentRateRepository.GetAllQueryableAsync()
                .Include(p => p.PaymentTypeLookup)
                .Include(p => p.EventLookup)
                .Where(p =>
                p.PaymentTypeLookup.Value.Value<string>("en") == Enum.GetName<PaymentType>(PaymentType.CertificateGeneration)
                    && p.EventLookup.Value.Value<string>("en").ToLower() == eventType.ToLower())
                    .FirstOrDefaultAsync();
            if (paymentRate == null)
            {
                throw new NotFoundException("payment rate not found");
            }
            var paymentRequest = new PaymentRequest
            {
                EventId = eventId,
                Amount = paymentRate.Amount,
                status = false,
                PaymentRateId = paymentRate.Id,
                Reason = new JObject{
                        {"en",$"for {eventType}  certificate generation payment "},
                        {"or",$"for {eventType}  certificate generation payment "},
                        {"am",$"for {eventType}  certificate generation payment "}
                      }
            };
            await _paymentRequestRepository.InsertAsync(paymentRequest , cancellationToken);
            await _paymentRequestRepository.SaveChangesAsync(cancellationToken);


        }

    }
}
