using System;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Application.Service
{
    public class DateAndAddressService : IDateAndAddressService
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;
        private readonly ILogger<DateAndAddressService> _Ilogger;
        public DateAndAddressService(IAddressLookupRepository AddresslookupRepository, ILogger<DateAndAddressService> Ilogger)
        {
            _AddresslookupRepository = AddresslookupRepository;
            _Ilogger = Ilogger;
        }
        public (string, string) addressFormat(Guid? id)

        {
            var Address = _AddresslookupRepository.GetAll()
                                    .Include(a => a.ParentAddress)
                                   .Where(a => a.Id == id).FirstOrDefault();
            _Ilogger.LogCritical(id.ToString());
            var addressStringAm =
            Address?.ParentAddress?.ParentAddress?.ParentAddress?.ParentAddress?.AddressName.Value<string>("am")
            + "," + Address?.ParentAddress?.ParentAddress?.ParentAddress?.AddressName.Value<string>("am")
            + "," + Address?.ParentAddress?.ParentAddress?.AddressName.Value<string>("am")
            + "," + Address?.ParentAddress?.AddressName.Value<string>("am") + ","
            + Address?.AddressName.Value<string>("am");

            var addressStringor =
            Address?.ParentAddress?.ParentAddress?.ParentAddress?.ParentAddress?.AddressName.Value<string>("or")
            + "," + Address?.ParentAddress?.ParentAddress?.ParentAddress?.AddressName.Value<string>("or")
            + "," + Address?.ParentAddress?.ParentAddress?.AddressName.Value<string>("or")
            + "," + Address?.ParentAddress?.AddressName.Value<string>("or") + ","
            + Address?.AddressName.Value<string>("or");

            return (addressStringAm, addressStringor);

        }
    }
}

