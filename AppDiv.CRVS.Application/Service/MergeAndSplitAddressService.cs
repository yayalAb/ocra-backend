using Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Application.Common;

namespace AppDiv.CRVS.Application.Service
{
    public class MergeAndSplitAddressService : IMergeAndSplitAddressService
    {
        private readonly IAddressLookupRepository _AddressRepository;
        public MergeAndSplitAddressService(IAddressLookupRepository AddressRepository)
        {
            _AddressRepository = AddressRepository;

        }

        public async Task<BaseResponse> MergeAndSplitAddress(List<AddAddressRequest> AddressList, CancellationToken cancellationToken)
        {
            Guid? Id;
            foreach (var add in AddressList)
            {
                var Address = new Address
                {
                    Id = Guid.NewGuid(),
                    AddressName = add.AddressName,
                    StatisticCode = add.StatisticCode,
                    Code = add.Code,
                    AdminLevel = add.AdminLevel,
                    AreaTypeLookupId = add.AreaTypeLookupId,
                    ParentAddressId = add.ParentAddressId,
                    AdminTypeLookupId = add.AdminTypeLookupId,
                    OldAddressId = add.Id
                };
                Guid ParentAddressId = Address.Id;
                string Code = Address.Code;

                await _AddressRepository.InsertAsync(Address, cancellationToken);
                var selectAddress = _AddressRepository.GetAll().Where(x => x.Id == add.Id).FirstOrDefault();
                selectAddress.Status = true;
                await _AddressRepository.UpdateAsync(selectAddress, x => x.Id);

                if (add.AdminLevel != 5)
                {
                    IQueryable<Address> totalChilders = _AddressRepository.GetAll()
                                       .Include(x => x.ParentAddress)
                                       .ThenInclude(x => x.ParentAddress)
                                       .ThenInclude(x => x.ParentAddress);

                    if (add.AdminLevel == 1)
                    {
                        totalChilders = totalChilders.Where(x =>
                             (x.ParentAddress.ParentAddress.ParentAddress.ParentAddress.Id == add.Id)
                            || (x.ParentAddress.ParentAddress.ParentAddress.Id == add.Id)
                            || (x.ParentAddress.ParentAddress.Id == add.Id)
                            || (x.ParentAddress.Id == add.Id)).OrderBy(x => x.CreatedAt);
                    }
                    else if (add.AdminLevel == 2)
                    {
                        totalChilders = totalChilders.Where(x =>
                            (x.ParentAddress.ParentAddress.ParentAddress.Id == add.Id)
                            || (x.ParentAddress.ParentAddress.Id == add.Id)
                            || (x.ParentAddress.Id == add.Id)).OrderBy(x => x.CreatedAt);
                    }
                    else if (add.AdminLevel == 3)
                    {

                        totalChilders = totalChilders.Where(x =>
                             (x.ParentAddress.ParentAddress.Id == add.Id)
                            || (x.ParentAddress.Id == add.Id)).OrderBy(x => x.CreatedAt);
                    }
                    else if (add.AdminLevel == 4)
                    {
                        totalChilders = totalChilders.Where(x =>
                             (x.ParentAddress.Id == add.Id)).OrderBy(x => x.CreatedAt);
                    }






                    // totalChilders.BatchUpdateAsync(a => new Address { Status = "Active" });
                    // foreach (var address in totalChilders)
                    // {
                    //     address.Status = true;
                    //     await _AddressRepository.UpdateAsync(address, x => x.Id);
                    // }
                    Console.WriteLine("total childes {0} ", totalChilders.Count());
                }
            }
            return new BaseResponse { Message = "Address Migrated Sucessfully" };
        }
    }
}
