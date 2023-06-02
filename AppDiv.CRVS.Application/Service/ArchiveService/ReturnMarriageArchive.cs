using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;
using AppDiv.CRVS.Application.Interfaces.Archive;

namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public class ReturnMarriageArchive : IReturnMarriageArchive
    {
        IDateAndAddressService _DateAndAddressService;
        public ReturnMarriageArchive(IDateAndAddressService DateAndAddressService)
        {
            _DateAndAddressService = DateAndAddressService;
        }

        public MarriageArchiveDTO GetMarriageArchive(Event marriage, string? BirthCertNo)
        {
            (string am, string or)? address = (marriage?.EventAddressId == Guid.Empty
               || marriage?.EventAddressId == null) ? null :
               _DateAndAddressService.addressFormat(marriage.EventAddressId);

            var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(marriage.CreatedAt);

            (string[] am, string[] or) splitedAddress = _DateAndAddressService.SplitedAddress(address?.am, address?.or);
            return new MarriageArchiveDTO()
            {
                CertifcateId = marriage?.CertificateId,
                BirthCertificateGroomId = marriage.MarriageEvent.BirthCertificateGroomId,
                BirthCertificateBrideId = marriage.MarriageEvent.BirthCertificateBrideId,
                RegBookNo = marriage.RegBookNo,
                // BrideBirthCertifcateId = marriage.BirthCertificateBrideId,
                BrideFirstNameAm = marriage.MarriageEvent.BrideInfo.FirstName?.Value<string>("am"),
                BrideMiddleNameAm = marriage.MarriageEvent.BrideInfo.MiddleName?.Value<string>("am"),
                BrideLastNameAm = marriage.MarriageEvent.BrideInfo.LastName?.Value<string>("am"),
                BrideFirstNameOr = marriage.MarriageEvent.BrideInfo.FirstName?.Value<string>("or"),
                BrideMiddleNameOr = marriage.MarriageEvent.BrideInfo.MiddleName?.Value<string>("or"),
                BrideLastNameOr = marriage.MarriageEvent.BrideInfo.LastName?.Value<string>("or"),

                BrideBirthMonthOr = new EthiopicDateTime(convertor.getSplitted(marriage.MarriageEvent.BrideInfo?.BirthDateEt).month, "or").month,
                BrideBirthMonthAm = new EthiopicDateTime(convertor.getSplitted(marriage.MarriageEvent.BrideInfo?.BirthDateEt).month, "Am").month,
                BrideBirthDay = convertor.getSplitted(marriage.MarriageEvent.BrideInfo?.BirthDateEt).day.ToString(),
                BrideBirthYear = convertor.getSplitted(marriage.MarriageEvent.BrideInfo?.BirthDateEt).year.ToString(),

                BrideNationalityOr = marriage?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                BrideNationalityAm = marriage?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                // GroomBirthCertifcateId = marriage.BirthCertificateGroomId,
                GroomFirstNameAm = marriage.EventOwener?.FirstName?.Value<string>("am"),
                GroomMiddleNameAm = marriage.EventOwener?.MiddleName?.Value<string>("am"),
                GroomLastNameAm = marriage.EventOwener?.LastName?.Value<string>("am"),
                GroomFirstNameOr = marriage.EventOwener?.FirstName?.Value<string>("or"),
                GroomMiddleNameOr = marriage.EventOwener?.MiddleName?.Value<string>("or"),
                GroomLastNameOr = marriage.EventOwener?.LastName?.Value<string>("or"),

                GroomBirthMonthOr = new EthiopicDateTime(convertor.getSplitted(marriage?.EventOwener?.BirthDateEt).month, "or").month,
                GroomBirthMonthAm = new EthiopicDateTime(convertor.getSplitted(marriage?.EventOwener?.BirthDateEt).month, "Am").month,
                GroomBirthDay = convertor.getSplitted(marriage?.EventOwener?.BirthDateEt).day.ToString(),
                GroomBirthYear = convertor.getSplitted(marriage?.EventOwener?.BirthDateEt).year.ToString(),

                GroomNationalityOr = marriage?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                GroomNationalityAm = marriage?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                MarriageMonthOr = new EthiopicDateTime(convertor.getSplitted(marriage?.EventDateEt).month, "or").month,
                MarriageMonthAm = new EthiopicDateTime(convertor.getSplitted(marriage?.EventDateEt).month, "Am").month,
                MarriageDay = convertor.getSplitted(marriage?.EventDateEt).day.ToString(),
                MarriageYear = convertor.getSplitted(marriage?.EventDateEt).year.ToString(),

                // BirthAddressAm = marriage?.EventAddress?.Id.ToString(),
                MarriageAddressAm = address?.am,
                MarriageAddressOr = address?.or,

                EventRegisteredMonthOr = new EthiopicDateTime(convertor.getSplitted(marriage.EventRegDateEt).month, "or").month,
                EventRegisteredMonthAm = new EthiopicDateTime(convertor.getSplitted(marriage.EventRegDateEt).month, "am").month,
                EventRegisteredDay = convertor.getSplitted(marriage.EventRegDateEt).day.ToString(),
                EventRegisteredYear = convertor.getSplitted(marriage.EventRegDateEt).year.ToString(),

                GeneratedMonthOr = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "or").month,
                GeneratedMonthAm = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "am").month,
                GeneratedDay = convertor.getSplitted(CreatedAtEt).day.ToString(),
                GeneratedYear = convertor.getSplitted(CreatedAtEt).year.ToString(),

                CivileRegOfficerFullNameOr = marriage.CivilRegOfficer?.FirstName?.Value<string>("or") + " "
                                           + marriage.CivilRegOfficer?.MiddleName?.Value<string>("or") + " "
                                           + marriage.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = marriage.CivilRegOfficer?.FirstName?.Value<string>("am") + " "
                                           + marriage.CivilRegOfficer?.MiddleName?.Value<string>("am") + " "
                                           + marriage.CivilRegOfficer?.LastName?.Value<string>("am"),

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