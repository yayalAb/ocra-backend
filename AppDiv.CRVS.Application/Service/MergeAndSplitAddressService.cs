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
using AppDiv.CRVS.Application.Mapper;

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
            var Address = AddressList.Select(add => new Address
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
            });
            var address = CustomMapper.Mapper.Map<List<Address>>(Address);
            var updateAddress = CustomMapper.Mapper.Map<List<Address>>(AddressList);
            updateAddress.ForEach(x => x.Status = true);
            await _AddressRepository.UpdateAsync(address, x => x.Id);
            await _AddressRepository.InsertAsync(address, cancellationToken);
            foreach (var item in address)
            {
                if (item.AdminLevel == 3)
                {
                    var childs = _AddressRepository.GetAll().
                    Where(x => x.ParentAddressId == item.OldAddressId);
                }

            }

            await _AddressRepository.SaveChangesAsync(cancellationToken);

            return new BaseResponse { Message = "Address Migrated Sucessfully" };
        }


        public async Task<bool> UpdateAddress(Address add, CancellationToken cancellationToken)
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
            await _AddressRepository.SaveChangesAsync(cancellationToken);
            var Chlds = _AddressRepository.GetAll().Where(x => x.ParentAddressId == add.Id && !x.Status).FirstOrDefault();
            if (Chlds != null)
            {
                // var update = _AddressRepository.GetAll().Where(x => x.ParentAddressId == add.Id && !x.Status);
                Chlds.Code = Code + Chlds.CodePostfix;
                Chlds.ParentAddressId = ParentAddressId;
                await UpdateAddress(Chlds, cancellationToken);
            }
            else
            {
                var Parent = _AddressRepository.GetAll()
                .Where(x => x.Id == add.Id && !x.Status).FirstOrDefault();

                var Parent1 = _AddressRepository.GetAll()
                .Where(x => x.ParentAddressId == Parent.ParentAddressId && !x.Status).FirstOrDefault();
                if (Parent != null)
                {
                    Chlds.Code = Code + Chlds.CodePostfix;
                    Chlds.ParentAddressId = ParentAddressId;
                    await UpdateAddress(Parent, cancellationToken);
                }
            }

            return true;
        }

    }
}












// totalChilders.BatchUpdateAsync(a => new Address { Status = "Active" });
// foreach (var address in totalChilders)
// {
//     address.Status = true;
//     await _AddressRepository.UpdateAsync(address, x => x.Id);
// }
// Console.WriteLine("total childes {0} ", totalChilders.Count());
//     }
// }


// foreach (var add in AddressList)
// {
//     var Address = new Address
//     {
//         Id = Guid.NewGuid(),
//         AddressName = add.AddressName,
//         StatisticCode = add.StatisticCode,
//         Code = add.Code,
//         AdminLevel = add.AdminLevel,
//         AreaTypeLookupId = add.AreaTypeLookupId,
//         ParentAddressId = add.ParentAddressId,
//         AdminTypeLookupId = add.AdminTypeLookupId,
//         OldAddressId = add.Id
//     };
//     Guid ParentAddressId = Address.Id;
//     string Code = Address.Code;

//     await _AddressRepository.InsertAsync(Address, cancellationToken);
//     var selectAddress = _AddressRepository.GetAll().Where(x => x.Id == add.Id).FirstOrDefault();
//     selectAddress.Status = true;
//     await _AddressRepository.UpdateAsync(selectAddress, x => x.Id);
//     if (add.AdminLevel != 5)
//     {
//         IQueryable<Address> totalChilders = _AddressRepository.GetAll()
//                            .Include(x => x.ParentAddress)
//                            .ThenInclude(x => x.ParentAddress)
//                            .ThenInclude(x => x.ParentAddress);


//         if (add.AdminLevel == 1)
//         {
//             totalChilders = totalChilders.Where(x =>
//                  (x.ParentAddress.ParentAddress.ParentAddress.ParentAddress.Id == add.Id)
//                 || (x.ParentAddress.ParentAddress.ParentAddress.Id == add.Id)
//                 || (x.ParentAddress.ParentAddress.Id == add.Id)
//                 || (x.ParentAddress.Id == add.Id)).OrderBy(x => x.CreatedAt);
//         }
//         else if (add.AdminLevel == 2)
//         {
//             totalChilders = totalChilders.Where(x =>
//                 (x.ParentAddress.ParentAddress.ParentAddress.Id == add.Id)
//                 || (x.ParentAddress.ParentAddress.Id == add.Id)
//                 || (x.ParentAddress.Id == add.Id)).OrderBy(x => x.CreatedAt);
//         }
//         else if (add.AdminLevel == 3)
//         {

//             totalChilders = totalChilders.Where(x =>
//                  (x.ParentAddress.ParentAddress.Id == add.Id)
//                 || (x.ParentAddress.Id == add.Id)).OrderBy(x => x.CreatedAt);
//         }
//         else if (add.AdminLevel == 4)
//         {
//             totalChilders = totalChilders.Where(x =>
//                  (x.ParentAddress.Id == add.Id)).OrderBy(x => x.CreatedAt);
//         }
//     }
// }