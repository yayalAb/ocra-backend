using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;

namespace AppDiv.CRVS.Application.Service
{
    public class ReturnDeathCertificate : IReturnDeathCertificate
    {
        IDateAndAddressService _DateAndAddressService;
        public ReturnDeathCertificate(IDateAndAddressService DateAndAddressService)
        {
            _DateAndAddressService = DateAndAddressService;
        }

        public DeathCertificateDTO GetDeathCertificate(DeathEvent death, string? BirthCertNo)
        {
            (string am, string or)? address = (death.Event?.EventAddressId == Guid.Empty
               || death.Event?.EventAddressId == null) ? null :
               _DateAndAddressService.addressFormat(death.Event.EventAddressId);

            var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(death.Event.CreatedAt);

            (string[]? am, string[]? or)? splitedAddress = _DateAndAddressService.SplitedAddress(address?.am, address?.or);
            return new DeathCertificateDTO()
            {
                CertifcateId = death.Event.CertificateId,
                RegBookNo = death.Event.RegBookNo,
                BirthCertifcateId = death.BirthCertificateId,
                FirstNameAm = death.Event.EventOwener?.FirstName?.Value<string>("am"),
                MiddleNameAm = death.Event.EventOwener?.MiddleName?.Value<string>("am"),
                LastNameAm = death.Event.EventOwener?.LastName?.Value<string>("am"),
                FirstNameOr = death.Event.EventOwener?.FirstName?.Value<string>("or"),
                MiddleNameOr = death.Event.EventOwener?.MiddleName?.Value<string>("or"),
                LastNameOr = death.Event.EventOwener?.LastName?.Value<string>("or"),

                TitleAm = death.Event?.EventOwener?.TitleLookup?.Value?.Value<string>("am"),
                TitleOr = death.Event?.EventOwener?.TitleLookup?.Value?.Value<string>("or"),

                GenderAm = death.Event?.EventOwener?.SexLookup?.Value?.Value<string>("am"),
                GenderOr = death.Event?.EventOwener?.SexLookup?.Value?.Value<string>("or"),

                BirthMonthOr = new EthiopicDateTime(convertor.getSplitted(death.Event.EventOwener.BirthDateEt).month, "or").month,
                BirthMonthAm = new EthiopicDateTime(convertor.getSplitted(death.Event.EventOwener.BirthDateEt).month, "Am").month,
                BirthDay = convertor.getSplitted(death.Event.EventOwener.BirthDateEt).day.ToString("D2"),
                BirthYear = convertor.getSplitted(death.Event.EventOwener.BirthDateEt).year.ToString(),

                // BirthAddressAm = death.Event?.EventAddress?.Id.ToString(),
                DeathPlaceAm = address?.am,
                DeathPlaceOr = address?.or,

                DeathMonthOr = new EthiopicDateTime(convertor.getSplitted(death.Event.EventDateEt).month, "or").month,
                DeathMonthAm = new EthiopicDateTime(convertor.getSplitted(death.Event.EventDateEt).month, "Am").month,
                DeathDay = convertor.getSplitted(death.Event.EventDateEt).day.ToString("D2"),
                DeathYear = convertor.getSplitted(death.Event.EventDateEt).year.ToString(),

                // DeathMonth = death.Event.EventDate.Month.ToString(),
                // DeathDay = death.Event.EventDate.day.ToString("D2"),
                // DeathYear = death.Event.EventDate.Year.ToString(),

                NationalityOr = death.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                NationalityAm = death.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                EventRegisteredMonthOr = new EthiopicDateTime(convertor.getSplitted(death.Event.EventRegDateEt).month, "or").month,
                EventRegisteredMonthAm = new EthiopicDateTime(convertor.getSplitted(death.Event.EventRegDateEt).month, "am").month,
                EventRegisteredDay = convertor.getSplitted(death.Event.EventRegDateEt).day.ToString("D2"),
                EventRegisteredYear = convertor.getSplitted(death.Event.EventRegDateEt).year.ToString(),

                GeneratedMonthOr = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "or").month,
                GeneratedMonthAm = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "am").month,
                GeneratedDay = convertor.getSplitted(CreatedAtEt).day.ToString("D2"),
                GeneratedYear = convertor.getSplitted(CreatedAtEt).year.ToString(),

                CivileRegOfficerFullNameOr = death.Event.CivilRegOfficer?.FirstName?.Value<string>("or") + " "
                                           + death.Event.CivilRegOfficer?.MiddleName?.Value<string>("or") + " "
                                           + death.Event.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = death.Event.CivilRegOfficer?.FirstName?.Value<string>("am") + " "
                                           + death.Event.CivilRegOfficer?.MiddleName?.Value<string>("am") + " "
                                           + death.Event.CivilRegOfficer?.LastName?.Value<string>("am"),

                CountryOr = splitedAddress?.or?.ElementAtOrDefault(0),
                CountryAm = splitedAddress?.am?.ElementAtOrDefault(0),
                RegionOr = splitedAddress?.or?.ElementAtOrDefault(1),
                RegionAm = splitedAddress?.am?.ElementAtOrDefault(1),
                ZoneOr = splitedAddress?.or?.ElementAtOrDefault(2),
                ZoneAm = splitedAddress?.am?.ElementAtOrDefault(2),
                WoredaOr = splitedAddress?.or?.ElementAtOrDefault(3),
                WoredaAm = splitedAddress?.am?.ElementAtOrDefault(3),
                CityOr = splitedAddress?.or?.ElementAtOrDefault(4),
                CityAm = splitedAddress?.am?.ElementAtOrDefault(4),
                KebeleOr = splitedAddress?.or?.ElementAtOrDefault(4),
                KebeleAm = splitedAddress?.am?.ElementAtOrDefault(4),


            };
        }
    }
}