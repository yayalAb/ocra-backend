
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface ICertificateTransferRepository : IBaseRepository<CertificateSerialTransfer>
    {
        Task InsertWithRangeAsync(CertificateSerialTransfer transfer, CancellationToken cancellationToken);
        // public Task<Guid> Add(CertificateTemplate certificateTemplate);
        // public new IQueryable<CertificateTemplate> GetAllAsync();
    }
}
