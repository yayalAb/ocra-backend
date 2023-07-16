using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IPaymentRateRepository : IBaseRepository<PaymentRate>
    {
        Task<IEnumerable<PaymentRate>> GetAllAsync();
        Task<PaymentRate> GetByIdAsync(Guid id);
        IQueryable<PaymentRate> GetAllQueryableAsync();
        Task InitializePaymentRateCouch();
    }
}