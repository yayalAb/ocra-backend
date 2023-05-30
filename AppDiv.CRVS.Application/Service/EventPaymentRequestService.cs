

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
        public async Task<(float amount, string code)> CreatePaymentRequest(string eventType, Event Event, CancellationToken cancellationToken)
        {
            var isForeign = !(Event.EventOwener.NationalityLookup.ValueStr.ToLower().Contains("ethiopia")
                                 || Event.EventOwener.NationalityLookup.ValueStr.ToLower().Contains("ኢትዮጵያ")
                                  || Event.EventOwener.NationalityLookup.ValueStr.ToLower().Contains("itoophiyaa"));
            var paymentType = (Enum.GetName<PaymentType>(PaymentType.CertificateGeneration)!).ToLower();
            eventType = eventType.ToLower();
            var paymentRate = await _paymentRateRepository.GetAllQueryableAsync()
                .Include(p => p.PaymentTypeLookup)
                .Include(p => p.EventLookup)
                .Where(p => p.IsForeign == isForeign)
                .Where(p => EF.Functions.Like(p.EventLookup.ValueStr.ToLower(), $"%{eventType}%"))
                .Where(p => EF.Functions.Like(p.PaymentTypeLookup.ValueStr.ToLower(), $"%{paymentType}%"))

                .FirstOrDefaultAsync();
            // 
            //  throw new NotFoundException("payment rate not found");
            if (paymentRate == null)
            {
                throw new NotFoundException("payment rate not found");
            }
            string paymentCode = "";
            do
            {
                paymentCode = HelperService.GenerateRandomCode();
            }
            while (_paymentRequestRepository.GetAll().Any(x => x.PaymentCode == paymentCode));


            var paymentRequest = new PaymentRequest
            {
                EventId = Event.Id,
                Amount = paymentRate.Amount,
                status = false,
                PaymentCode = paymentCode,
                PaymentRateId = paymentRate.Id,
                Reason = new JObject{
                        {"en",$"for {eventType}  certificate generation payment "},
                        {"or",$"for {eventType}  certificate generation payment "},
                        {"am",$"for {eventType}  certificate generation payment "}
                      }
            };
            await _paymentRequestRepository.InsertAsync(paymentRequest, cancellationToken);
            await _paymentRequestRepository.SaveChangesAsync(cancellationToken);

            return (amount: paymentRate.Amount, code: paymentCode);
        }

    }
}
