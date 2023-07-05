using Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Service
{
    public class MergeAndSplitAddressService : IMergeAndSplitAddressService
    {
        private readonly IAddressLookupRepository _AddressRepository;
        public MergeAndSplitAddressService(IAddressLookupRepository AddressRepository)
        {
            _AddressRepository = AddressRepository;

        }

        public async Task<string> MergeAndSplitAddress(List<AddAddressRequest> AddressList, CancellationToken cancellationToken)
        {
            foreach (var add in AddressList)
            {

                // var GetChildAddress = _AddressRepository.GetAll().Where(x => x.ParentAddressId == add.Id && !x.Status).ToList();
                // while (GetChildAddress.Count() > 0)
                // {
                //     GetChildAddress = _AddressRepository.GetAll().Where(x => x.ParentAddressId == add.Id && !x.Status).ToList();
                //     if (GetChildAddress.Count() == 0)
                //     {
                //         foreach (var add1 in AddressList)
                //         {
                //             Console.WriteLine("addresss {0}", add1);
                // var Address = new Address
                // {
                //     Id = Guid.NewGuid(),
                //     AddressName = add1.AddressName,
                //     StatisticCode = add1.StatisticCode,
                //     Code = add1.Code,
                //     AdminLevel = add1.AdminLevel,
                //     AreaTypeLookupId = add1.AreaTypeLookupId,
                //     ParentAddressId = add1.ParentAddressId,
                //     AdminTypeLookupId = add1.AdminTypeLookupId,
                //     OldAddressId = add1.Id
                // };
                // var oldAddress = _AddressRepository.GetAll().Where(x => x.Id == add1.Id).FirstOrDefault();
                // oldAddress.Status = true;
                // await _AddressRepository.UpdateAsync(oldAddress, x => x.Id);
                // await _AddressRepository.InsertAsync(Address, cancellationToken);
                //         }
                //     }
                // }
            }
            // var result = await _AddressRepository.SaveChangesAsync(cancellationToken);
            return "Address Migrated Sucessfully";
        }
    }
}
