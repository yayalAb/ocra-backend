using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Service
{
    public class ReturnAdoptionCertfcate : IReturnAdoptionCertfcate
    {
        IDateAndAddressService _DateAndAddressService;
        public ReturnAdoptionCertfcate(IDateAndAddressService DateAndAddressService)
        {
            _DateAndAddressService = DateAndAddressService;
        }
        AdoptionCertificateDTO IReturnAdoptionCertfcate.GetAdoptionCertificate(AdoptionEvent adoption, string? BirthCertNo)
        {
            (string am, string or)? address = (adoption.Event?.EventOwener?.BirthAddressId == Guid.Empty
               || adoption.Event?.EventOwener?.BirthAddressId == null) ? null :
               _DateAndAddressService.addressFormat(adoption.Event.EventOwener.BirthAddressId);

            (string[] am, string[] or) splitedAddress = _DateAndAddressService.SplitedAddress(address?.am, address?.or);
            return new AdoptionCertificateDTO()
            {
                CertifcateId = adoption.Event.CertificateId,
                RegBookNo = adoption.Event.RegBookNo,
                BirthCertifcateId = (adoption.BirthCertificateId == null || string.IsNullOrEmpty(adoption.BirthCertificateId)) ? BirthCertNo : adoption.BirthCertificateId,
                ChildFirstNameAm = adoption.Event.EventOwener?.FirstName?.Value<string>("am"),
                ChildMiddleNameAm = adoption.Event.EventOwener?.MiddleName?.Value<string>("am"),
                ChildLastNameAm = adoption.Event.EventOwener?.LastName?.Value<string>("am"),
                ChildFirstNameOr = adoption.Event.EventOwener?.FirstName?.Value<string>("or"),
                ChildMiddleNameOr = adoption.Event.EventOwener?.MiddleName?.Value<string>("or"),
                ChildLastNameOr = adoption.Event.EventOwener?.LastName?.Value<string>("or"),
                GenderAm = adoption.Event?.EventOwener?.SexLookup?.Value?.Value<string>("am"),
                GenderOr = adoption.Event?.EventOwener?.SexLookup?.Value?.Value<string>("or"),

                BirthMonth = adoption.Event.EventDate.Month.ToString(),
                BirthDay = adoption.Event.EventDate.Day.ToString(),
                BirthYear = adoption.Event.EventDate.Year.ToString(),
                BirthAddressAm = address?.am,
                BirthAddressOr = address?.or,
                NationalityOr = adoption.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                NationalityAm = adoption.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                MotherFullNameOr = adoption.AdoptiveMother?.FirstName?.Value<string>("or")
                    + " " + adoption.AdoptiveMother?.MiddleName?.Value<string>("or") + " " + adoption.AdoptiveMother?.LastName?.Value<string>("or"),
                MotherFullNameAm = adoption.AdoptiveMother?.FirstName?.Value<string>("am")
                    + " " + adoption.AdoptiveMother?.MiddleName?.Value<string>("am") + " " + adoption.AdoptiveMother?.LastName?.Value<string>("am"),
                MotherNationalityOr = adoption.AdoptiveMother?.NationalityLookup?.Value?.Value<string>("or"),
                MotherNationalityAm = adoption.AdoptiveMother?.NationalityLookup?.Value?.Value<string>("am"),

                FatherFullNameOr = adoption.AdoptiveFather?.FirstName?.Value<string>("or")
                    + " " + adoption.AdoptiveFather?.MiddleName?.Value<string>("or") + " " + adoption.AdoptiveFather?.LastName?.Value<string>("or"),
                FatherFullNameAm = adoption.AdoptiveFather?.FirstName?.Value<string>("am")
                    + " " + adoption.AdoptiveFather?.MiddleName?.Value<string>("am") + " " + adoption.AdoptiveFather?.LastName?.Value<string>("am"),
                FatherNationalityOr = adoption.AdoptiveFather?.NationalityLookup?.Value?.Value<string>("or"),
                FatherNationalityAm = adoption.AdoptiveFather?.NationalityLookup?.Value?.Value<string>("am"),
                EventRegisteredMonth = adoption.Event.EventRegDate.Month.ToString(),
                EventRegisteredDay = adoption.Event.EventRegDate.Day.ToString(),
                EventRegisteredYear = adoption.Event.EventRegDate.Year.ToString(),
                GeneratedMonth = adoption.Event.CreatedAt.Month.ToString(),
                GeneratedDay = adoption.Event.CreatedAt.Day.ToString(),
                GeneratedYear = adoption.Event.CreatedAt.Year.ToString(),
                CivileRegOfficerFullNameOr = adoption.Event.CivilRegOfficer?.FirstName?.Value<string>("or")
                    + " " + adoption.Event.CivilRegOfficer?.MiddleName?.Value<string>("or") + " " + adoption.Event.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = adoption.Event.CivilRegOfficer?.FirstName?.Value<string>("am")
                    + " " + adoption.Event.CivilRegOfficer?.MiddleName?.Value<string>("am") + " " + adoption.Event.CivilRegOfficer?.LastName?.Value<string>("am"),


                //splited address

                CountryOr = splitedAddress.or?.ElementAtOrDefault(0),
                CountryAm = splitedAddress.am?.ElementAtOrDefault(0),
                RegionOr = splitedAddress.or?.ElementAtOrDefault(1),
                RegionAm = splitedAddress.am?.ElementAtOrDefault(1),
                ZoneOr = splitedAddress.or?.ElementAtOrDefault(2),
                ZoneAm = splitedAddress.am?.ElementAtOrDefault(2),
                WoredaOr = splitedAddress.or?.ElementAtOrDefault(3),
                WoredaAm = splitedAddress.am?.ElementAtOrDefault(3),
                CityOr = splitedAddress.or?.ElementAtOrDefault(4),
                CityAm = splitedAddress.am?.ElementAtOrDefault(4),
                KebeleOr = splitedAddress.or?.ElementAtOrDefault(5),
                KebeleAm = splitedAddress.am?.ElementAtOrDefault(5),

            };
        }
    }
}