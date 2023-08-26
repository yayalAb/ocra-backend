using System;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Domain.Entities;

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
                                   .Where(a => a.Id == id).FirstOrDefault();

            string addressStringAm = Address?.AddressName?.Value<string>("am");
            string addressStringOr = Address?.AddressName?.Value<string>("or");
            string adressStr = adressStr = Address.AddressNameStr;
            while (Address?.ParentAddressId != null)
            {

                Address = _AddresslookupRepository.GetAll()
                                    .Where(a => a.Id == Address.ParentAddressId).FirstOrDefault();
                addressStringAm = Address?.AddressName?.Value<string>("am") + "/" + addressStringAm;
                addressStringOr = Address?.AddressName?.Value<string>("or") + "/" + addressStringOr;
            }
            return (addressStringAm, addressStringOr);

        }



        public async Task<AddressResponseDTOE>? FormatedAddress(Guid? id)
        {
            if (id == null || id == Guid.Empty)
            {
                return null;
            }
            string addessSt = "";
            var Address = _AddresslookupRepository.GetAll()
                                   .Where(a => a.Id == id).FirstOrDefault();
            if (Address == null)
            {
                return null;
            }
            addessSt = Address?.Id.ToString();
            while (Address?.ParentAddressId != null)
            {
                Address = _AddresslookupRepository.GetAll()
                                    .Where(a => a.Id == Address.ParentAddressId).FirstOrDefault();
                addessSt = Address?.Id.ToString() + "/" + addessSt;

            };
            if (string.IsNullOrEmpty(addessSt))
            {
                return null;
            }
            string[] address = addessSt.Split("/");

            var FormatAddress = new AddressResponseDTOE
            {
                Country = address.ElementAtOrDefault(0),
                Region = address.ElementAtOrDefault(1),
                Zone = address.ElementAtOrDefault(2),
                Woreda = address.ElementAtOrDefault(3),
                Kebele = address.ElementAtOrDefault(4),
            };
            return FormatAddress;
        }

        public (string[]?, string[]?)? SplitedAddress(string? am, string? or)
        {
            string[]? addressAm = am?.Split("/");
            string[]? addressOr = or?.Split("/");
            return (addressAm, addressOr);
        }

        public string GetFullAddress(Address address)
        {
            string addressString = "";
            if (address != null)
            {
                addressString += address?.AddressNameLang;
                if (address?.ParentAddress != null)
                {
                    addressString = address?.ParentAddress?.AddressNameLang + "/" + addressString;
                    if (address.ParentAddress.ParentAddress != null)
                    {
                        addressString = address?.ParentAddress?.ParentAddress?.AddressNameLang + "/" + addressString;
                        if (address.ParentAddress?.ParentAddress?.ParentAddress != null)
                        {
                            addressString = address?.ParentAddress?.ParentAddress?.ParentAddress?.AddressNameLang + "/" + addressString;
                            if (address.ParentAddress?.ParentAddress?.ParentAddress?.ParentAddress != null)
                            {
                                addressString = address.ParentAddress?.ParentAddress?.ParentAddress?.ParentAddress?.AddressNameLang + "/" + addressString;
                            }
                        }
                    }
                }
            }
            return addressString.TrimEnd('/');
        }


        public string[] SplitedAddressByLang(Guid? id)
        {
            string addessSt = "";
            var Address = _AddresslookupRepository.GetAll()
                                   .Where(a => a.Id == id).FirstOrDefault();
            addessSt = Address?.AddressNameLang;
            while (Address?.ParentAddressId != null)
            {
                Address = _AddresslookupRepository.GetAll()
                                    .Where(a => a.Id == Address.ParentAddressId).FirstOrDefault();
                addessSt = Address?.AddressNameLang + "/" + addessSt;

            };
            string[] address = addessSt.Split("/");
            return address;
        }

        public bool IsCityAdmin(Guid? Id)
        {
            var Address = _AddresslookupRepository.GetAll()
                                   .Include(x=>x.ParentAddress)
                                   .ThenInclude(x=>x.ParentAddress)
                                   .ThenInclude(x=>x.ParentAddress)
                                   .ThenInclude(x=>x.ParentAddress)
                                   .Where(a => a.Id == Id).FirstOrDefault();
             if(Address?.ParentAddress?.ParentAddress?.ParentAddress?.ParentAddress==null){
                  return true;
             }                      
            return false;
        }
    }

    public class LookupFromId : ILookupFromId
    {
        private readonly ILookupRepository _lookupRepository;
        public LookupFromId(ILookupRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;
        }
        public bool CheckMatchLookup(Guid id, string key, string like)
        {
            return _lookupRepository.GetAll().Any(l => l.Id == id
                                                && EF.Functions.Like(l.ValueStr.ToLower(), $"%{like}%")
                                                && l.Key.ToLower() == key);
        }
        public string? GetLookupOr(Guid? id)
        {
            if (id == null)
                return null;
            var lookup = _lookupRepository.GetSingle(id);
            return lookup?.Value?.Value<string>("or");
        }
        public string? GetLookupAm(Guid? id)
        {
            if (id == null)
                return null;
            var lookup = _lookupRepository.GetSingle(id);
            return lookup?.Value?.Value<string>("am");
        }
    }
}

