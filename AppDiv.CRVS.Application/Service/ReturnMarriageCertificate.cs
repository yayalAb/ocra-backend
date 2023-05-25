using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;

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
            (string am, string or)? address = (marriage.Event?.EventAddressId == Guid.Empty
               || marriage.Event?.EventAddressId == null) ? null :
               _DateAndAddressService.addressFormat(marriage.Event.EventAddressId);

            var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(marriage.Event.CreatedAt);

            (string[] am, string[] or) splitedAddress = _DateAndAddressService.SplitedAddress(address?.am, address?.or);
            return new MarriageCertificateDTO()
            {
                CertifcateId = marriage.Event.CertificateId,
                BirthCertificateGroomId = marriage.BirthCertificateGroomId,
                BirthCertificateBrideId = marriage.BirthCertificateBrideId,
                RegBookNo = marriage.Event.RegBookNo,
                // BrideBirthCertifcateId = marriage.BirthCertificateBrideId,
                BrideFirstNameAm = marriage.BrideInfo.FirstName?.Value<string>("am"),
                BrideMiddleNameAm = marriage.BrideInfo.MiddleName?.Value<string>("am"),
                BrideLastNameAm = marriage.BrideInfo.LastName?.Value<string>("am"),
                BrideFirstNameOr = marriage.BrideInfo.FirstName?.Value<string>("or"),
                BrideMiddleNameOr = marriage.BrideInfo.MiddleName?.Value<string>("or"),
                BrideLastNameOr = marriage.BrideInfo.LastName?.Value<string>("or"),

                BrideBirthMonthOr = new EthiopicDateTime(convertor.getSplitted(marriage.BrideInfo?.BirthDateEt).month, "or").month,
                BrideBirthMonthAm = new EthiopicDateTime(convertor.getSplitted(marriage.BrideInfo?.BirthDateEt).month, "Am").month,
                BrideBirthDay = convertor.getSplitted(marriage.BrideInfo?.BirthDateEt).day.ToString(),
                BrideBirthYear = convertor.getSplitted(marriage.BrideInfo?.BirthDateEt).year.ToString(),

                BrideNationalityOr = marriage.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                BrideNationalityAm = marriage.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                // GroomBirthCertifcateId = marriage.BirthCertificateGroomId,
                GroomFirstNameAm = marriage.Event.EventOwener?.FirstName?.Value<string>("am"),
                GroomMiddleNameAm = marriage.Event.EventOwener?.MiddleName?.Value<string>("am"),
                GroomLastNameAm = marriage.Event.EventOwener?.LastName?.Value<string>("am"),
                GroomFirstNameOr = marriage.Event.EventOwener?.FirstName?.Value<string>("or"),
                GroomMiddleNameOr = marriage.Event.EventOwener?.MiddleName?.Value<string>("or"),
                GroomLastNameOr = marriage.Event.EventOwener?.LastName?.Value<string>("or"),

                GroomBirthMonthOr = new EthiopicDateTime(convertor.getSplitted(marriage.Event?.EventOwener?.BirthDateEt).month, "or").month,
                GroomBirthMonthAm = new EthiopicDateTime(convertor.getSplitted(marriage.Event?.EventOwener?.BirthDateEt).month, "Am").month,
                GroomBirthDay = convertor.getSplitted(marriage.Event?.EventOwener?.BirthDateEt).day.ToString(),
                GroomBirthYear = convertor.getSplitted(marriage.Event?.EventOwener?.BirthDateEt).year.ToString(),

                GroomNationalityOr = marriage.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                GroomNationalityAm = marriage.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                MarriageMonthOr = new EthiopicDateTime(convertor.getSplitted(marriage.Event?.EventDateEt).month, "or").month,
                MarriageMonthAm = new EthiopicDateTime(convertor.getSplitted(marriage.Event?.EventDateEt).month, "Am").month,
                MarriageDay = convertor.getSplitted(marriage.Event?.EventDateEt).day.ToString(),
                MarriageYear = convertor.getSplitted(marriage.Event?.EventDateEt).year.ToString(),

                // BirthAddressAm = marriage.Event?.EventAddress?.Id.ToString(),
                MarriageAddressAm = address?.am,
                MarriageAddressOr = address?.or,

                EventRegisteredMonthOr = new EthiopicDateTime(convertor.getSplitted(marriage.Event.EventRegDateEt).month, "or").month,
                EventRegisteredMonthAm = new EthiopicDateTime(convertor.getSplitted(marriage.Event.EventRegDateEt).month, "am").month,
                EventRegisteredDay = convertor.getSplitted(marriage.Event.EventRegDateEt).day.ToString(),
                EventRegisteredYear = convertor.getSplitted(marriage.Event.EventRegDateEt).year.ToString(),

                GeneratedMonthOr = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "or").month,
                GeneratedMonthAm = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "am").month,
                GeneratedDay = convertor.getSplitted(CreatedAtEt).day.ToString(),
                GeneratedYear = convertor.getSplitted(CreatedAtEt).year.ToString(),

                CivileRegOfficerFullNameOr = marriage.Event.CivilRegOfficer?.FirstName?.Value<string>("or") + " "
                                           + marriage.Event.CivilRegOfficer?.MiddleName?.Value<string>("or") + " "
                                           + marriage.Event.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = marriage.Event.CivilRegOfficer?.FirstName?.Value<string>("am") + " "
                                           + marriage.Event.CivilRegOfficer?.MiddleName?.Value<string>("am") + " "
                                           + marriage.Event.CivilRegOfficer?.LastName?.Value<string>("am"),

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