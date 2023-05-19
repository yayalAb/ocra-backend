using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class PaymentExamptionRequestRepository : BaseRepository<PaymentExamptionRequest>, IPaymentExamptionRequestRepository
    {
        private readonly CRVSDbContext _dbContext;
        public PaymentExamptionRequestRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }
        public bool exists(Guid id)
        {
            return _dbContext.PaymentExamptionRequests.Where(m => m.Id == id).Any();
        }
        public IQueryable<PaymentExamptionRequest> GetAllQueryable()
        {

            return _dbContext.PaymentExamptionRequests.AsQueryable();
        }
        // public async Task<IEnumerable<PaymentExamptionRequest>> GetByEventAsync(Guid id)
        // {
        //     return await _dbContext.PaymentExamptionRequests.Where(c => c.EventId == id).ToListAsync();
        // }
    }
}