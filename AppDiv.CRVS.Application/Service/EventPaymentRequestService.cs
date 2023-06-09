

using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
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
        private readonly ILookupRepository _lookupRepository;

        public EventPaymentRequestService(IPaymentRateRepository paymentRateRepository, IPaymentRequestRepository paymentRequestRepository, ILookupRepository lookupRepository)
        {
            _paymentRateRepository = paymentRateRepository;
            _paymentRequestRepository = paymentRequestRepository;
            _lookupRepository = lookupRepository;
        }
        public async Task<(float amount, string code)> CreatePaymentRequest(string eventType, Event Event, string paymentType, CancellationToken cancellationToken)
        {
            var nationalityLookup = _lookupRepository.GetAll().Where(x => x.Id == Event.EventOwener.NationalityLookupId).FirstOrDefault();
            if (nationalityLookup == null)
            {
                throw new NotFoundException($"nationality with id = {Event.EventOwener.NationalityLookupId} is not found");
            }
            var isForeign = !(nationalityLookup.ValueStr.ToLower().Contains("ethiopia")
                                 || nationalityLookup.ValueStr.ToLower().Contains("ኢትዮጵያ")
                                  || nationalityLookup.ValueStr.ToLower().Contains("itoophiyaa"));
            eventType = eventType.ToLower();
            var paymentRate = _paymentRateRepository.GetAll()
                .Include(p => p.PaymentTypeLookup)
                .Include(p => p.EventLookup)
                .Where(p => p.IsForeign == isForeign)
                .Where(p => EF.Functions.Like(p.EventLookup.ValueStr.ToLower(), $"%{eventType}%"))
                .Where(p => EF.Functions.Like(p.PaymentTypeLookup.ValueStr.ToLower(), $"%{paymentType.ToLower()}%"))
                .FirstOrDefault();
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
            var newRequest = new AddRequest
            {
                RequestType = paymentType,
                CivilRegOfficerId = new Guid("08db642b-8c3c-4d7b-8943-14a01aa0c19c"),
                currentStep = 0
            };

            var paymentRequest = new PaymentRequest
            {
                EventId = Event?.Id,
                Amount = paymentRate.Amount,
                Request = CustomMapper.Mapper.Map<Request>(newRequest),
                status = false,
                PaymentCode = paymentCode,
                PaymentRateId = paymentRate.Id,
                Reason = new JObject{
                        {"en",$"for {eventType}  certificate {paymentType} payment "},
                        { "or",$"for {eventType}  certificate {paymentType} payment "},
                        { "am",$"for {eventType}  certificate {paymentType} payment "}
                      }
            };
            await _paymentRequestRepository.InsertAsync(paymentRequest, cancellationToken);
            _paymentRequestRepository.SaveChanges();
            return (amount: paymentRate.Amount, code: paymentCode);
        }

    }
}
