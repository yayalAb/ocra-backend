using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Application.Interfaces.Persistence.Couch;
using Microsoft.EntityFrameworkCore;
using  AppDiv.CRVS.Application.Contracts.DTOs;
using  AppDiv.CRVS.Infrastructure.CouchModels;
using AppDiv.CRVS.Application.Mapper;


namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class PaymentRateRepository : BaseRepository<PaymentRate>, IPaymentRateRepository
    {
        private readonly CRVSDbContext dbContext;
        private readonly IPaymentRateCouchRepository paymentRateCouchRepo;

        public PaymentRateRepository(CRVSDbContext dbContext , IPaymentRateCouchRepository paymentRateCouchRepo) : base(dbContext)
        {
            this.dbContext = dbContext;
            this.paymentRateCouchRepo=  paymentRateCouchRepo;
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
          public virtual async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
        {

            var entries = dbContext.ChangeTracker
              .Entries()
              .Where(e => e.Entity is PaymentRate &&
                      (e.State == EntityState.Added
                      || e.State == EntityState.Modified || e.State == EntityState.Deleted));
            List<PaymentRateEntry> paymentRateEntries = entries.Select(e => new PaymentRateEntry
            {
                State = e.State,
                PaymentRate = CustomMapper.Mapper.Map<PaymentRateCouchDTO>((PaymentRate)e.Entity)
            }).ToList();

            bool saveRes = await base.SaveChangesAsync(cancellationToken);

            if (saveRes)
            {
                foreach (var entry in paymentRateEntries)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            await paymentRateCouchRepo.InsertPaymentRateAsync(entry.PaymentRate);
                            break;
                        case EntityState.Modified:
                            await paymentRateCouchRepo.UpdatePaymentRateAsync(entry.PaymentRate);
                            break;
                        case EntityState.Deleted:
                            await paymentRateCouchRepo.RemovePaymentRateAsync(entry.PaymentRate);
                            break;
                        default: break;

                    }
                }

            }
            return saveRes;


        }
          public async Task InitializePaymentRateCouch()
        {
            var empty = await paymentRateCouchRepo.IsEmpty();
            if (empty)
            {
                await paymentRateCouchRepo.BulkInsertAsync
                (dbContext.PaymentRates
                .Include(pr => pr.EventLookup)
                .Include(pr => pr.PaymentTypeLookup)
                );
            }
        }
    }

}