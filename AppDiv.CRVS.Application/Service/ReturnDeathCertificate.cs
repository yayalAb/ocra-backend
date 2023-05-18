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
            (string am, string or)? address = (death.Event?.EventOwener?.BirthAddressId == Guid.Empty
               || death.Event?.EventOwener?.BirthAddressId == null) ? null :
               _DateAndAddressService.addressFormat(death.Event.EventOwener.BirthAddressId);


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
                DeathDay = death.Event.EventDate.Month.ToString(),
                DeathYear = death.Event.EventDate.Month.ToString(),

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

            };
        }
    }
}