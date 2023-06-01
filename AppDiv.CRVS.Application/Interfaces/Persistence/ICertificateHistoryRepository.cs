using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface ICertificateHistoryRepository : IBaseRepository<CertificateHistory>
    {
        Task<IEnumerable<CertificateHistory>> GetAllAsync();
        Task<CertificateHistory> GetByIdAsync(Guid id);
    }
}
