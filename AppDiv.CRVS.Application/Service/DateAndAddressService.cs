using System;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Service
{
    public class DateAndAddressService : IDateAndAddressService
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;
        public DateAndAddressService(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public (string, string) addressFormat(Guid? id)

        {
            var Address = _AddresslookupRepository.GetAll()
                                    .Include(a => a.ParentAddress)
                                   .Where(a => a.Id == id).FirstOrDefault();
            var addressStringAm =
            Address?.ParentAddress?.ParentAddress?.ParentAddress?.ParentAddress?.AddressName.Value<string>("am")
            + "," + Address?.ParentAddress?.ParentAddress?.ParentAddress?.AddressName.Value<string>("am")
            + "," + Address?.ParentAddress?.ParentAddress?.AddressName.Value<string>("am")
            + "," + Address?.ParentAddress?.AddressName.Value<string>("am") + ","
            + Address?.AddressName.Value<string>("am");

            var addressStringOr =
            Address?.ParentAddress?.ParentAddress?.ParentAddress?.ParentAddress?.AddressName.Value<string>("or")
            + "," + Address?.ParentAddress?.ParentAddress?.ParentAddress?.AddressName.Value<string>("or")
            + "," + Address?.ParentAddress?.ParentAddress?.AddressName.Value<string>("or")
            + "," + Address?.ParentAddress?.AddressName.Value<string>("or") + ","
            + Address?.AddressName.Value<string>("or");

            return (addressStringAm, addressStringOr);

        }


    }
}

