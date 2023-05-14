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
        private readonly CRVSDbContext dbContext;

        public PaymentRateRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<PaymentRate> GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }
        public IQueryable<PaymentRate> GetAllQueryableAsync()
        {

            return dbContext.PaymentRates.AsQueryable();
        }

        public override async Task InsertAsync(PaymentRate paymentRate, CancellationToken cancellationToken)
        {
            await base.InsertAsync(paymentRate, cancellationToken);
        }
    }

}