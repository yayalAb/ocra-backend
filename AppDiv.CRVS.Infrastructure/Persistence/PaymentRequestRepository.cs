
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;


namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class PaymentRequestRepository : BaseRepository<PaymentRequest>, IPaymentRequestRepository
    {
        private readonly CRVSDbContext _dbContext;

        public PaymentRequestRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }




    }

}