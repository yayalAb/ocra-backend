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
        private readonly IAddressLookupRepository _addressRepository;
        public MergeAndSplitAddressService(IAddressLookupRepository addressRepository)
        {
            _addressRepository = addressRepository;

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
            await _addressRepository.UpdateAsync(updateAddress, x => x.Id);
            await _addressRepository.InsertAsync(address, cancellationToken);
            await _addressRepository.SaveChangesAsync(cancellationToken);
            List<ToUpdateList> ListOfAdress1 = new List<ToUpdateList>();
            foreach (var add in address)
            {
                if (add.AdminLevel != 5)
                {
                    var ChildAddress = _addressRepository.GetAll().Where(x => x.ParentAddressId == add.OldAddressId);
                    if (ChildAddress.FirstOrDefault() != null)
                    {
                        var pushToList = new ToUpdateList
                        {
                            AddressList = ChildAddress.ToList(),
                            ParentAddress = add
                        };
                        ListOfAdress1.Add(pushToList);
                    }
                }
            }
            if (ListOfAdress1?.FirstOrDefault()?.AddressList?.FirstOrDefault() != null)
            {
                await UpdateAddress(ListOfAdress1, cancellationToken);

            }
            await _addressRepository.SaveChangesAsync(cancellationToken);

            return new BaseResponse { Message = "Address Migrated Sucessfully" };
        }
        public async Task<bool> UpdateAddress(List<ToUpdateList> AddressList, CancellationToken cancellationToken)
        {
            List<ToUpdateList> ListOfAdress = new List<ToUpdateList>();
            List<Address> ListAddress = new List<Address>();

            foreach (var la in AddressList)
            {
                var NewAddressList = la.AddressList.Select(x => new Address
                {
                    Id = Guid.NewGuid(),
                    AddressName = x.AddressName,
                    StatisticCode = x.StatisticCode,
                    Code = la.ParentAddress.Code + x.CodePostfix,
                    AdminLevel = la.ParentAddress.AdminLevel + 1,
                    AreaTypeLookupId = x.AreaTypeLookupId,
                    ParentAddressId = la.ParentAddress.Id,
                    AdminTypeLookupId = x.AdminTypeLookupId,
                    OldAddressId = x.Id
                });
                var address1 = CustomMapper.Mapper.Map<List<Address>>(NewAddressList);
                var updateAddress1 = CustomMapper.Mapper.Map<List<Address>>(la.AddressList);
                updateAddress1.ForEach(x => x.Status = true);
                await _addressRepository.UpdateAsync(updateAddress1, x => x.Id);
                await _addressRepository.InsertAsync(address1, cancellationToken);
                ListAddress.AddRange(address1);
            }
            await _addressRepository.SaveChangesAsync(cancellationToken);

            foreach (var chil in ListAddress)
            {
                var ChildAddress = _addressRepository.GetAll().Where(x => x.ParentAddressId == chil.OldAddressId);
                if (ChildAddress?.FirstOrDefault() != null)
                {
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
