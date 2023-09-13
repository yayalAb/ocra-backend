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
                EventMonthAm = new EthiopicDateTime(convertor.getSplitted(events?.EventDateEt!).month, "am")?.month,
                EventDay = convertor.getSplitted(events?.EventDateEt!).day.ToString("D2"),
                EventYear = convertor.getSplitted(events?.EventDateEt!).year.ToString(),

                RegistrationMonthOr = new EthiopicDateTime(convertor.getSplitted(events?.EventRegDateEt!).month, "or")?.month,
                RegistrationMonthAm = new EthiopicDateTime(convertor.getSplitted(events?.EventRegDateEt!).month, "am")?.month,
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
         public static  Person GetPersonBirthAddrress(Person? person,IReportRepostory _reportRepostory, Guid? BirthAddressId,IDateAndAddressService dateAndAddressService){
           
            var BirthAddress=  _reportRepostory.ReturnAddress(BirthAddressId.ToString()).Result;
            JArray BirthAddressjsonObject = JArray.FromObject(BirthAddress);
            FormatedAddressDto BirthAddressResponse = BirthAddressjsonObject.ToObject<List<FormatedAddressDto>>().FirstOrDefault();
           (string? am, string? or)? BirthStringAddress=("","");
            if(BirthAddressResponse!=null){
              BirthStringAddress = dateAndAddressService.stringAddress(BirthAddressResponse);
            }
            bool BirthisCityAdmin=dateAndAddressService.IsCityAdmin(BirthAddressId);
            person.BirthCountryOr = BirthAddressResponse?.CountryOr;
            person.BirthCountryAm = BirthAddressResponse?.CountryAm;
            person.BirthRegionOr = BirthAddressResponse?.RegionOr;
            person.BirthRegionAm = BirthAddressResponse?.RegionAm;
            person.BirthZoneOr =!BirthisCityAdmin?BirthAddressResponse?.ZoneOr:null;
            person.BirthZoneAm =!BirthisCityAdmin? BirthAddressResponse?.ZoneAm:null;
            person.BirthSubcityOr =BirthisCityAdmin?BirthAddressResponse?.WoredaOr:null;
            person.BirthSubcityAm =BirthisCityAdmin? BirthAddressResponse?.WoredaAm:null;
            person.BirthWoredaOr = BirthAddressResponse?.WoredaOr;
            person.BirthWoredaAm = BirthAddressResponse?.WoredaAm;
            person.BirthKebeleOr = BirthAddressResponse?.KebeleOr;
            person.BirthKebeleAm = BirthAddressResponse?.KebeleAm;
            person.BirthAddressAm =BirthStringAddress?.am;
            person.BirthAddressOr =BirthStringAddress?.or;
            return person;
         }
          public static  Person GetPersonResidentAddrress(Person? person,IReportRepostory _reportRepostory, Guid? ResidentAddressId,IDateAndAddressService dateAndAddressService){
            var ResidentAddress=  _reportRepostory.ReturnAddress(ResidentAddressId.ToString()).Result;
            JArray ResidentAddressjsonObject = JArray.FromObject(ResidentAddress);
            FormatedAddressDto ResidentAddressResponse = ResidentAddressjsonObject.ToObject<List<FormatedAddressDto>>().FirstOrDefault();           
            (string? am, string? or)? residentSplitedAddress = dateAndAddressService.stringAddress(ResidentAddressResponse);
            
            bool ResidentisCityAdmin=dateAndAddressService.IsCityAdmin(ResidentAddressId);
                person.ResidentCountryOr = ResidentAddressResponse?.CountryOr;
                person.ResidentCountryAm = ResidentAddressResponse?.CountryAm;
                person.ResidentRegionOr = ResidentAddressResponse?.RegionOr;
                person.ResidentRegionAm = ResidentAddressResponse?.RegionAm;
                person.ResidentZoneOr =!ResidentisCityAdmin?ResidentAddressResponse?.ZoneOr:null;
                person.ResidentZoneAm =!ResidentisCityAdmin? ResidentAddressResponse?.ZoneAm:null;
                person.ResidentSubcityOr =ResidentisCityAdmin?ResidentAddressResponse?.WoredaOr:null;
                person.ResidentSubcityAm =ResidentisCityAdmin? ResidentAddressResponse?.WoredaAm:null;
                person.ResidentWoredaOr = ResidentAddressResponse?.WoredaOr;
                person.ResidentWoredaAm = ResidentAddressResponse?.WoredaAm;
                person.ResidentKebeleOr = ResidentAddressResponse?.KebeleOr;
                person.ResidentKebeleAm = ResidentAddressResponse?.KebeleAm;

                person.ResidentAddressAm = residentSplitedAddress?.am;
                person.ResidentAddressOr = residentSplitedAddress?.or;
            return person;
         }


            public static  Person GetCorrectionRequestPerson(Person? person,PersonalInfo? personInfo){
                person.FirstNameAm =personInfo?.FirstName?.Value<string>("am");
                person.MiddleNameAm =personInfo?.MiddleName?.Value<string>("am");
                person.LastNameAm = personInfo?.LastName?.Value<string>("am");

                person.FirstNameOr = personInfo?.FirstName?.Value<string>("or");
                person.MiddleNameOr = personInfo?.MiddleName?.Value<string>("or");
                person.LastNameOr = personInfo?.LastName?.Value<string>("or");

                person.GenderAm = personInfo?.SexLookup?.Value?.Value<string>("am");
                person.GenderOr = personInfo?.SexLookup?.Value?.Value<string>("or");

                person.NationalId = personInfo?.NationalId;
                person.NationalityOr = personInfo?.NationalityLookup?.Value?.Value<string>("or");
                person.NationalityAm = personInfo?.NationalityLookup?.Value?.Value<string>("am");

                person.MarriageStatusOr = personInfo?.MarraigeStatusLookup?.Value?.Value<string>("or");
                person.MarriageStatusAm = personInfo?.MarraigeStatusLookup?.Value?.Value<string>("am");

                person.ReligionOr =  personInfo?.ReligionLookup?.Value?.Value<string>("or");
                person.ReligionAm =  personInfo?.ReligionLookup?.Value?.Value<string>("am");

                person.NationOr = personInfo?.NationLookup?.Value?.Value<string>("or");
                person.NationAm =  personInfo?.NationLookup?.Value?.Value<string>("am");

                person.EducationalStatusOr = personInfo?.EducationalStatusLookup?.Value?.Value<string>("or");
                person.EducationalStatusAm = personInfo?.EducationalStatusLookup?.Value?.Value<string>("am");

                person.TypeOfWorkOr =personInfo?.TypeOfWorkLookup?.Value?.Value<string>("or");
                person.TypeOfWorkAm =personInfo?.TypeOfWorkLookup?.Value?.Value<string>("am");
            return person;
         }
          public static  Person GetEventPerson(Person? person,PersonalInfo? personInfo,IReportRepostory _reportRepostory){
                if(string.IsNullOrEmpty(personInfo.Id.ToString())){
                    return new Person();
                }
                var personalInfo=  _reportRepostory.ReturnPerson(personInfo.Id.ToString()).Result;
                JArray jsonObject = JArray.FromObject(personalInfo);
                PersonalInfoDtoPero personResponse = jsonObject.ToObject<List<PersonalInfoDtoPero>>().FirstOrDefault();
                person.FirstNameAm = personResponse?.FirstNameAm;
                person.MiddleNameAm = personResponse?.MiddleNameAm;
                person.LastNameAm = personResponse?.LastNameAm;

                person.FirstNameOr = personResponse?.FirstNameOr;
                person.MiddleNameOr = personResponse?.MiddleNameOr;
                person.LastNameOr = personResponse?.LastNameOr;

                person.GenderAm = personResponse?.GenderAm;
                person.GenderOr = personResponse?.GenderOr;

                person.NationalId = personResponse?.NationalId;
                person.NationalityOr = personResponse?.NationalityOr;
                person.NationalityAm = personResponse?.NationalityAm;

                person.MarriageStatusOr = personResponse?.MarriageStatusOr;
                person.MarriageStatusAm = personResponse?.MarriageStatusAm;

                person.ReligionOr =  personResponse?.ReligionOr;
                person.ReligionAm =  personResponse?.ReligionAm;

                person.NationOr = personResponse?.NationOr;
                person.NationAm =  personResponse?.NationAm;

                person.EducationalStatusOr = personResponse?.EducationalStatusOr;
                person.EducationalStatusAm = personResponse?.EducationalStatusAm;

                person.TypeOfWorkOr =personResponse?.TypeOfWorkOr;
                person.TypeOfWorkAm = personResponse?.TypeOfWorkAm;
            return person;
         }

        
        public static  Person GetPerson(PersonalInfo? person, IDateAndAddressService dateAndAddressService //)
        , ILookupFromId lookupService, IReportRepostory _reportRepostory, bool IsCorrection=false)
        {
            if(person==null){
                return null;
            }
            var personInfo = new Person();
            if(IsCorrection){
             personInfo=GetCorrectionRequestPerson(personInfo,person);
            }
            else{
                personInfo=GetEventPerson(personInfo,person , _reportRepostory);
            }
            
            personInfo=GetPersonBirthAddrress(personInfo, _reportRepostory, person?.BirthAddressId,dateAndAddressService);
            personInfo=GetPersonResidentAddrress(personInfo, _reportRepostory, person?.ResidentAddressId,dateAndAddressService);
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