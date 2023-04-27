using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class PaymentRateRepository : BaseRepository<PaymentRate>, IPaymentRateRepository
    {
        public PaymentRateRepository(CRVSDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<PaymentRate> GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }

        public async Task<IEnumerable<PaymentRate>> GetAllAsync(Guid id)
        {
            return await base.GetAllAsync();
        }

        public override async Task InsertAsync(PaymentRate paymentRate, CancellationToken cancellationToken)
        {
            await base.InsertAsync(paymentRate, cancellationToken);
        }
    }

}