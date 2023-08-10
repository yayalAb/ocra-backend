using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IBirthEventRepository : IBaseRepository<BirthEvent>
    {
        public DatabaseFacade Database { get; }
        Task<BirthEvent> GetWithIncludedAsync(Guid id);
        Task InsertOrUpdateAsync(BirthEvent entity, CancellationToken cancellationToken);
        Task UpdateAll(BirthEvent entity, IEventPaymentRequestService paymentRequestService, CancellationToken cancellationToken);
    }
}