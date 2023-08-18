using System.Diagnostics.Tracing;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Service
{
    public class EventPaymentRequestService : IEventPaymentRequestService
    {
        private readonly IPaymentRateRepository _paymentRateRepository;
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly ILookupRepository _lookupRepository;
        private readonly IPersonalInfoRepository _personalInfoRepo;
        private readonly ISettingRepository _SettinglookupRepository;

        public EventPaymentRequestService(ISettingRepository SettinglookupRepository,
                                          IPaymentRateRepository paymentRateRepository,
                                          IPaymentRequestRepository paymentRequestRepository,
                                          ILookupRepository lookupRepository,
                                          IPersonalInfoRepository personalInfoRepo)
        {
            _paymentRateRepository = paymentRateRepository;
            _paymentRequestRepository = paymentRequestRepository;
            _lookupRepository = lookupRepository;
            _personalInfoRepo = personalInfoRepo;
            _SettinglookupRepository = SettinglookupRepository;
        }
        public async Task<(float amount, string code)> CreatePaymentRequest(string eventType, Event Event, string paymentType,
         Guid? RequestId, bool IsUseCamera, bool HasVideo, CancellationToken cancellationToken)
        {
        
            var nationalityLookupId = Event.EventOwener != null
                            ? Event.EventOwener.NationalityLookupId
                            : _personalInfoRepo.GetAll().Where(p => p.Id == Event.EventOwenerId).Select(p => p.NationalityLookupId).FirstOrDefault();
            var nationalityLookup = _lookupRepository.GetAll().Where(x => nationalityLookupId != null && x.Id == nationalityLookupId).FirstOrDefault();
            if (nationalityLookup == null)
            {
                throw new NotFoundException($"nationality with id = {nationalityLookupId} is not found");
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
                return (0, "Action Done without payment");
            }
            string paymentCode = "";
            do
            {
                paymentCode = HelperService.GenerateRandomCode();
            }
            while (_paymentRequestRepository.GetAll().Any(x => x.PaymentCode == paymentCode));
            string massage = "";
            float amount = 0;
            if (paymentType.ToLower() == "certificategeneration")
            {
                if (eventType.ToLower() == "marriage")
                {
                    amount = await this.IsActive(eventType, Event.EventDate, Event.EventRegDate) ? paymentRate.Amount : paymentRate.Backlog;
                    amount = IsUseCamera ? amount + paymentRate.HasCamera : amount;
                    amount = HasVideo ? amount + paymentRate.HasCamera : amount;
                }
                else
                {
                    amount = await this.IsActive(eventType, Event.EventDate, Event.EventRegDate) ? paymentRate.Amount : paymentRate.Backlog;

                }
                massage = " Certificate Generation";
            }
            else
            {
                amount = paymentRate.Amount;
                massage = " Certificate " + paymentType;
            }
            if (amount != 0)
            {
                var paymentRequest = new PaymentRequest
                {
                    EventId = Event?.Id,
                    Amount = amount,
                    RequestId = RequestId,
                    status = false,
                    PaymentCode = paymentCode,
                    PaymentRateId = paymentRate.Id,
                    Reason = new JObject{
                        {"en",$" {eventType} {massage}"},
                        { "or",$" {eventType} {massage}"},
                        { "am",$" {eventType} {massage}"}
                      }
                };
                await _paymentRequestRepository.InsertAsync(paymentRequest, cancellationToken);
            }


            _paymentRequestRepository.SaveChanges();
            return (amount: paymentRate.Amount, code: paymentCode);
        }

        public async Task RemovePaymentRequest(Guid eventId)
        {


        }

        public async Task<bool> IsActive(string eventType, DateTime EventDate, DateTime EventRegDate)
        {
            string eve = eventType.ToLower() + "Setting";
            var defualtAddress = _SettinglookupRepository.GetAll().Where(x => x.Key == eve).FirstOrDefault();
            if (defualtAddress == null)
            {
                throw new NotFoundException("Defualt Address not Found");
            }
            int days = int.Parse(defualtAddress.Value.Value<string>("active_registration"));

            TimeSpan deff = EventRegDate - EventDate;
            int daysDef = Convert.ToInt32(deff.TotalDays);
            return daysDef <= days;
        }

    }
}
