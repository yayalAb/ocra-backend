using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Service
{
    public class ReturnBirthCertificate : IReturnBirthCertificate
    {
        IDateAndAddressService _DateAndAddressService;
        public ReturnBirthCertificate(IDateAndAddressService DateAndAddressService)
        {
            _DateAndAddressService = DateAndAddressService;
        }

        public BirthCertificateDTO GetBirthCertificate(BirthEvent birth, string? BirthCertNo)
        {
            (string am, string or)? address = (birth.Event?.EventAddressId == Guid.Empty
               || birth.Event?.EventAddressId == null) ? null :
               _DateAndAddressService.addressFormat(birth.Event.EventAddressId);


            return new BirthCertificateDTO()
            {
                CertifcateId = birth.Event.CertificateId,
                RegBookNo = birth.Event.RegBookNo,
                // BirthCertifcateId = birth.BirthCertificateId,
                ChildFirstNameAm = birth.Event.EventOwener?.FirstName?.Value<string>("am"),
                ChildMiddleNameAm = birth.Event.EventOwener?.MiddleName?.Value<string>("am"),
                ChildLastNameAm = birth.Event.EventOwener?.LastName?.Value<string>("am"),
                ChildFirstNameOr = birth.Event.EventOwener?.FirstName?.Value<string>("or"),
                ChildMiddleNameOr = birth.Event.EventOwener?.MiddleName?.Value<string>("or"),
                ChildLastNameOr = birth.Event.EventOwener?.LastName?.Value<string>("or"),

                GenderAm = birth.Event?.EventOwener?.SexLookup?.Value?.Value<string>("am"),
                GenderOr = birth.Event?.EventOwener?.SexLookup?.Value?.Value<string>("or"),

                BirthMonth = birth.Event.EventDate.Month.ToString(),
                BirthDay = birth.Event.EventDate.Month.ToString(),
                BirthYear = birth.Event.EventDate.Month.ToString(),

                // BirthAddressAm = birth.Event?.EventAddress?.Id.ToString(),
                BirthAddressAm = address?.am,
                BirthAddressOr = address?.or,
                NationalityOr = birth.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                NationalityAm = birth.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                MotherFullNameOr = birth.Mother?.FirstName?.Value<string>("or") + " "
                                 + birth.Mother?.MiddleName?.Value<string>("or") + " "
                                 + birth.Mother?.LastName?.Value<string>("or"),
                MotherFullNameAm = birth.Mother?.FirstName?.Value<string>("am") + " "
                                 + birth.Mother?.MiddleName?.Value<string>("am") + " "
                                 + birth.Mother?.LastName?.Value<string>("am"),
                MotherNationalityOr = birth.Mother?.NationalityLookup?.Value?.Value<string>("or"),
                MotherNationalityAm = birth.Mother?.NationalityLookup?.Value?.Value<string>("am"),

                FatherFullNameOr = birth.Father?.FirstName?.Value<string>("or") + " "
                                 + birth.Father?.MiddleName?.Value<string>("or") + " "
                                 + birth.Father?.LastName?.Value<string>("or"),
                FatherFullNameAm = birth.Father?.FirstName?.Value<string>("am") + " "
                                 + birth.Father?.MiddleName?.Value<string>("am") + " "
                                 + birth.Father?.LastName?.Value<string>("am"),
                FatherNationalityOr = birth.Father?.NationalityLookup?.Value?.Value<string>("or"),
                FatherNationalityAm = birth.Father?.NationalityLookup?.Value?.Value<string>("am"),

                EventRegisteredMonth = birth.Event.EventRegDate.Month.ToString(),
                EventRegisteredDay = birth.Event.EventRegDate.Day.ToString(),
                EventRegisteredYear = birth.Event.EventRegDate.Year.ToString(),

                GeneratedMonth = birth.Event.CreatedAt.Month.ToString(),
                GeneratedDay = birth.Event.CreatedAt.Day.ToString(),
                GeneratedYear = birth.Event.CreatedAt.Year.ToString(),

                CivileRegOfficerFullNameOr = birth.Event.CivilRegOfficer?.FirstName?.Value<string>("or") + " "
                                           + birth.Event.CivilRegOfficer?.MiddleName?.Value<string>("or") + " "
                                           + birth.Event.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = birth.Event.CivilRegOfficer?.FirstName?.Value<string>("am") + " "
                                           + birth.Event.CivilRegOfficer?.MiddleName?.Value<string>("am") + " "
                                           + birth.Event.CivilRegOfficer?.LastName?.Value<string>("am"),

            };
        }
    }
}