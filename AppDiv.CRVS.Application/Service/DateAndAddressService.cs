using System;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Domain.Entities;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Service
{
    public class DateAndAddressService : IDateAndAddressService
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;
        private readonly ILogger<DateAndAddressService> _Ilogger;
        private readonly IReportRepostory _reportRepostory;
        public DateAndAddressService(IAddressLookupRepository AddresslookupRepository, ILogger<DateAndAddressService> Ilogger, IReportRepostory reportRepostory)
        {
            _AddresslookupRepository = AddresslookupRepository;
            _Ilogger = Ilogger;
            _reportRepostory=reportRepostory;
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



        public async Task<AddressResponseDTOE?> FormatedAddress(Guid? id)
        {
            if (id == null || id == Guid.Empty)
            {
                return new AddressResponseDTOE();
            }
            var Address=  _reportRepostory.ReturnAddressIds(id.ToString()).Result;
            JArray jsonObject = JArray.FromObject(Address);
            AddressResponseDTOE addressResponse = jsonObject.ToObject<List<AddressResponseDTOE>>().FirstOrDefault();
            var FormatAddress = new AddressResponseDTOE
            {
                Country = addressResponse?.Country,
                Region = addressResponse?.Region,
                Zone = addressResponse?.Zone,
                Woreda = addressResponse?.Woreda,
                Kebele = addressResponse?.Kebele,
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
  public async Task<AddressResponseDTOE>? FormatedAddressLoop(Guid? id)
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




   public (string?,string?) stringAddress(FormatedAddressDto? address)
        {  // address?.CountryOr, address?.RegionOr,
           // address?.CountryAm, address?.RegionAm,
           string addressAm;
           string addressOr;
           if(address==null){
            return ("","");
           }
           if(string.IsNullOrEmpty(address.RegionAm)&& string.IsNullOrEmpty(address.RegionOr)){

                addressAm =address?.CountryAm;

                addressOr =  address?.CountryOr;
           }else{

                addressAm = string.Join("/",address?.ZoneAm,address?.WoredaAm,address?.KebeleAm);

                addressOr = string.Join("/", address?.ZoneOr,address?.WoredaOr,address?.KebeleOr);
                addressAm = Regex.Replace(addressAm, "/+", "/").TrimEnd('/');

               addressOr = Regex.Replace(addressOr, "/+", "/").TrimEnd('/');;
           }


            
            return (addressAm,addressOr);
        }
         public FormatedAddressDto?  AddressfromProcedure(Guid? Id)
            { 

            var Address=  _reportRepostory.ReturnAddress(Id.ToString()).Result;
            JArray AddressjsonObject = JArray.FromObject(Address);
            FormatedAddressDto AddressResponse = AddressjsonObject.ToObject<List<FormatedAddressDto>>().FirstOrDefault();           
            bool iscityadmin=IsCityAdmin(Id);
            return AddressResponse;
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



      public Lookup? GetLookup(Guid? id)
        {
            if (id == null)
                return null;
            var lookup = _lookupRepository.GetSingle(id);
            return lookup;
        }
    }
}

