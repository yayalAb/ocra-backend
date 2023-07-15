using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence.Couch
{
    public interface IPaymentRateCouchRepository
    {
        public Task<bool> InsertPaymentRateAsync(PaymentRateCouchDTO paymentRate);
        public Task<bool> UpdatePaymentRateAsync(PaymentRateCouchDTO paymentRate);
        public Task<bool> RemovePaymentRateAsync(PaymentRateCouchDTO paymentRate);
        public Task<bool> BulkInsertAsync(IQueryable<PaymentRate> paymentRate);
        public Task<bool> IsEmpty();
    }
}