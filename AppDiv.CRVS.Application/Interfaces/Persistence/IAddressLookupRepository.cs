using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IAddressLookupRepository : IBaseRepository<Address>
    {
        Task<IEnumerable<Address>> GetAllAsync();
        Task<Address> GetByIdAsync(Guid id);
        Task<Address> GetAddressByKey(string key);
        Task<Address> GetAddressAdminstrativeLevel(Guid id);
        Task InitializeAddressLookupCouch();
        Task<object> GetAllAddressFromView();
        Task<(object createdAddresses, object updatedAddresses, DateTime date)> GetLastUpdatedAddresses(DateTime since);
        Task Import(ICollection<Address> addresses, CancellationToken cancellationToken);
        // Task<string> DeleteAsync(Address entities);

    }
}
