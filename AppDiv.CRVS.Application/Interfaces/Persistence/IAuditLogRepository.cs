using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities.Audit;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IAuditLogRepository : IBaseRepository<AuditLog>
    {
        IQueryable<AuditLog> GetAllGrid();
    }
}