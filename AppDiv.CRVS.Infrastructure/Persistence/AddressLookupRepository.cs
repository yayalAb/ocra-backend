using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Infrastructure.Context;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class AddressLookupRepository : BaseRepository<Address>, IAddressLookupRepository
    {
        private readonly CRVSDbContext _DbContext;
        public AddressLookupRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _DbContext = dbContext;
        }

        // Task<Address> IAddressLookupRepository.DeleteAsync(Address entities)
        // {
        //     _DbContext.Remove(entities);
        //     return entities;
        // }

        Task<Address> IAddressLookupRepository.GetAddressAdminstrativeLevel(Guid id)
        {
            return base.GetFirstEntryAsync(x => x.Id.Equals(id), q => q.Id, Utility.Contracts.SortingDirection.Ascending);
        }

        async Task<Address> IAddressLookupRepository.GetAddressByKey(string key)
        {
            return await base.GetAsync(key);
        }

        async Task<Address> IAddressLookupRepository.GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }
    }
}