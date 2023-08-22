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
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Localization;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using AppDiv.CRVS.Domain.Enums;
using AppDiv.CRVS.Infrastructure.Services;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class AddressLookupRepository : BaseRepository<Address>, IAddressLookupRepository
    {
        private readonly CRVSDbContext _DbContext;
        private readonly IAddressLookupCouchRepository addressLookupCouchRepo;
        private readonly ILogger<AddressLookupRepository> _logger;


        public AddressLookupRepository(CRVSDbContext dbContext, IAddressLookupCouchRepository addressLookupCouchRepo, ILogger<AddressLookupRepository> logger) : base(dbContext)
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

        public async Task Import(ICollection<Address> addresses, CancellationToken cancellationToken)
        {
            await _DbContext.Addresses.AddRangeAsync(addresses, cancellationToken);
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

            List<AddressEntry> addressEntries = entries.Select(e => new AddressEntry
            {
                State = e.State,
                AddressId = ((Address)e.Entity).Id
            }).ToList();


            var saveChangeRes = await base.SaveChangesAsync(cancellationToken);



            if (saveChangeRes)
            {
                foreach (var entry in addressEntries)
                {
                    entry.Address = _DbContext.Addresses
                            .Include(a => a.AdminTypeLookup)
                            .Include(a => a.ParentAddress)
                                    .Where(a => a.Id == entry.AddressId)
                                    .Select(a => CustomMapper.Mapper.Map<AddressCouchDTO>(a))
                                    .FirstOrDefault();
                    if (entry.Address != null)
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
            }

            return saveChangeRes;
        }

        public async Task<object> GetAllAddressFromView()
        {
            var sql = "SELECT * FROM addressFetch";
            var result = new List<object>();
            var viewReader = await HelperService.ConnectDatabase(sql, _DbContext);
            while (viewReader.Item1.Read())
            {
                result.Add(new
                {
                    id = viewReader.Item1["Id"],
                    nameAm = viewReader.Item1["nameAm"],
                    nameOr = viewReader.Item1["nameOr"],
                    adminLevel = viewReader.Item1["AdminLevel"],
                    adminTypeAm = viewReader.Item1["adminTypeAm"],
                    adminTypeOr = viewReader.Item1["adminTypeOr"],
                    mergeStatus = viewReader.Item1["status"],
                    addresses = JsonConvert.DeserializeObject<JArray>(viewReader.Item1["addresses"].ToString()),
                    // cou = viewReader.Item1["cou"]
                });
            }
            await viewReader.Item2.CloseAsync();


            return result;
        }
        public async Task<(object createdAddresses, object updatedAddresses, DateTime date)> GetLastUpdatedAddresses(DateTime since)
        {
            var timestamp = since.ToString("yyyy-MM-dd HH:mm:ss");
            var sql = $"SELECT * FROM addressWithDate where CreatedAt > '{timestamp}' or ModifiedAt > '{timestamp}';";
            var created = new List<object>();
            var updated = new List<object>();
            var viewReader = await HelperService.ConnectDatabase(sql, _DbContext);
            while (viewReader.Item1.Read())
            {
                if ((DateTime)viewReader.Item1["CreatedAt"] > since)
                {

                    created.Add(new
                    {
                        id = viewReader.Item1["Id"],
                        nameAm = viewReader.Item1["nameAm"],
                        nameOr = viewReader.Item1["nameOr"],
                        adminLevel = viewReader.Item1["AdminLevel"],
                        adminTypeAm = viewReader.Item1["adminTypeAm"],
                        adminTypeOr = viewReader.Item1["adminTypeOr"],
                        mergeStatus = viewReader.Item1["status"],
                    });
                }
                else
                {
                    updated.Add(new
                    {
                        id = viewReader.Item1["Id"],
                        nameAm = viewReader.Item1["nameAm"],
                        nameOr = viewReader.Item1["nameOr"],
                        adminLevel = viewReader.Item1["AdminLevel"],
                        adminTypeAm = viewReader.Item1["adminTypeAm"],
                        adminTypeOr = viewReader.Item1["adminTypeOr"],
                        mergeStatus = viewReader.Item1["status"],
                    });
                }
            }
            await viewReader.Item2.CloseAsync();

            return (createdAddresses: created, updatedAddresses: updated, date: DateTime.Now);

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

