using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;

namespace AppDiv.CRVS.Application.Service
{
    public class MergeAndSplitAddressService : IMergeAndSplitAddressService
    {
        private readonly IAddressLookupRepository _AddressRepository;
        public MergeAndSplitAddressService(IAddressLookupRepository AddressRepository)
        {
            _AddressRepository = AddressRepository;

        }

        public string GetAdoptionCertificate(List<AddAddressRequest> Address)
        {

            return "Address Migrated Sucessfully";
        }
    }
}
