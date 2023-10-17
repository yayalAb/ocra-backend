using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;
namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface ISharedReportRepository: IBaseRepository<SharedReport>
    {
        Task<SharedReport> GetByIdAsync(Guid id);
    }
}