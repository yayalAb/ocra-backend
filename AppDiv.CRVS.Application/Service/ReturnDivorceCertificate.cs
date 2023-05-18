using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Service
{
    public class ReturnDivorceCertificate : IReturnDivorceCertificate
    {
        IDateAndAddressService _DateAndAddressService;
        public ReturnDivorceCertificate(IDateAndAddressService DateAndAddressService)
        {
            _DateAndAddressService = DateAndAddressService;
        }

        public DivorceCertificateDTO GetDivorceCertificate(DivorceEvent divorce, string? BirthCertNo)
        {
            (string am, string or)? address = (divorce.Event?.EventOwener?.BirthAddressId == Guid.Empty
               || divorce.Event?.EventOwener?.BirthAddressId == null) ? null :
               _DateAndAddressService.addressFormat(divorce.Event.EventOwener.BirthAddressId);


            return new DivorceCertificateDTO()
            {
                CertifcateId = divorce.Event?.CertificateId,
                RegBookNo = divorce.Event?.RegBookNo,
                WifeBirthCertifcateId = divorce.WifeBirthCertificateId,
                WifeFirstNameAm = divorce.DivorcedWife.FirstName?.Value<string>("am"),
                WifeMiddleNameAm = divorce.DivorcedWife.MiddleName?.Value<string>("am"),
                WifeLastNameAm = divorce.DivorcedWife.LastName?.Value<string>("am"),
                WifeFirstNameOr = divorce.DivorcedWife.FirstName?.Value<string>("or"),
                WifeMiddleNameOr = divorce.DivorcedWife.MiddleName?.Value<string>("or"),
                WifeLastNameOr = divorce.DivorcedWife.LastName?.Value<string>("or"),

                WifeNationalityOr = divorce.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                WifeNationalityAm = divorce.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                HusbandBirthCertifcateId = divorce.HusbandBirthCertificate,
                HusbandFirstNameAm = divorce.Event.EventOwener?.FirstName?.Value<string>("am"),
                HusbandMiddleNameAm = divorce.Event.EventOwener?.MiddleName?.Value<string>("am"),
                HusbandLastNameAm = divorce.Event.EventOwener?.LastName?.Value<string>("am"),
                HusbandFirstNameOr = divorce.Event.EventOwener?.FirstName?.Value<string>("or"),
                HusbandMiddleNameOr = divorce.Event.EventOwener?.MiddleName?.Value<string>("or"),
                HusbandLastNameOr = divorce.Event.EventOwener?.LastName?.Value<string>("or"),

                HusbandNationalityOr = divorce.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                HusbandNationalityAm = divorce.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                DivorceMonth = divorce.Event.EventDate.Month.ToString(),
                DivorceDay = divorce.Event.EventDate.Day.ToString(),
                DivorceYear = divorce.Event.EventDate.Year.ToString(),

                // BirthAddressAm = birth.Event?.EventAddress?.Id.ToString(),
                DivorceAddressAm = address?.am,
                DivorceAddressOr = address?.or,

                EventRegisteredMonth = divorce.Event.EventRegDate.Month.ToString(),
                EventRegisteredDay = divorce.Event.EventRegDate.Day.ToString(),
                EventRegisteredYear = divorce.Event.EventRegDate.Year.ToString(),

                GeneratedMonth = divorce.Event.CreatedAt.Month.ToString(),
                GeneratedDay = divorce.Event.CreatedAt.Day.ToString(),
                GeneratedYear = divorce.Event.CreatedAt.Year.ToString(),

                CivileRegOfficerFullNameOr = divorce.Event.CivilRegOfficer?.FirstName?.Value<string>("or") + " "
                                           + divorce.Event.CivilRegOfficer?.MiddleName?.Value<string>("or") + " "
                                           + divorce.Event.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = divorce.Event.CivilRegOfficer?.FirstName?.Value<string>("am") + " "
                                           + divorce.Event.CivilRegOfficer?.MiddleName?.Value<string>("am") + " "
                                           + divorce.Event.CivilRegOfficer?.LastName?.Value<string>("am"),


            };
        }
    }
}