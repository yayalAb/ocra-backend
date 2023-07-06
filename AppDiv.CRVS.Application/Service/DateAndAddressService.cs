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

        public async Task<AddressResponseDTOE> FormatedAddress(Guid? id)
        {
            string addessSt = "";
            var Address = _AddresslookupRepository.GetAll()
                                   .Where(a => a.Id == id).FirstOrDefault();
            addessSt = Address?.Id.ToString();
            while (Address?.ParentAddressId != null)
            {
                Address = _AddresslookupRepository.GetAll()
                                    .Where(a => a.Id == Address.ParentAddressId).FirstOrDefault();
                addessSt = Address?.Id.ToString() + "/" + addessSt;

            };
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
            var lookup = _lookupRepository.GetSingle(id);
            return lookup?.Value?.Value<string>("or");
        }
        public string? GetLookupAm(Guid? id)
        {
            var lookup = _lookupRepository.GetSingle(id);
            return lookup?.Value?.Value<string>("am");
        }



    }
}

