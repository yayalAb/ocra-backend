using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IUpdateEventPaymetnService
    {
        public Task UpdatePaymetnStatus(Guid paymentRequestId, CancellationToken cancellationToken);

    }
}