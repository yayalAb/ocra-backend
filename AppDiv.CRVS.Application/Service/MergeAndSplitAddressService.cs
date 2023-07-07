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
            await _AddressRepository.UpdateAsync(updateAddress, x => x.Id);
            await _AddressRepository.InsertAsync(address, cancellationToken);
            await _AddressRepository.SaveChangesAsync(cancellationToken);
            List<ToUpdateList> ListOfAdress1 = new List<ToUpdateList>();
            foreach (var add in address)
            {
                if (add.AdminLevel != 5)
                {
                    var ChildAddress = _AddressRepository.GetAll().Where(x => x.ParentAddressId == add.OldAddressId);
                    var pushToList = new ToUpdateList
                    {
                        AddressList = ChildAddress.ToList(),
                        ParentAddress = add
                    };
                    ListOfAdress1.Add(pushToList);
                }
            }
            await UpdateAddress(ListOfAdress1, cancellationToken);
            await _AddressRepository.SaveChangesAsync(cancellationToken);

            return new BaseResponse { Message = "Address Migrated Sucessfully" };
        }
        public async Task<bool> UpdateAddress(List<ToUpdateList> AddressList, CancellationToken cancellationToken)
        {
            List<ToUpdateList> ListOfAdress = new List<ToUpdateList>();
            foreach (var la in AddressList)
            {
                var NewAddressList = la.AddressList.Select(x => new Address
                {
                    Id = Guid.NewGuid(),
                    AddressName = x.AddressName,
                    StatisticCode = x.StatisticCode,
                    Code = la.ParentAddress.Code + x.CodePostfix,
                    AdminLevel = x.AdminLevel,
                    AreaTypeLookupId = x.AreaTypeLookupId,
                    ParentAddressId = la.ParentAddress.Id,
                    AdminTypeLookupId = x.AdminTypeLookupId,
                    OldAddressId = x.Id
                });
                var address1 = CustomMapper.Mapper.Map<List<Address>>(NewAddressList);
                var updateAddress1 = CustomMapper.Mapper.Map<List<Address>>(la.AddressList);
                updateAddress1.ForEach(x => x.Status = true);
                await _AddressRepository.UpdateAsync(updateAddress1, x => x.Id);
                await _AddressRepository.InsertAsync(address1, cancellationToken);
                await _AddressRepository.SaveChangesAsync(cancellationToken);
                foreach (var chil in NewAddressList)
                {
                    var ChildAddress = _AddressRepository.GetAll().Where(x => x.ParentAddressId == chil.OldAddressId);
                    var pushToList = new ToUpdateList
                    {
                        AddressList = ChildAddress.ToList(),
                        ParentAddress = chil
                    };
                    ListOfAdress.Add(pushToList);
                }
            }

            if (ListOfAdress?.FirstOrDefault()?.AddressList?.FirstOrDefault() != null)
            {
                await UpdateAddress(ListOfAdress, cancellationToken);
            }
            return true;
        }

    }
    public class ToUpdateList
    {
        public List<Address> AddressList { get; set; }
        public Address ParentAddress { get; set; }
    }
}



// if (item.AdminLevel == 4)
// {
//     var childs = _AddressRepository.GetAll().
//     Where(x => x.ParentAddressId == item.OldAddressId);
//     var newAddress = childs.Select(add => new Address
//     {
//         Id = Guid.NewGuid(),
//         AddressName = add.AddressName,
//         StatisticCode = add.StatisticCode,
//         Code = add.Code,
//         AdminLevel = add.AdminLevel,
//         AreaTypeLookupId = add.AreaTypeLookupId,
//         ParentAddressId = add.Id,
//         AdminTypeLookupId = add.AdminTypeLookupId,
//         OldAddressId = add.Id
//     });
//     var NewAddress = CustomMapper.Mapper.Map<List<Address>>(newAddress);
//     var UpdateChildeAddress = CustomMapper.Mapper.Map<List<Address>>(childs);
//     UpdateChildeAddress.ForEach(x => x.Status = true);
//     await _AddressRepository.UpdateAsync(UpdateChildeAddress, x => x.Id);
//     await _AddressRepository.InsertAsync(NewAddress, cancellationToken);

// }


// var NewAddress = AddressList.Select(x => new Address
// {
//     Id = Guid.NewGuid(),
//     AddressName = x.AddressName,
//     StatisticCode = x.StatisticCode,
//     Code = ParentAddress.Code + x.CodePostfix,
//     AdminLevel = x.AdminLevel,
//     AreaTypeLookupId = x.AreaTypeLookupId,
//     ParentAddressId = ParentAddress.Id,
//     AdminTypeLookupId = x.AdminTypeLookupId,
//     OldAddressId = x.Id
// });

// var address1 = CustomMapper.Mapper.Map<List<Address>>(NewAddress);
// var updateAddress1 = CustomMapper.Mapper.Map<List<Address>>(AddressList);
// updateAddress1.ForEach(x => x.Status = true);
// await _AddressRepository.UpdateAsync(updateAddress1, x => x.Id);
// await _AddressRepository.InsertAsync(address1, cancellationToken);





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



// IQueryable<Address> totalChilders = _AddressRepository.GetAll()
//                    .Include(x => x.ParentAddress)
//                    .ThenInclude(x => x.ParentAddress)
//                    .ThenInclude(x => x.ParentAddress);


// if (add.AdminLevel == 1)
// {
//     totalChilders = totalChilders.Where(x =>
//          (x.ParentAddress.ParentAddress.ParentAddress.ParentAddress.Id == add.OldAddressId)
//         || (x.ParentAddress.ParentAddress.ParentAddress.Id == add.OldAddressId)
//         || (x.ParentAddress.ParentAddress.Id == add.OldAddressId)
//         || (x.ParentAddress.Id == add.OldAddressId)).OrderBy(x => x.CreatedAt);
// }
// else if (add.AdminLevel == 2)
// {
//     totalChilders = totalChilders.Where(x =>
//         (x.ParentAddress.ParentAddress.ParentAddress.Id == add.OldAddressId)
//         || (x.ParentAddress.ParentAddress.Id == add.OldAddressId)
//         || (x.ParentAddress.Id == add.OldAddressId)).OrderBy(x => x.CreatedAt);
// }
// else if (add.AdminLevel == 3)
// {

//     totalChilders = totalChilders.Where(x =>
//          (x.ParentAddress.ParentAddress.Id == add.OldAddressId)
//         || (x.ParentAddress.Id == add.OldAddressId)).OrderBy(x => x.CreatedAt);
// }
// else if (add.AdminLevel == 4)
// {
//     totalChilders = totalChilders.Where(x =>
//          (x.ParentAddress.Id == add.OldAddressId)).OrderBy(x => x.CreatedAt);
// }