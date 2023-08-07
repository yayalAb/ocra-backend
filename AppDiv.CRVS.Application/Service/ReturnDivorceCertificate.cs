using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;

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
            (string am, string or)? address = (divorce.Event?.EventAddressId == Guid.Empty
               || divorce.Event?.EventAddressId == null) ? null :
               _DateAndAddressService.addressFormat(divorce.Event.EventAddressId);
            // wife birth address
            (string am, string or)? wifeBirthAddress = (divorce.DivorcedWife?.BirthAddressId == Guid.Empty
               || divorce.DivorcedWife?.BirthAddressId == null) ? null :
               _DateAndAddressService.addressFormat(divorce.DivorcedWife?.BirthAddressId);
            // husband birth address
            (string am, string or)? husbandBirthAddress = (divorce.Event.EventOwener?.BirthAddressId == Guid.Empty
               || divorce.Event?.EventOwener?.BirthAddressId == null) ? null :
               _DateAndAddressService.addressFormat(divorce.Event.EventOwener?.BirthAddressId);

            var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(divorce.Event.CreatedAt);

            (string[]? am, string[]? or)? splitedAddress = _DateAndAddressService.SplitedAddress(address?.am, address?.or);
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

                WifeBirthMonthOr = new EthiopicDateTime(convertor.getSplitted(divorce.DivorcedWife?.BirthDateEt).month, "or").month,
                WifeBirthMonthAm = new EthiopicDateTime(convertor.getSplitted(divorce.DivorcedWife?.BirthDateEt).month, "Am").month,
                WifeBirthDay = convertor.getSplitted(divorce.DivorcedWife?.BirthDateEt).day.ToString("D2"),
                WifeBirthYear = convertor.getSplitted(divorce.DivorcedWife?.BirthDateEt).year.ToString(),

                WifeBirthAddressAm = wifeBirthAddress?.am,
                WifeBirthAddressOr = wifeBirthAddress?.or,

                WifeNationalityOr = divorce.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                WifeNationalityAm = divorce.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                HusbandBirthCertifcateId = divorce.HusbandBirthCertificate,
                HusbandFirstNameAm = divorce.Event.EventOwener?.FirstName?.Value<string>("am"),
                HusbandMiddleNameAm = divorce.Event.EventOwener?.MiddleName?.Value<string>("am"),
                HusbandLastNameAm = divorce.Event.EventOwener?.LastName?.Value<string>("am"),
                HusbandFirstNameOr = divorce.Event.EventOwener?.FirstName?.Value<string>("or"),
                HusbandMiddleNameOr = divorce.Event.EventOwener?.MiddleName?.Value<string>("or"),
                HusbandLastNameOr = divorce.Event.EventOwener?.LastName?.Value<string>("or"),

                HusbandBirthMonthOr = new EthiopicDateTime(convertor.getSplitted(divorce.Event.EventOwener?.BirthDateEt).month, "or").month,
                HusbandBirthMonthAm = new EthiopicDateTime(convertor.getSplitted(divorce.Event.EventOwener?.BirthDateEt).month, "Am").month,
                HusbandBirthDay = convertor.getSplitted(divorce.Event.EventOwener?.BirthDateEt).day.ToString("D2"),
                HusbandBirthYear = convertor.getSplitted(divorce.Event.EventOwener?.BirthDateEt).year.ToString(),

                HusbandBirthAddressAm = husbandBirthAddress?.am,
                HusbandBirthAddressOr = husbandBirthAddress?.or,

                HusbandNationalityOr = divorce.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                HusbandNationalityAm = divorce.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                DivorceMonthOr = new EthiopicDateTime(convertor.getSplitted(divorce.Event?.EventDateEt).month, "or").month,
                DivorceMonthAm = new EthiopicDateTime(convertor.getSplitted(divorce.Event?.EventDateEt).month, "Am").month,
                DivorceDay = convertor.getSplitted(divorce.Event?.EventDateEt).day.ToString("D2"),
                DivorceYear = convertor.getSplitted(divorce.Event?.EventDateEt).year.ToString(),

                // BirthAddressAm = birth.Event?.EventAddress?.Id.ToString(),
                DivorceAddressAm = address?.am,
                DivorceAddressOr = address?.or,

                EventRegisteredMonthOr = new EthiopicDateTime(convertor.getSplitted(divorce.Event.EventRegDateEt).month, "or").month,
                EventRegisteredMonthAm = new EthiopicDateTime(convertor.getSplitted(divorce.Event.EventRegDateEt).month, "am").month,
                EventRegisteredDay = convertor.getSplitted(divorce.Event.EventRegDateEt).day.ToString("D2"),
                EventRegisteredYear = convertor.getSplitted(divorce.Event.EventRegDateEt).year.ToString(),

                GeneratedMonthOr = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "or").month,
                GeneratedMonthAm = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "am").month,
                GeneratedDay = convertor.getSplitted(CreatedAtEt).day.ToString("D2"),
                GeneratedYear = convertor.getSplitted(CreatedAtEt).year.ToString(),

                CivileRegOfficerFullNameOr = divorce.Event.CivilRegOfficer?.FirstName?.Value<string>("or") + " "
                                           + divorce.Event.CivilRegOfficer?.MiddleName?.Value<string>("or") + " "
                                           + divorce.Event.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = divorce.Event.CivilRegOfficer?.FirstName?.Value<string>("am") + " "
                                           + divorce.Event.CivilRegOfficer?.MiddleName?.Value<string>("am") + " "
                                           + divorce.Event.CivilRegOfficer?.LastName?.Value<string>("am"),

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
                KebeleOr = splitedAddress?.or?.ElementAtOrDefault(5),
                KebeleAm = splitedAddress?.am?.ElementAtOrDefault(5),

            };
        }
    }
}