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
            var EventRegAddress=  _reportRepostory.ReturnAddress(events?.EventRegisteredAddressId.ToString()!).Result;
            JArray EventRegAddressjsonObject = JArray.FromObject(EventRegAddress);
            FormatedAddressDto EventRegAddressResponse = EventRegAddressjsonObject.ToObject<List<FormatedAddressDto>>()?.FirstOrDefault()!;

            var EventAddress=  _reportRepostory.ReturnAddress(events?.EventAddressId.ToString()!).Result;
            JArray eventAddressJArray = JArray.FromObject(EventAddress);
            FormatedAddressDto eventAddress = eventAddressJArray?.ToObject<List<FormatedAddressDto>>()?.FirstOrDefault()!;
             
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
                RegistrationCountryOr = EventRegAddressResponse?.CountryOr,
                RegistrationCountryAm = EventRegAddressResponse?.CountryAm,
                RegistrationRegionOr = EventRegAddressResponse?.RegionOr,
                RegistrationRegionAm = EventRegAddressResponse?.RegionAm,
                RegistrationZoneOr =!isCityAdmin?EventRegAddressResponse?.ZoneOr:null,
                RegistrationZoneAm =!isCityAdmin? EventRegAddressResponse?.ZoneAm:null,
                RegistrationSubcityOr =isCityAdmin?EventRegAddressResponse?.WoredaOr:null,
                RegistrationSubcityAm =isCityAdmin? EventRegAddressResponse?.WoredaAm:null,
                RegistrationWoredaOr = EventRegAddressResponse?.WoredaOr,
                RegistrationWoredaAm = EventRegAddressResponse?.WoredaAm,
                RegistrationKebeleOr = EventRegAddressResponse?.KebeleOr,
                RegistrationKebeleAm = EventRegAddressResponse?.KebeleAm,

                EventCountryOr = eventAddress?.CountryOr,
                EventCountryAm = eventAddress?.CountryAm,
                EventRegionOr = eventAddress?.RegionOr,
                EventRegionAm = eventAddress?.RegionAm,
                EventZoneOr =!isCityAdmin?eventAddress?.ZoneOr:null,
                EventZoneAm =!isCityAdmin? eventAddress?.ZoneAm:null,
                EventSubcityOr =isCityAdmin?eventAddress?.WoredaOr:null,
                EventSubcityAm =isCityAdmin? eventAddress?.WoredaAm:null,
                EventWoredaOr = eventAddress?.WoredaOr,
                EventWoredaAm = eventAddress?.WoredaAm,
                EventKebeleOr = eventAddress?.KebeleOr,
                EventKebeleAm = eventAddress?.KebeleAm,
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


            public static  Person GetCorrectionRequestPerson(Person? person,PersonalInfo? personInfo,ILookupFromId lookupService){
                var Gender=lookupService.GetLookup(personInfo?.SexLookupId);
                var Nationality=lookupService.GetLookup(personInfo?.NationalityLookupId);
                var Religion=lookupService.GetLookup(personInfo?.ReligionLookupId);
                var MarriageStatus=lookupService.GetLookup(personInfo?.MarriageStatusLookupId);
                var Nation=lookupService.GetLookup(personInfo?.NationLookupId);
                var EducationalStatus=lookupService.GetLookup(personInfo?.EducationalStatusLookupId);
                var TypeOfWork=lookupService.GetLookup(personInfo?.TypeOfWorkLookupId);
                person.FirstNameAm =personInfo?.FirstName?.Value<string>("am");
                person.MiddleNameAm =personInfo?.MiddleName?.Value<string>("am");
                person.LastNameAm = personInfo?.LastName?.Value<string>("am");

                person.FirstNameOr = personInfo?.FirstName?.Value<string>("or");
                person.MiddleNameOr = personInfo?.MiddleName?.Value<string>("or");
                person.LastNameOr = personInfo?.LastName?.Value<string>("or");

                person.GenderAm =Gender?.Value?.Value<string>("am");
                person.GenderOr =Gender?.Value?.Value<string>("or");

                person.NationalId = personInfo?.NationalId;
                person.NationalityOr = Nationality?.Value?.Value<string>("or");
                person.NationalityAm = Nationality?.Value?.Value<string>("am");

                person.MarriageStatusOr =MarriageStatus?.Value?.Value<string>("or");
                person.MarriageStatusAm =MarriageStatus?.Value?.Value<string>("am");

                person.ReligionOr = Religion?.Value?.Value<string>("or");
                person.ReligionAm = Religion?.Value?.Value<string>("am");

                person.NationOr =Nation?.Value?.Value<string>("or");
                person.NationAm =Nation?.Value?.Value<string>("am");

                person.EducationalStatusOr = EducationalStatus?.Value?.Value<string>("or");
                person.EducationalStatusAm = EducationalStatus?.Value?.Value<string>("am");

                person.TypeOfWorkOr =TypeOfWork?.Value?.Value<string>("or");
                person.TypeOfWorkAm =TypeOfWork?.Value?.Value<string>("am");
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
             personInfo=GetCorrectionRequestPerson(personInfo,person,lookupService);
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