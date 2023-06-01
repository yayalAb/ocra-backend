using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;

namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public class ReturnDivorceArchive
    {
        IDateAndAddressService _DateAndAddressService;
        public ReturnDivorceArchive(IDateAndAddressService DateAndAddressService)
        {
            _DateAndAddressService = DateAndAddressService;
        }

        public DivorceArchiveDTO GetDivorceArchive(Event divorce, string? BirthCertNo)
        {
            (string am, string or)? address = (divorce?.EventAddressId == Guid.Empty
               || divorce?.EventAddressId == null) ? null :
               _DateAndAddressService.addressFormat(divorce.EventAddressId);
            // wife birth address
            (string am, string or)? wifeBirthAddress = (divorce.DivorceEvent.DivorcedWife?.BirthAddressId == Guid.Empty
               || divorce.DivorceEvent.DivorcedWife?.BirthAddressId == null) ? null :
               _DateAndAddressService.addressFormat(divorce.DivorceEvent.DivorcedWife?.BirthAddressId);
            // husband birth address
            (string am, string or)? husbandBirthAddress = (divorce.EventOwener?.BirthAddressId == Guid.Empty
               || divorce?.EventOwener?.BirthAddressId == null) ? null :
               _DateAndAddressService.addressFormat(divorce.EventOwener?.BirthAddressId);

            var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(divorce.CreatedAt);

            (string[] am, string[] or) splitedAddress = _DateAndAddressService.SplitedAddress(address?.am, address?.or);
            return new DivorceArchiveDTO()
            {
                CertifcateId = divorce?.CertificateId,
                RegBookNo = divorce?.RegBookNo,
                WifeBirthCertifcateId = divorce.DivorceEvent.WifeBirthCertificateId,
                WifeFirstNameAm = divorce.DivorceEvent.DivorcedWife.FirstName?.Value<string>("am"),
                WifeMiddleNameAm = divorce.DivorceEvent.DivorcedWife.MiddleName?.Value<string>("am"),
                WifeLastNameAm = divorce.DivorceEvent.DivorcedWife.LastName?.Value<string>("am"),
                WifeFirstNameOr = divorce.DivorceEvent.DivorcedWife.FirstName?.Value<string>("or"),
                WifeMiddleNameOr = divorce.DivorceEvent.DivorcedWife.MiddleName?.Value<string>("or"),
                WifeLastNameOr = divorce.DivorceEvent.DivorcedWife.LastName?.Value<string>("or"),

                WifeBirthMonthOr = new EthiopicDateTime(convertor.getSplitted(divorce.DivorceEvent.DivorcedWife?.BirthDateEt).month, "or").month,
                WifeBirthMonthAm = new EthiopicDateTime(convertor.getSplitted(divorce.DivorceEvent.DivorcedWife?.BirthDateEt).month, "Am").month,
                WifeBirthDay = convertor.getSplitted(divorce.DivorceEvent.DivorcedWife?.BirthDateEt).day.ToString(),
                WifeBirthYear = convertor.getSplitted(divorce.DivorceEvent.DivorcedWife?.BirthDateEt).year.ToString(),

                WifeBirthAddressAm = wifeBirthAddress?.am,
                WifeBirthAddressOr = wifeBirthAddress?.or,

                WifeNationalityOr = divorce?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                WifeNationalityAm = divorce?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                HusbandBirthCertifcateId = divorce.DivorceEvent.HusbandBirthCertificate,
                HusbandFirstNameAm = divorce.EventOwener?.FirstName?.Value<string>("am"),
                HusbandMiddleNameAm = divorce.EventOwener?.MiddleName?.Value<string>("am"),
                HusbandLastNameAm = divorce.EventOwener?.LastName?.Value<string>("am"),
                HusbandFirstNameOr = divorce.EventOwener?.FirstName?.Value<string>("or"),
                HusbandMiddleNameOr = divorce.EventOwener?.MiddleName?.Value<string>("or"),
                HusbandLastNameOr = divorce.EventOwener?.LastName?.Value<string>("or"),

                HusbandBirthMonthOr = new EthiopicDateTime(convertor.getSplitted(divorce.EventOwener?.BirthDateEt).month, "or").month,
                HusbandBirthMonthAm = new EthiopicDateTime(convertor.getSplitted(divorce.EventOwener?.BirthDateEt).month, "Am").month,
                HusbandBirthDay = convertor.getSplitted(divorce.EventOwener?.BirthDateEt).day.ToString(),
                HusbandBirthYear = convertor.getSplitted(divorce.EventOwener?.BirthDateEt).year.ToString(),

                HusbandBirthAddressAm = husbandBirthAddress?.am,
                HusbandBirthAddressOr = husbandBirthAddress?.or,

                HusbandNationalityOr = divorce?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                HusbandNationalityAm = divorce?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                DivorceMonthOr = new EthiopicDateTime(convertor.getSplitted(divorce?.EventDateEt).month, "or").month,
                DivorceMonthAm = new EthiopicDateTime(convertor.getSplitted(divorce?.EventDateEt).month, "Am").month,
                DivorceDay = convertor.getSplitted(divorce?.EventDateEt).day.ToString(),
                DivorceYear = convertor.getSplitted(divorce?.EventDateEt).year.ToString(),

                // BirthAddressAm = birth?.EventAddress?.Id.ToString(),
                DivorceAddressAm = address?.am,
                DivorceAddressOr = address?.or,

                EventRegisteredMonthOr = new EthiopicDateTime(convertor.getSplitted(divorce.EventRegDateEt).month, "or").month,
                EventRegisteredMonthAm = new EthiopicDateTime(convertor.getSplitted(divorce.EventRegDateEt).month, "am").month,
                EventRegisteredDay = convertor.getSplitted(divorce.EventRegDateEt).day.ToString(),
                EventRegisteredYear = convertor.getSplitted(divorce.EventRegDateEt).year.ToString(),

                GeneratedMonthOr = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "or").month,
                GeneratedMonthAm = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "am").month,
                GeneratedDay = convertor.getSplitted(CreatedAtEt).day.ToString(),
                GeneratedYear = convertor.getSplitted(CreatedAtEt).year.ToString(),

                CivileRegOfficerFullNameOr = divorce.CivilRegOfficer?.FirstName?.Value<string>("or") + " "
                                           + divorce.CivilRegOfficer?.MiddleName?.Value<string>("or") + " "
                                           + divorce.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = divorce.CivilRegOfficer?.FirstName?.Value<string>("am") + " "
                                           + divorce.CivilRegOfficer?.MiddleName?.Value<string>("am") + " "
                                           + divorce.CivilRegOfficer?.LastName?.Value<string>("am"),

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