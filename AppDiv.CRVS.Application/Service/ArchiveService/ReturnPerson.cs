using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Newtonsoft.Json.Linq;
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public static class ReturnPerson
    {
        private static readonly CustomDateConverter convertor = new();
        public static EventInfoArchive GetEventInfo(Event? events, IDateAndAddressService dateAndAddressService,
        IReportRepostory _reportRepostory)
        {
            // (string am, string or)? address = (events?.EventRegisteredAddressId == Guid.Empty
            //    || events?.EventRegisteredAddressId == null) ? null :
            //    dateAndAddressService.addressFormat(events?.EventRegisteredAddressId);

            // (string[] am, string[] or)? splitedAddress = dateAndAddressService.SplitedAddress(address?.am, address?.or)!;
            // var convertor = new CustomDateConverter();
            var EventAddress=  _reportRepostory.ReturnAddress(events?.EventRegisteredAddressId.ToString()).Result;
            JArray EventAddressjsonObject = JArray.FromObject(EventAddress);
            FormatedAddressDto EventAddressResponse = EventAddressjsonObject.ToObject<List<FormatedAddressDto>>().FirstOrDefault();
             
            bool isCityAdmin=dateAndAddressService.IsCityAdmin(events?.EventRegisteredAddressId);
            return new EventInfoArchive
            {
                CertificateId = events?.CertificateId,
                RegistrationBookNumber = events?.RegBookNo,
                RegistrationOfficeId = events?.CertificateId?[..((events?.CertificateId?.Length ?? 0) - 8)],
                EventType = events?.EventType,
                EventMonthOr = new EthiopicDateTime(convertor.getSplitted(events?.EventDateEt!).month, "or")?.month,
                EventMonthAm = new EthiopicDateTime(convertor.getSplitted(events?.EventDateEt!).month, "Am")?.month,
                EventDay = convertor.getSplitted(events?.EventDateEt!).day.ToString("D2"),
                EventYear = convertor.getSplitted(events?.EventDateEt!).year.ToString(),

                RegistrationMonthOr = new EthiopicDateTime(convertor.getSplitted(events?.EventRegDateEt!).month, "or")?.month,
                RegistrationMonthAm = new EthiopicDateTime(convertor.getSplitted(events?.EventRegDateEt!).month, "Am")?.month,
                RegistrationDay = convertor.getSplitted(events?.EventRegDateEt!).day.ToString("D2"),
                RegistrationYear = convertor.getSplitted(events?.EventRegDateEt!).year.ToString(),
                RegistrationCountryOr = EventAddressResponse?.CountryOr,
                RegistrationCountryAm = EventAddressResponse?.CountryAm,
                RegistrationRegionOr = EventAddressResponse?.RegionOr,
                RegistrationRegionAm = EventAddressResponse?.RegionAm,
                RegistrationZoneOr =!isCityAdmin?EventAddressResponse?.ZoneOr:null,
                RegistrationZoneAm =!isCityAdmin? EventAddressResponse?.ZoneAm:null,
                RegistrationSubcityOr =isCityAdmin?EventAddressResponse?.WoredaOr:null,
                RegistrationSubcityAm =isCityAdmin? EventAddressResponse?.WoredaAm:null,
                RegistrationWoredaOr = EventAddressResponse?.WoredaOr,
                RegistrationWoredaAm = EventAddressResponse?.WoredaAm,
                RegistrationKebeleOr = EventAddressResponse?.KebeleOr,
                RegistrationKebeleAm = EventAddressResponse?.KebeleAm,
            };
        }
        public static  Person GetPerson(PersonalInfo? person, IDateAndAddressService dateAndAddressService //)
        , ILookupFromId lookupService, IReportRepostory _reportRepostory)
        {
            if(person==null){
                return null;
            }
             var personalInfo=  _reportRepostory.ReturnPerson(person.Id.ToString()).Result;
             JArray jsonObject = JArray.FromObject(personalInfo);
            PersonalInfoDtoPero personResponse = jsonObject.ToObject<List<PersonalInfoDtoPero>>().FirstOrDefault();

            var BirthAddress=  _reportRepostory.ReturnAddress(person.BirthAddressId.ToString()).Result;
             JArray BirthAddressjsonObject = JArray.FromObject(BirthAddress);
            FormatedAddressDto BirthAddressResponse = BirthAddressjsonObject.ToObject<List<FormatedAddressDto>>().FirstOrDefault();
            (string? am, string? or)? BirthStringAddress = dateAndAddressService.stringAddress(BirthAddressResponse);
            var ResidentAddress=  _reportRepostory.ReturnAddress(person.ResidentAddressId.ToString()).Result;
            JArray ResidentAddressjsonObject = JArray.FromObject(ResidentAddress);
            FormatedAddressDto ResidentAddressResponse = ResidentAddressjsonObject.ToObject<List<FormatedAddressDto>>().FirstOrDefault();           
           
            // (string am, string or)? birthAddress = (person?.BirthAddressId == Guid.Empty
            //    || person?.BirthAddress == null) ? null :
            //    dateAndAddressService.addressFormat(person.BirthAddressId);
            // (string[]? am, string[]? or)? birthSplitedAddress = dateAndAddressService.SplitedAddress(birthAddress?.am, birthAddress?.or);
            // (string am, string or)? residentAddress = (person?.ResidentAddressId == Guid.Empty
            //    || person?.ResidentAddress == null) ? null :
            //    dateAndAddressService.addressFormat(person.ResidentAddressId);
            
            (string? am, string? or)? residentSplitedAddress = dateAndAddressService.stringAddress(ResidentAddressResponse);
            
            bool BirthisCityAdmin=dateAndAddressService.IsCityAdmin(person.BirthAddressId);
            bool ResidentisCityAdmin=dateAndAddressService.IsCityAdmin(person.ResidentAddressId);

            
            var personInfo = new Person
            {
                FirstNameAm = personResponse?.FirstNameAm,
                MiddleNameAm = personResponse?.MiddleNameAm,
                LastNameAm = personResponse?.LastNameAm,

                FirstNameOr = personResponse?.FirstNameOr,
                MiddleNameOr = personResponse?.MiddleNameOr,
                LastNameOr = personResponse?.LastNameOr,

                GenderAm = personResponse?.GenderAm,
                GenderOr = personResponse?.GenderOr,

                NationalId = personResponse?.NationalId,

                BirthAddressAm = BirthStringAddress?.am,
                BirthAddressOr = BirthStringAddress?.or,

                BirthCountryOr = BirthAddressResponse?.CountryOr,
                BirthCountryAm = BirthAddressResponse?.CountryAm,
                BirthRegionOr = BirthAddressResponse?.RegionOr,
                BirthRegionAm = BirthAddressResponse?.RegionAm,
                BirthZoneOr =!BirthisCityAdmin?BirthAddressResponse?.ZoneOr:null,
                BirthZoneAm =!BirthisCityAdmin? BirthAddressResponse?.ZoneAm:null,
                BirthSubcityOr =BirthisCityAdmin?BirthAddressResponse?.WoredaOr:null,
                BirthSubcityAm =BirthisCityAdmin? BirthAddressResponse?.WoredaAm:null,
                BirthWoredaOr = BirthAddressResponse?.WoredaOr,
                BirthWoredaAm = BirthAddressResponse?.WoredaAm,
                BirthKebeleOr = BirthAddressResponse?.KebeleOr,
                BirthKebeleAm = BirthAddressResponse?.KebeleAm,

                ResidentCountryOr = ResidentAddressResponse?.CountryOr,
                ResidentCountryAm = ResidentAddressResponse?.CountryAm,
                ResidentRegionOr = ResidentAddressResponse?.RegionOr,
                ResidentRegionAm = ResidentAddressResponse?.RegionAm,
                ResidentZoneOr =!ResidentisCityAdmin?ResidentAddressResponse?.ZoneOr:null,
                ResidentZoneAm =!ResidentisCityAdmin? ResidentAddressResponse?.ZoneAm:null,
                ResidentSubcityOr =ResidentisCityAdmin?ResidentAddressResponse?.WoredaOr:null,
                ResidentSubcityAm =ResidentisCityAdmin? ResidentAddressResponse?.WoredaAm:null,
                ResidentWoredaOr = ResidentAddressResponse?.WoredaOr,
                ResidentWoredaAm = ResidentAddressResponse?.WoredaAm,
                ResidentKebeleOr = ResidentAddressResponse?.KebeleOr,
                ResidentKebeleAm = ResidentAddressResponse?.KebeleAm,

                ResidentAddressAm = residentSplitedAddress?.am,
                ResidentAddressOr = residentSplitedAddress?.or,

                NationalityOr = personResponse?.NationalityOr,
                NationalityAm = personResponse?.NationalityAm,

                MarriageStatusOr = personResponse?.MarriageStatusOr,
                MarriageStatusAm = personResponse?.MarriageStatusAm,

                ReligionOr = personResponse?.ReligionOr,
                ReligionAm = personResponse?.ReligionAm,

                NationOr =personResponse?.NationOr,
                NationAm = personResponse?.NationAm,

                EducationalStatusOr = personResponse?.EducationalStatusOr,
                EducationalStatusAm = personResponse?.EducationalStatusAm,

                TypeOfWorkOr =personResponse?.TypeOfWorkOr,
                TypeOfWorkAm = personResponse?.TypeOfWorkAm,


            };
            if (!string.IsNullOrEmpty(person?.BirthDateEt))
            {
                personInfo.BirthMonthOr = new EthiopicDateTime(convertor.getSplitted(person?.BirthDateEt!).month, "or")?.month;
                personInfo.BirthMonthAm = new EthiopicDateTime(convertor.getSplitted(person?.BirthDateEt!).month, "am")?.month;
                personInfo.BirthDay = convertor.getSplitted(person?.BirthDateEt!).day.ToString("D2");
                personInfo.BirthYear = convertor.getSplitted(person?.BirthDateEt!).year.ToString();
            }
            return personInfo;
        }
    }
}