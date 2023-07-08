using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Persistence.Couch;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Infrastructure.Context;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class AddressLookupRepository : BaseRepository<Address>, IAddressLookupRepository
    {
        private readonly CRVSDbContext _DbContext;
        private readonly IAddressLookupCouchRepository addressLookupCouchRepo;

        public AddressLookupRepository(CRVSDbContext dbContext, IAddressLookupCouchRepository addressLookupCouchRepo) : base(dbContext)
        {
            _DbContext = dbContext;
            this.addressLookupCouchRepo = addressLookupCouchRepo;
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

        // public bool Exists(Guid id)
        // {
        //     return _DbContext.Addresses.Where(p => p.Id == id).Any();
        // }
        public virtual async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
        {

            var entries = _DbContext.ChangeTracker
              .Entries()
              .Where(e => e.Entity is Address &&
                      (e.State == EntityState.Added
                      || e.State == EntityState.Modified
                      || e.State == EntityState.Deleted));
            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    // case EntityState.Added:
                    //     await addressLookupCouchRepo.InserAsync((Address)entry.Entity);
                    //     break;
                    case EntityState.Deleted:
                        await addressLookupCouchRepo.RemoveAsync((Address)entry.Entity);
                        break;
                    // case EntityState.Modified:
                    //     await addressLookupCouchRepo.UpdateAsync((Address)entry.Entity);
                    //     break;
                    default: break;

                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
        public async Task InitializeAddressLookupCouch()
        {
            var empty = await addressLookupCouchRepo.IsEmpty();
            if (empty)
            {
                await addressLookupCouchRepo.BulkInsertAsync(_DbContext.Addresses
                .Include(a => a.AdminTypeLookup)
                );


            }
        }
    }
}

