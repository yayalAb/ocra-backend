using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

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

            (string[] am, string[] or) splitedAddress = _DateAndAddressService.SplitedAddress(address?.am, address?.or);
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

                GenderAm = death.Event?.EventOwener?.SexLookup?.Value?.Value<string>("am"),
                GenderOr = death.Event?.EventOwener?.SexLookup?.Value?.Value<string>("or"),

                BirthMonth = death.Event.EventDate.Month.ToString(),
                BirthDay = death.Event.EventDate.Day.ToString(),
                BirthYear = death.Event.EventDate.Year.ToString(),

                // BirthAddressAm = birth.Event?.EventAddress?.Id.ToString(),
                DeathPlaceAm = address?.am,
                DeathPlaceOr = address?.or,

                DeathMonth = death.Event.EventDate.Month.ToString(),
                DeathDay = death.Event.EventDate.Day.ToString(),
                DeathYear = death.Event.EventDate.Year.ToString(),

                NationalityOr = death.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                NationalityAm = death.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                EventRegisteredMonth = death.Event.EventRegDate.Month.ToString(),
                EventRegisteredDay = death.Event.EventRegDate.Day.ToString(),
                EventRegisteredYear = death.Event.EventRegDate.Year.ToString(),

                GeneratedMonth = death.Event.CreatedAt.Month.ToString(),
                GeneratedDay = death.Event.CreatedAt.Day.ToString(),
                GeneratedYear = death.Event.CreatedAt.Year.ToString(),

                CivileRegOfficerFullNameOr = death.Event.CivilRegOfficer?.FirstName?.Value<string>("or") + " "
                                           + death.Event.CivilRegOfficer?.MiddleName?.Value<string>("or") + " "
                                           + death.Event.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = death.Event.CivilRegOfficer?.FirstName?.Value<string>("am") + " "
                                           + death.Event.CivilRegOfficer?.MiddleName?.Value<string>("am") + " "
                                           + death.Event.CivilRegOfficer?.LastName?.Value<string>("am"),

                CountryOr = splitedAddress.or.ElementAtOrDefault(0),
                CountryAm = splitedAddress.am.ElementAtOrDefault(0),
                RegionOr = splitedAddress.or.ElementAtOrDefault(1),
                RegionAm = splitedAddress.am.ElementAtOrDefault(1),
                ZoneOr = splitedAddress.or.ElementAtOrDefault(2),
                ZoneAm = splitedAddress.am.ElementAtOrDefault(2),
                WoredaOr = splitedAddress.or.ElementAtOrDefault(3),
                WoredaAm = splitedAddress.am.ElementAtOrDefault(3),
                CityOr = splitedAddress.or.ElementAtOrDefault(4),
                CityAm = splitedAddress.am.ElementAtOrDefault(4),
                KebeleOr = splitedAddress.or.ElementAtOrDefault(5),
                KebeleAm = splitedAddress.am.ElementAtOrDefault(5),


            };
        }
    }
}