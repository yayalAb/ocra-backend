using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public static class ReturnPerson
    {
        private static CustomDateConverter convertor = new CustomDateConverter();
        public static EventInfoArchive GetEventInfo(Event events, IDateAndAddressService dateAndAddressService)
        {
            (string am, string or)? address = (events.EventAddressId == Guid.Empty
               || events?.EventAddressId == null) ? null :
               dateAndAddressService.addressFormat(events.EventAddressId);

            (string[] am, string[] or)? splitedAddress = dateAndAddressService.SplitedAddress(address?.am, address?.or);
            // var convertor = new CustomDateConverter();
            return new EventInfoArchive
            {
                CertificateId = events?.CertificateId,
                RegistrationBookNumber = events?.RegBookNo,
                RegistrationOfficeId = events?.CertificateId.Substring(0, events.CertificateId.Length - 8),

                EventMonthOr = new EthiopicDateTime(convertor.getSplitted(events?.EventDateEt).month, "or")?.month,
                EventMonthAm = new EthiopicDateTime(convertor.getSplitted(events?.EventDateEt).month, "Am")?.month,
                EventDay = convertor.getSplitted(events?.EventDateEt).day.ToString(),
                EventYear = convertor.getSplitted(events?.EventDateEt).year.ToString(),

                RegistrationMonthOr = new EthiopicDateTime(convertor.getSplitted(events?.EventRegDateEt).month, "or")?.month,
                RegistrationMonthAm = new EthiopicDateTime(convertor.getSplitted(events?.EventRegDateEt).month, "Am")?.month,
                RegistrationDay = convertor.getSplitted(events?.EventRegDateEt).day.ToString(),
                RegistrationYear = convertor.getSplitted(events?.EventRegDateEt).year.ToString(),

                EventCountryOr = splitedAddress?.or?.ElementAtOrDefault(0),
                EventCountryAm = splitedAddress?.am?.ElementAtOrDefault(0),
                EventRegionOr = splitedAddress?.or?.ElementAtOrDefault(1),
                EventRegionAm = splitedAddress?.am?.ElementAtOrDefault(1),
                EventZoneOr = splitedAddress?.or?.ElementAtOrDefault(2),
                EventZoneAm = splitedAddress?.am?.ElementAtOrDefault(2),
                EventWoredaOr = splitedAddress?.or?.ElementAtOrDefault(3),
                EventWoredaAm = splitedAddress?.am?.ElementAtOrDefault(3),
                EventCityKetemaOr = splitedAddress?.or?.ElementAtOrDefault(4),
                EventCityKetemaAm = splitedAddress?.am?.ElementAtOrDefault(4),
                EventKebeleOr = splitedAddress?.or?.ElementAtOrDefault(5),
                EventKebeleAm = splitedAddress?.am?.ElementAtOrDefault(5),


            };
        }
        public static Person GetPerson(PersonalInfo? person, IDateAndAddressService dateAndAddressService //)
        , ILookupFromId lookupService)
        {
            // return Fill.Filler<Person, PersonalInfo>(new Person(), person);
            // var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(DateTime.Now);
            (string am, string or)? birthAddress = (person?.BirthAddressId == Guid.Empty
               || person?.BirthAddress == null) ? null :
               dateAndAddressService.addressFormat(person.BirthAddressId);
            (string[]? am, string[]? or)? birthSplitedAddress = dateAndAddressService.SplitedAddress(birthAddress?.am, birthAddress?.or);

            (string am, string or)? residentAddress = (person?.ResidentAddressId == Guid.Empty
               || person?.ResidentAddress == null) ? null :
               dateAndAddressService.addressFormat(person.ResidentAddressId);
            (string[]? am, string[]? or)? residentSplitedAddress = dateAndAddressService.SplitedAddress(residentAddress?.am, residentAddress?.or);

            var personInfo = new Person
            {
                FirstNameAm = person?.FirstName?.Value<string>("am"),
                MiddleNameAm = person?.MiddleName?.Value<string>("am"),
                LastNameAm = person?.LastName?.Value<string>("am"),

                FirstNameOr = person?.FirstName?.Value<string>("or"),
                MiddleNameOr = person?.MiddleName?.Value<string>("or"),
                LastNameOr = person?.LastName?.Value<string>("or"),

                GenderAm = person?.SexLookup?.Value?.Value<string>("am"),
                GenderOr = person?.SexLookup?.Value?.Value<string>("or"),

                NationalId = person?.NationalId,

                BirthAddressAm = birthAddress?.am,
                BirthAddressOr = birthAddress?.or,

                BirthCountryOr = birthSplitedAddress?.or?.ElementAtOrDefault(0),
                BirthCountryAm = birthSplitedAddress?.am?.ElementAtOrDefault(0),
                BirthRegionOr = birthSplitedAddress?.or?.ElementAtOrDefault(1),
                BirthRegionAm = birthSplitedAddress?.am?.ElementAtOrDefault(1),
                BirthZoneOr = birthSplitedAddress?.or?.ElementAtOrDefault(2),
                BirthZoneAm = birthSplitedAddress?.am?.ElementAtOrDefault(2),
                BirthWoredaOr = birthSplitedAddress?.or?.ElementAtOrDefault(3),
                BirthWoredaAm = birthSplitedAddress?.am?.ElementAtOrDefault(3),
                BirthCityKetemaOr = birthSplitedAddress?.or?.ElementAtOrDefault(4),
                BirthCityKetemaAm = birthSplitedAddress?.am?.ElementAtOrDefault(4),
                BirthKebeleOr = birthSplitedAddress?.or?.ElementAtOrDefault(5),
                BirthKebeleAm = birthSplitedAddress?.am?.ElementAtOrDefault(5),

                ResidentAddressAm = residentAddress?.am,
                ResidentAddressOr = residentAddress?.or,

                ResidentCountryOr = residentSplitedAddress?.or?.ElementAtOrDefault(0),
                ResidentCountryAm = residentSplitedAddress?.am?.ElementAtOrDefault(0),
                ResidentRegionOr = residentSplitedAddress?.or?.ElementAtOrDefault(1),
                ResidentRegionAm = residentSplitedAddress?.am?.ElementAtOrDefault(1),
                ResidentZoneOr = residentSplitedAddress?.or?.ElementAtOrDefault(2),
                ResidentZoneAm = residentSplitedAddress?.am?.ElementAtOrDefault(2),
                ResidentWoredaOr = residentSplitedAddress?.or?.ElementAtOrDefault(3),
                ResidentWoredaAm = residentSplitedAddress?.am?.ElementAtOrDefault(3),
                ResidentCityKetemaOr = residentSplitedAddress?.or?.ElementAtOrDefault(4),
                ResidentCityKetemaAm = residentSplitedAddress?.am?.ElementAtOrDefault(4),
                ResidentKebeleOr = residentSplitedAddress?.or?.ElementAtOrDefault(5),
                ResidentKebeleAm = residentSplitedAddress?.am?.ElementAtOrDefault(5),

                NationalityOr = person?.NationalityLookup?.Value?.Value<string>("or") ?? lookupService.GetLookupOr(person?.NationalityLookupId),
                NationalityAm = person?.NationalityLookup?.Value?.Value<string>("am") ?? lookupService.GetLookupAm(person?.NationalityLookupId),

                MarriageStatusOr = person?.MarraigeStatusLookup?.Value?.Value<string>("or") ?? lookupService.GetLookupOr(person?.MarriageStatusLookupId),
                MarriageStatusAm = person?.MarraigeStatusLookup?.Value?.Value<string>("am") ?? lookupService.GetLookupAm(person?.MarriageStatusLookupId),

                ReligionOr = person?.ReligionLookup?.Value?.Value<string>("or") ?? lookupService.GetLookupOr(person?.ReligionLookupId),
                ReligionAm = person?.ReligionLookup?.Value?.Value<string>("am") ?? lookupService.GetLookupAm(person?.ReligionLookupId),

                NationOr = person?.NationLookup?.Value?.Value<string>("or") ?? lookupService.GetLookupOr(person?.NationLookupId),
                NationAm = person?.NationLookup?.Value?.Value<string>("am") ?? lookupService.GetLookupAm(person?.NationLookupId),

                EducationalStatusOr = person?.EducationalStatusLookup?.Value?.Value<string>("or") ?? lookupService.GetLookupOr(person?.EducationalStatusLookupId),
                EducationalStatusAm = person?.EducationalStatusLookup?.Value?.Value<string>("am") ?? lookupService.GetLookupAm(person?.EducationalStatusLookupId),

                TypeOfWorkOr = person?.TypeOfWorkLookup?.Value?.Value<string>("or") ?? lookupService.GetLookupOr(person?.TypeOfWorkLookupId),
                TypeOfWorkAm = person?.TypeOfWorkLookup?.Value?.Value<string>("am") ?? lookupService.GetLookupAm(person?.TypeOfWorkLookupId),


            };
            if (!string.IsNullOrEmpty(person?.BirthDateEt))
            {
                personInfo.BirthMonthOr = new EthiopicDateTime(convertor.getSplitted(person?.BirthDateEt).month, "or")?.month;
                personInfo.BirthMonthAm = new EthiopicDateTime(convertor.getSplitted(person?.BirthDateEt).month, "am")?.month;
                personInfo.BirthDay = convertor.getSplitted(person?.BirthDateEt).day.ToString();
                personInfo.BirthYear = convertor.getSplitted(person?.BirthDateEt).year.ToString();
            }
            return personInfo;
        }
    }
}