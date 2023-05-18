using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Service
{
    public class ReturnMarriageCertificate : IReturnMarriageCertificate
    {
        IDateAndAddressService _DateAndAddressService;
        public ReturnMarriageCertificate(IDateAndAddressService DateAndAddressService)
        {
            _DateAndAddressService = DateAndAddressService;
        }

        public MarriageCertificateDTO GetMarriageCertificate(MarriageEvent marriage, string? BirthCertNo)
        {
            (string am, string or)? address = (marriage.Event?.EventOwener?.BirthAddressId == Guid.Empty
               || marriage.Event?.EventOwener?.BirthAddressId == null) ? null :
               _DateAndAddressService.addressFormat(marriage.Event.EventOwener.BirthAddressId);

            return new MarriageCertificateDTO()
            {
                CertifcateId = marriage.Event.CertificateId,
                RegBookNo = marriage.Event.RegBookNo,
                // BrideBirthCertifcateId = marriage.BirthCertificateBrideId,
                BrideFirstNameAm = marriage.BrideInfo.FirstName?.Value<string>("am"),
                BrideMiddleNameAm = marriage.BrideInfo.MiddleName?.Value<string>("am"),
                BrideLastNameAm = marriage.BrideInfo.LastName?.Value<string>("am"),
                BrideFirstNameOr = marriage.BrideInfo.FirstName?.Value<string>("or"),
                BrideMiddleNameOr = marriage.BrideInfo.MiddleName?.Value<string>("or"),
                BrideLastNameOr = marriage.BrideInfo.LastName?.Value<string>("or"),

                BrideNationalityOr = marriage.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                BrideNationalityAm = marriage.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                // GroomBirthCertifcateId = marriage.BirthCertificateGroomId,
                GroomFirstNameAm = marriage.Event.EventOwener?.FirstName?.Value<string>("am"),
                GroomMiddleNameAm = marriage.Event.EventOwener?.MiddleName?.Value<string>("am"),
                GroomLastNameAm = marriage.Event.EventOwener?.LastName?.Value<string>("am"),
                GroomFirstNameOr = marriage.Event.EventOwener?.FirstName?.Value<string>("or"),
                GroomMiddleNameOr = marriage.Event.EventOwener?.MiddleName?.Value<string>("or"),
                GroomLastNameOr = marriage.Event.EventOwener?.LastName?.Value<string>("or"),

                GroomNationalityOr = marriage.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                GroomNationalityAm = marriage.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                MarriageMonth = marriage.Event.EventDate.Month.ToString(),
                MarriageDay = marriage.Event.EventDate.Month.ToString(),
                MarriageYear = marriage.Event.EventDate.Month.ToString(),

                // BirthAddressAm = birth.Event?.EventAddress?.Id.ToString(),
                MarriageAddressAm = address?.am,
                MarriageAddressOr = address?.or,

                EventRegisteredMonth = marriage.Event.EventRegDate.Month.ToString(),
                EventRegisteredDay = marriage.Event.EventRegDate.Day.ToString(),
                EventRegisteredYear = marriage.Event.EventRegDate.Year.ToString(),

                GeneratedMonth = marriage.Event.CreatedAt.Month.ToString(),
                GeneratedDay = marriage.Event.CreatedAt.Day.ToString(),
                GeneratedYear = marriage.Event.CreatedAt.Year.ToString(),

                CivileRegOfficerFullNameOr = marriage.Event.CivilRegOfficer?.FirstName?.Value<string>("or") + " "
                                           + marriage.Event.CivilRegOfficer?.MiddleName?.Value<string>("or") + " "
                                           + marriage.Event.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = marriage.Event.CivilRegOfficer?.FirstName?.Value<string>("am") + " "
                                           + marriage.Event.CivilRegOfficer?.MiddleName?.Value<string>("am") + " "
                                           + marriage.Event.CivilRegOfficer?.LastName?.Value<string>("am"),


            };
        }
    }
}