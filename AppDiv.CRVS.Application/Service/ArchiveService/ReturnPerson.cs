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
        public static EventInfo GetEventInfo(Event events, IDateAndAddressService dateAndAddressService)
        {
            (string am, string or)? address = (events.EventAddressId == Guid.Empty
               || events.EventAddressId == null) ? null :
               dateAndAddressService.addressFormat(events.EventAddressId);

            (string[] am, string[] or) splitedAddress = dateAndAddressService.SplitedAddress(address?.am, address?.or);
            // var convertor = new CustomDateConverter();
            return new EventInfo
            {
                EventMonthOr = new EthiopicDateTime(convertor.getSplitted(events?.EventDateEt).month, "or")?.month,
                EventMonthAm = new EthiopicDateTime(convertor.getSplitted(events?.EventDateEt).month, "Am")?.month,
                EventDay = convertor.getSplitted(events?.EventDateEt).day.ToString(),
                EventYear = convertor.getSplitted(events?.EventDateEt).year.ToString(),

                RegistrationMonthOr = new EthiopicDateTime(convertor.getSplitted(events?.EventRegDateEt).month, "or")?.month,
                RegistrationMonthAm = new EthiopicDateTime(convertor.getSplitted(events?.EventRegDateEt).month, "Am")?.month,
                RegistrationDay = convertor.getSplitted(events?.EventRegDateEt).day.ToString(),
                RegistrationYear = convertor.getSplitted(events?.EventRegDateEt).year.ToString(),

                EventCountryOr = splitedAddress.or?.ElementAtOrDefault(0),
                EventCountryAm = splitedAddress.am?.ElementAtOrDefault(0),
                EventRegionOr = splitedAddress.or?.ElementAtOrDefault(1),
                EventRegionAm = splitedAddress.am?.ElementAtOrDefault(1),
                EventZoneOr = splitedAddress.or?.ElementAtOrDefault(2),
                EventZoneAm = splitedAddress.am?.ElementAtOrDefault(2),
                EventWoredaOr = splitedAddress.or?.ElementAtOrDefault(3),
                EventWoredaAm = splitedAddress.am?.ElementAtOrDefault(3),
                EventCityKetemaOr = splitedAddress.or?.ElementAtOrDefault(4),
                EventCityKetemaAm = splitedAddress.am?.ElementAtOrDefault(4),
                EventKebeleOr = splitedAddress.or?.ElementAtOrDefault(5),
                EventKebeleAm = splitedAddress.am?.ElementAtOrDefault(5),


            };
        }
        public static Person GetPerson(PersonalInfo person, IDateAndAddressService dateAndAddressService)
        {
            // return Fill.Filler<Person, PersonalInfo>(new Person(), person);
            // var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(DateTime.Now);

            (string am, string or)? birthAddress = (person?.BirthAddressId == Guid.Empty
               || person?.BirthAddressId == null) ? null :
               dateAndAddressService.addressFormat(person.BirthAddressId);
            return new Person
            {
                FirstNameAm = person?.FirstName?.Value<string>("am"),
                MiddleNameAm = person?.MiddleName?.Value<string>("am"),
                LastNameAm = person?.LastName?.Value<string>("am"),

                FirstNameOr = person?.FirstName?.Value<string>("or"),
                MiddleNameOr = person?.MiddleName?.Value<string>("or"),
                LastNameOr = person?.LastName?.Value<string>("or"),

                GenderAm = person?.SexLookup?.Value?.Value<string>("am"),
                GenderOr = person?.SexLookup?.Value?.Value<string>("or"),

                BirthMonthOr = new EthiopicDateTime(convertor.getSplitted(person?.BirthDateEt).month, "or")?.month,
                BirthMonthAm = new EthiopicDateTime(convertor.getSplitted(person?.BirthDateEt).month, "Am")?.month,
                BirthDay = convertor.getSplitted(person?.BirthDateEt).day.ToString(),
                BirthYear = convertor.getSplitted(person?.BirthDateEt).year.ToString(),

                BirthAddressAm = birthAddress?.am,
                BirthAddressOr = birthAddress?.or,

                NationalityOr = person?.NationalityLookup?.Value?.Value<string>("or"),
                NationalityAm = person?.NationalityLookup?.Value?.Value<string>("am"),

                MarriageStatusOr = person?.MarraigeStatusLookup?.Value?.Value<string>("or"),
                MarriageStatusAm = person?.MarraigeStatusLookup?.Value?.Value<string>("am"),

                ReligionOr = person?.ReligionLookup?.Value?.Value<string>("or"),
                ReligionAm = person?.ReligionLookup?.Value?.Value<string>("am"),

                NationOr = person?.NationLookup?.Value?.Value<string>("or"),
                NationAm = person?.NationLookup?.Value?.Value<string>("am"),

                EducationalStatusOr = person?.EducationalStatusLookup?.Value?.Value<string>("or"),
                EducationalStatusAm = person?.EducationalStatusLookup?.Value?.Value<string>("am"),

                TypeOfWorkOr = person?.TypeOfWorkLookup?.Value?.Value<string>("or"),
                TypeOfWorkAm = person?.TypeOfWorkLookup?.Value?.Value<string>("am"),


            };
        }
    }
}