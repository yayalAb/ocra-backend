
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Application.Features.CertificateStores.CertificateTransfers.Command.Create;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface ICertificateTransferRepository : IBaseRepository<CertificateSerialTransfer>
    {
        Task UpdateWithRangeAsync(CertificateSerialTransfer transfer, CancellationToken cancellationToken);
        // public Task<Guid> Add(CertificateTemplate certificateTemplate);
        // public new IQueryable<CertificateTemplate> GetAllAsync();
    }
}
