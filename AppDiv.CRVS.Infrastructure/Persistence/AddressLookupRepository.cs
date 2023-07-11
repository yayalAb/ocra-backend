using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces.Persistence.Couch;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Infrastructure.Context;
using AppDiv.CRVS.Application.Mapper;

using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Infrastructure.CouchModels;
using AutoMapper.QueryableExtensions;



namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class AddressLookupRepository : BaseRepository<Address>, IAddressLookupRepository
    {
        private readonly CRVSDbContext _DbContext;
        private readonly IAddressLookupCouchRepository addressLookupCouchRepo;
        private readonly ILogger<AddressLookupRepository> _logger;


        public AddressLookupRepository(CRVSDbContext dbContext, IAddressLookupCouchRepository addressLookupCouchRepo , ILogger<AddressLookupRepository> logger ) : base(dbContext)
        {
            _DbContext = dbContext;
            this.addressLookupCouchRepo = addressLookupCouchRepo;
            _logger = logger;
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

            List<AddressEntry> addressEntries = entries.Select(e =>new AddressEntry{
                State = e.State,
                Address = CustomMapper.Mapper.Map<AddressCouchDTO>( (Address)e.Entity)
            }).ToList();   
                

            var saveChangeRes = await base.SaveChangesAsync(cancellationToken);
            

            if (saveChangeRes)
            {


                foreach (var entry in addressEntries)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            await addressLookupCouchRepo.InserAsync(entry.Address);
                            break;
                        case EntityState.Deleted:
                            await addressLookupCouchRepo.RemoveAsync(entry.Address);
                            break;
                        case EntityState.Modified:
                            await addressLookupCouchRepo.UpdateAsync(entry.Address);
                            break;
                        default: break;

                    }
                }
            }

            return saveChangeRes;
        }
        public async Task InitializeAddressLookupCouch()
        {
            var empty = await addressLookupCouchRepo.IsEmpty();
            if (empty)
            {
                await addressLookupCouchRepo.BulkInsertAsync(_DbContext.Addresses
                .Include(a => a.AdminTypeLookup)
                .Include(a => a.ParentAddress)
                );


            }
        }
    }
}

