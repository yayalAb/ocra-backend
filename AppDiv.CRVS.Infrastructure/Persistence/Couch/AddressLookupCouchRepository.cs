using AppDiv.CRVS.Application.Interfaces.Persistence.Couch;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Infrastructure.Context;
using AppDiv.CRVS.Infrastructure.CouchModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Infrastructure.Persistence.Couch;
public class AddressLookupCouchRepository : IAddressLookupCouchRepository
{
    private readonly CRVSCouchDbContext _couchContext;
    private readonly CRVSDbContext _dbContext;
    private readonly ILogger<AddressLookupCouchRepository> _logger;



    public AddressLookupCouchRepository(CRVSCouchDbContext couchContext, CRVSDbContext dbContext, ILogger<AddressLookupCouchRepository> logger)
    {
        _couchContext = couchContext;
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<bool> InserAsync(AddressCouchDTO address)
    {
        if (address.AdminLevel == 1)
        {

            var newCountry = new CountryCouch
            {
                Id2 = (Guid)address.Id,
                NameAm = address.AddressName?.Value<string>("am"),
                NameOr = address.AddressName?.Value<string>("or"),
                Status = address.Status,
                DeletedStatus = false
            };
            var res = await _couchContext.Countries.AddAsync(newCountry);
        }
        else
        {

            var newAddress = new AddressCouch
            {
                Id2 = address.Id,
                NameStr = address.AddressNameStr,
                AdminLevel = address.AdminLevel,
                AdminTypeAm = address.AdminTypeLookup == null ? null : address.AdminTypeLookup.Value.Value<string>("am"),
                AdminTypeOr = address.AdminTypeLookup == null ? null : address.AdminTypeLookup.Value.Value<string>("or"),
                ParentAddressId = address.ParentAddressId,
                Status = address.Status,
                DeletedStatus = false,
                addresses = address.ChildAddresses == null
                            ? new List<SingleAddressCouch>()
                            : address.ChildAddresses.Select(ca => new SingleAddressCouch
                            {
                                Id = ca.Id,
                                NameAm = ca.AddressName == null ? null : ca.AddressName.Value<string>("am"),
                                NameOr = ca.AddressName == null ? null : ca.AddressName.Value<string>("or"),
                                AdminLevel = ca.AdminLevel,
                                AdminTypeAm = ca.AdminTypeLookup == null ? null : ca.AdminTypeLookup.Value.Value<string>("am"),
                                AdminTypeOr = ca.AdminTypeLookup == null ? null : ca.AdminTypeLookup.Value.Value<string>("or"),
                                ParentAddressId = ca.ParentAddressId,
                                Status = ca.Status

                            }).ToList(),
            };
            var res2 = await _couchContext.AddressCouches.AddAsync(newAddress);


            var parentAddress = _couchContext.AddressCouches
                            .Where(a => a.Id2 == address.ParentAddressId)
                            .FirstOrDefault();
            // if parent is not registered as parent ie. first child registration
            if (parentAddress == null)
            {
                var newParentWithChild = new AddressCouch
                {
                    Id2 = address.ParentAddressId,
                    NameStr = address.AddressNameStr,
                    AdminLevel = address.AdminLevel,
                    AdminTypeAm = address.AdminTypeLookup == null ? null : address.AdminTypeLookup.Value.Value<string>("am"),
                    AdminTypeOr = address.AdminTypeLookup == null ? null : address.AdminTypeLookup.Value.Value<string>("or"),
                    ParentAddressId = address.ParentAddressId,
                    Status = address.Status,
                    DeletedStatus = false,
                    addresses = new List<SingleAddressCouch>{
                        new SingleAddressCouch
                        {
                            Id = address.Id,
                            ParentAddressId = address.ParentAddressId ?? Guid.Empty,
                            NameAm = address.AddressName?.Value<string>("am"),
                            NameOr = address.AddressName?.Value<string>("or"),
                            AdminLevel = address.AdminLevel,
                            AdminTypeAm = address.AdminTypeLookup == null ? null : address.AdminTypeLookup.Value.Value<string>("am"),
                            AdminTypeOr = address.AdminTypeLookup == null ? null : address.AdminTypeLookup.Value.Value<string>("or"),

                        }
                    },
                };
                var res4 = await _couchContext.AddressCouches.AddAsync(newParentWithChild);


            }
            else
            {

                var existingChild = parentAddress.addresses?.Where(ca => ca.Id == address.Id).FirstOrDefault();
                if (existingChild == null)
                {
                    // parent already have list of other addresses push the new address to  the list
                    if (parentAddress.addresses != null)
                    {
                        parentAddress.addresses?.Add(new SingleAddressCouch
                        {
                            Id = address.Id,
                            ParentAddressId = address.ParentAddressId ?? Guid.Empty,
                            NameAm = address.AddressName?.Value<string>("am"),
                            NameOr = address.AddressName?.Value<string>("or"),
                            AdminLevel = address.AdminLevel,
                            Status = address.Status,
                            AdminTypeAm = address.AdminTypeLookup == null ? null : address.AdminTypeLookup.Value.Value<string>("am"),
                            AdminTypeOr = address.AdminTypeLookup == null ? null : address.AdminTypeLookup.Value.Value<string>("or"),

                        });
                    }
                    else
                    {
                        parentAddress.addresses = new List<SingleAddressCouch>{
                            new SingleAddressCouch{
                                Id = address.Id,
                                ParentAddressId = address.ParentAddressId ?? Guid.Empty,
                                NameAm = address.AddressName?.Value<string>("am"),
                                NameOr = address.AddressName?.Value<string>("or"),
                                AdminLevel = address.AdminLevel,
                                AdminTypeAm = address.AdminTypeLookup == null ? null : address.AdminTypeLookup.Value.Value<string>("am"),
                                AdminTypeOr = address.AdminTypeLookup == null ? null : address.AdminTypeLookup.Value.Value<string>("or"),
                                Status = address.Status,

                            }
                        };
                    }


                }
                else
                {
                    existingChild.ParentAddressId = address.ParentAddressId ?? Guid.Empty;
                    existingChild.NameAm = address.AddressName?.Value<string>("am");
                    existingChild.NameOr = address.AddressName?.Value<string>("or");
                    existingChild.AdminLevel = address.AdminLevel;
                    existingChild.AdminTypeAm = address.AdminTypeLookup == null ? null : address.AdminTypeLookup.Value.Value<string>("am");
                    existingChild.AdminTypeOr = address.AdminTypeLookup == null ? null : address.AdminTypeLookup.Value.Value<string>("or");
                    existingChild.Status = address.Status;
                }
                var res3 = await _couchContext.AddressCouches.AddOrUpdateAsync(parentAddress);



            }

        }
        return true;
    }
    public async Task<bool> BulkInsertAsync(IQueryable<Address> addresses)
    {
        var countries = addresses.Where(a => a.AdminLevel == 1)
                                .Select(a => new CountryCouch
                                {
                                    Id2 = a.Id,
                                    NameAm = a.AddressName == null ? null : a.AddressName.Value<string>("am"),
                                    NameOr = a.AddressName == null ? null : a.AddressName.Value<string>("or"),
                                    Status = a.Status,
                                    DeletedStatus = false
                                }).ToList();
        await _couchContext.Countries.AddOrUpdateRangeAsync(countries);


        var selected = addresses.GroupBy(a => new { parentId = a.ParentAddressId, parentAddressName = a.ParentAddress.AddressNameStr, parentParentAddressId = a.ParentAddress.ParentAddressId }).Select(g => new AddressCouch
        {
            Id2 = g.Key.parentId,
            NameStr = g.Key.parentAddressName,
            DeletedStatus = false,
            ParentAddressId = g.Key.parentParentAddressId,
            addresses = g.Select(ca => new SingleAddressCouch
            {
                Id = ca.Id,
                NameAm = ca.AddressName == null ? null : ca.AddressName.Value<string>("am"),
                NameOr = ca.AddressName == null ? null : ca.AddressName.Value<string>("or"),
                AdminLevel = ca.AdminLevel,
                AdminTypeAm = ca.AdminTypeLookup == null ? null : ca.AdminTypeLookup.Value.Value<string>("am"),
                AdminTypeOr = ca.AdminTypeLookup == null ? null : ca.AdminTypeLookup.Value.Value<string>("or"),
                ParentAddressId = ca.ParentAddressId,
                Status = ca.Status,
            }).ToList()
        });


        var res = await _couchContext.AddressCouches.AddOrUpdateRangeAsync(selected.ToList());
        // insert kebeles  
        var kebeles = addresses.Where(a => a.AdminLevel == 5)
                        .Select(k => new AddressCouch
                        {
                            Id2 = k.Id,
                            NameStr = k.AddressNameStr,
                            ParentAddressId = k.ParentAddressId,
                            AdminLevel = k.AdminLevel,
                            Status = k.Status,
                            DeletedStatus = false,
                            addresses = new List<SingleAddressCouch>()
                        });
        var kebeleRes = await _couchContext.AddressCouches.AddOrUpdateRangeAsync(kebeles.ToList());

        return true;
    }


    public async Task<bool> UpdateAsync(AddressCouchDTO address)
    {
        if (address.AdminLevel == 1)
        {
            var existing = _couchContext.Countries.Where(c => c.Id2 == address.Id).FirstOrDefault();
            if (existing != null)
            {
                existing.Id2 = address.Id;
                existing.NameAm = address.AddressName?.Value<string>("am");
                existing.NameOr = address.AddressName?.Value<string>("or");
                existing.Status = address.Status;
                var res3 = await _couchContext.Countries.AddOrUpdateAsync(existing);

            }

        }
        else
        {

            var existingAddressAsParent = _couchContext.AddressCouches.Where(a => a.Id2 == address.Id).FirstOrDefault();
            if (existingAddressAsParent != null)
            {
                existingAddressAsParent.ParentAddressId = address?.ParentAddressId ?? Guid.Empty;
                existingAddressAsParent.NameStr = address?.AddressNameStr;
                existingAddressAsParent.AdminLevel = address?.AdminLevel;
                existingAddressAsParent.AdminTypeAm = address?.AdminTypeLookup == null ? null : address?.AdminTypeLookup?.Value?.Value<string>("am");
                existingAddressAsParent.AdminTypeOr = address?.AdminTypeLookup == null ? null : address?.AdminTypeLookup?.Value?.Value<string>("or");
                existingAddressAsParent.Status = address?.Status;
                await _couchContext.AddressCouches.AddOrUpdateAsync(existingAddressAsParent);
            }

            var existingParent = _couchContext.AddressCouches.Where(a => a.Id2 == address.ParentAddressId).FirstOrDefault();
            if (existingParent != null)
            {
                var child = existingParent.addresses.Where(a => a.Id == address.Id).FirstOrDefault();
                if (child != null)
                {
                    child.ParentAddressId = address?.ParentAddressId ?? Guid.Empty;
                    child.NameAm = address?.AddressName?.Value<string>("am");
                    child.NameOr = address?.AddressName?.Value<string>("or");
                    child.AdminLevel = address?.AdminLevel;
                    child.AdminTypeAm = address?.AdminTypeLookup == null ? null : address?.AdminTypeLookup?.Value?.Value<string>("am");
                    child.AdminTypeOr = address?.AdminTypeLookup == null ? null : address?.AdminTypeLookup?.Value?.Value<string>("or");
                    child.Status = address?.Status;
                    var res = await _couchContext.AddressCouches.AddOrUpdateAsync(existingParent);
                }



            }
        }


        return true;
    }
    public async Task<bool> RemoveAsync(AddressCouchDTO address)
    {

        if (address.AdminLevel == 1)
        {
            var existing = _couchContext.Countries.Where(c => c.Id2 == address.Id).FirstOrDefault();
            _logger.LogCritical($"removing{existing}");
            if (existing != null)
            {
                existing.DeletedStatus = true;
                await _couchContext.Countries.AddOrUpdateAsync(existing);
            }
        }
        else
        {

            var existing = _couchContext.AddressCouches.Where(l => l.Id2 == address.Id).FirstOrDefault();
            if (existing != null)
            {
                existing.DeletedStatus = true;
                await _couchContext.AddressCouches.AddOrUpdateAsync(existing);
            }

            var existingParent = _couchContext.AddressCouches.Where(a => a.Id2 == address.ParentAddressId).FirstOrDefault();
            if (existingParent != null)
            {
                var existingChild = existingParent.addresses.Where(a => a.Id == address.Id).FirstOrDefault();
                if (existingChild != null)
                {
                    existingParent.addresses.Remove(existingChild);
                    await _couchContext.AddressCouches.AddOrUpdateAsync(existingParent);

                }

            }

        }

        return true;
    }
    public async Task<bool> IsEmpty()
    {
        // return !(await _couchContext.);
        var count = _couchContext.Client.GetDatabase<AddressCouch>().ToList().Count;
        return count == 0;
    }
}
