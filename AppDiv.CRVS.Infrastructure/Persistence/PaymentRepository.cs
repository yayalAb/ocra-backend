using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        private readonly CRVSDbContext _dbContext;

        public PaymentRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task UpdateEventPaymentStatus(Guid PaymentRequestId)
        {
            var paymentRequest = await _dbContext.PaymentRequests
                .Where(pr => pr.Id == PaymentRequestId)
                .Include(pr => pr.Event).FirstOrDefaultAsync();
            if (paymentRequest == null)
            {
                throw new NotFoundException("payment request not found");
            }
            paymentRequest.Event.IsPaid = true;
            _dbContext.Events.Update(paymentRequest.Event);

        }



    }

}