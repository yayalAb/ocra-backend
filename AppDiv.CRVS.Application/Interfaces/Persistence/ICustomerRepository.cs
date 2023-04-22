
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;


namespace AppDiv.CRVS.Domain.Repositories
{
    // Interface for CustomerQueryRepository
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        //Custom operation which is not generic
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(Guid id);
        Task<Customer> GetCustomerByEmail(string email);
    }
}
