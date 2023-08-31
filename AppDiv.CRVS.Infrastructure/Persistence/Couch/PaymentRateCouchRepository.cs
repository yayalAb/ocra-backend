using AppDiv.CRVS.Application.Interfaces.Persistence.Couch;
using AppDiv.CRVS.Infrastructure.Context;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Infrastructure.CouchModels;
namespace AppDiv.CRVS.Infrastructure.Persistence.Couch
{
    public class PaymentRateCouchRepository : IPaymentRateCouchRepository
    {
        private readonly CRVSCouchDbContext _couchContext;

        public PaymentRateCouchRepository(CRVSCouchDbContext couchContext)
        {
            _couchContext = couchContext;
        }
        public async Task<bool> InsertPaymentRateAsync(PaymentRateCouchDTO paymentRate)
        {
            var newPaymentRate = new PaymentRateCouch
            {
                Id = paymentRate.Id,
                PaymentTypeLookupId = paymentRate.PaymentTypeLookup == null ? null : paymentRate.PaymentTypeLookup.Id,
                EventLookupId = paymentRate.EventLookup == null ? null : paymentRate.EventLookup.Id,
                EventLookupAm = paymentRate.EventLookup == null ? null : paymentRate.EventLookup.Value?.Value<string>("am"),
                EventLookupOr = paymentRate.EventLookup == null ? null : paymentRate.EventLookup.Value?.Value<string>("or"),
                PaymentTypeLookupAm = paymentRate.PaymentTypeLookup == null ? null : paymentRate.PaymentTypeLookup.Value?.Value<string>("am"),
                PaymentTypeLookupOr = paymentRate.PaymentTypeLookup == null ? null : paymentRate.PaymentTypeLookup.Value?.Value<string>("or"),
                PaymentTypeLookupEn = paymentRate.PaymentTypeLookup == null ? null : paymentRate.PaymentTypeLookup.Value?.Value<string>("en"),

                Amount = paymentRate.Amount,
                Backlog = paymentRate.Backlog,
                HasCamera = paymentRate.HasCamera,
                HasVideo = paymentRate.HasVideo,
                Status = paymentRate.Status,
                IsForeign = paymentRate.IsForeign,
                DeletedStatus = false
            };
            await _couchContext.PaymentRates.AddAsync(newPaymentRate);
            return true;
        }
        public async Task<bool> BulkInsertAsync(IQueryable<PaymentRate> paymentRates)
        {
            var res = await _couchContext.PaymentRates.AddOrUpdateRangeAsync(paymentRates.Select(
             pr => new PaymentRateCouch
             {
                 Id = pr.Id,
                 PaymentTypeLookupId = pr.PaymentTypeLookup == null ? null : pr.PaymentTypeLookup.Id,
                 EventLookupId = pr.EventLookup == null ? null : pr.EventLookup.Id,
                 EventLookupAm = pr.EventLookup == null ? null : pr.EventLookup.Value.Value<string>("am"),
                 EventLookupOr = pr.EventLookup == null ? null : pr.EventLookup.Value.Value<string>("or"),
                 PaymentTypeLookupAm = pr.PaymentTypeLookup == null ? null : pr.PaymentTypeLookup.Value.Value<string>("am"),
                 PaymentTypeLookupOr = pr.PaymentTypeLookup == null ? null : pr.PaymentTypeLookup.Value.Value<string>("or"),
                 PaymentTypeLookupEn = pr.PaymentTypeLookup == null ? null : pr.PaymentTypeLookup.Value.Value<string>("en"),
                 Amount = pr.Amount,
                 Backlog = pr.Backlog,
                 HasCamera = pr.HasCamera,
                 HasVideo = pr.HasVideo,
                 Status = pr.Status,
                 IsForeign = pr.IsForeign,
                 DeletedStatus = false
             }
            ).ToList());
            return true;
        }
        public async Task<bool> UpdatePaymentRateAsync(PaymentRateCouchDTO paymentRate)
        {
            var existing = _couchContext.PaymentRates.Where(pr => pr.Id == paymentRate.Id).FirstOrDefault();
            if (existing != null)
            {

                existing.PaymentTypeLookupId = paymentRate.PaymentTypeLookup?.Id;
                existing.EventLookupId = paymentRate.EventLookup?.Id;
                existing.EventLookupAm = paymentRate.EventLookup?.Value?.Value<string>("am");
                existing.EventLookupOr = paymentRate.EventLookup?.Value?.Value<string>("or");
                existing.PaymentTypeLookupAm = paymentRate.PaymentTypeLookup?.Value?.Value<string>("am");
                existing.PaymentTypeLookupOr = paymentRate.PaymentTypeLookup?.Value?.Value<string>("or");
                existing.PaymentTypeLookupEn = paymentRate.PaymentTypeLookup?.Value?.Value<string>("en");

                existing.Amount = paymentRate.Amount;
                existing.Backlog = paymentRate.Backlog;
                existing.HasCamera = paymentRate.HasCamera;
                existing.HasVideo = paymentRate.HasVideo;
                existing.Status = paymentRate.Status;
                existing.IsForeign = paymentRate.IsForeign;

                var res = await _couchContext.PaymentRates.AddOrUpdateAsync(existing);
            }


            return true;
        }
        public async Task<bool> RemovePaymentRateAsync(PaymentRateCouchDTO paymentRate)
        {
            var existing = _couchContext.PaymentRates.Where(pr => pr.Id == paymentRate.Id).FirstOrDefault();
            if (existing != null)
            {
                existing.DeletedStatus = true;
                await _couchContext.PaymentRates.AddOrUpdateAsync(existing);
            }
            return true;
        }
        public async Task<bool> IsEmpty()
        {
            var count = _couchContext.Client.GetDatabase<PaymentRateCouch>().ToList().Count;
            return count == 0;
        }
    }
}
