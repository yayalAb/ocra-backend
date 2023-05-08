using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface ICertificateRepository : IBaseRepository<Certificate>
    {
        Task<IEnumerable<Certificate>> GetByEventAsync(Guid id);
    }
}