using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IPaymentExamptionRequestRepository : IBaseRepository<PaymentExamptionRequest>
    {
        Task<PaymentExamptionRequest> GetByIdAsync(Guid id);
        // Task<IEnumerable<Certificate>> GetByEventAsync(Guid id);
    }
}