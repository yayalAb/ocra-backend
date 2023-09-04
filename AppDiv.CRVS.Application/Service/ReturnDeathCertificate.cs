using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Service
{
    public class ReturnDeathCertificate : IReturnDeathCertificate
    {
        IDateAndAddressService _DateAndAddressService;
        private readonly IReportRepostory _reportRepostory;
        public ReturnDeathCertificate(IReportRepostory reportRepostory,IDateAndAddressService DateAndAddressService)
        {
            _DateAndAddressService = DateAndAddressService;
            _reportRepostory=reportRepostory;
        }

        public DeathCertificateDTO GetDeathCertificate(DeathEvent death, string? BirthCertNo)
        {
            var deathAddress=  _reportRepostory.ReturnAddress(death.Event?.EventAddressId .ToString()).Result;
            JArray deathAddressjsonObject = JArray.FromObject(deathAddress);
            FormatedAddressDto deathAddressResponse = deathAddressjsonObject.ToObject<List<FormatedAddressDto>>().FirstOrDefault();           
            (string? am, string? or)? address = _DateAndAddressService.stringAddress(deathAddressResponse);

            var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(death.Event.CreatedAt);
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

                TitleAm = death.Event?.EventOwener?.TitleLookup?.Value?.Value<string>("am"),
                TitleOr = death.Event?.EventOwener?.TitleLookup?.Value?.Value<string>("or"),

                GenderAm = death.Event?.EventOwener?.SexLookup?.Value?.Value<string>("am"),
                GenderOr = death.Event?.EventOwener?.SexLookup?.Value?.Value<string>("or"),

                BirthMonthOr = new EthiopicDateTime(convertor.getSplitted(death.Event.EventOwener.BirthDateEt).month, "or").month,
                BirthMonthAm = new EthiopicDateTime(convertor.getSplitted(death.Event.EventOwener.BirthDateEt).month, "Am").month,
                BirthDay = convertor.getSplitted(death.Event.EventOwener.BirthDateEt).day.ToString("D2"),
                BirthYear = convertor.getSplitted(death.Event.EventOwener.BirthDateEt).year.ToString(),

                // BirthAddressAm = death.Event?.EventAddress?.Id.ToString(),
                DeathPlaceAm = address?.am,
                DeathPlaceOr = address?.or,

                DeathMonthOr = new EthiopicDateTime(convertor.getSplitted(death.Event.EventDateEt).month, "or").month,
                DeathMonthAm = new EthiopicDateTime(convertor.getSplitted(death.Event.EventDateEt).month, "Am").month,
                DeathDay = convertor.getSplitted(death.Event.EventDateEt).day.ToString("D2"),
                DeathYear = convertor.getSplitted(death.Event.EventDateEt).year.ToString(),

                // DeathMonth = death.Event.EventDate.Month.ToString(),
                // DeathDay = death.Event.EventDate.day.ToString("D2"),
                // DeathYear = death.Event.EventDate.Year.ToString(),

                NationalityOr = death.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                NationalityAm = death.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                EventRegisteredMonthOr = new EthiopicDateTime(convertor.getSplitted(death.Event.EventRegDateEt).month, "or").month,
                EventRegisteredMonthAm = new EthiopicDateTime(convertor.getSplitted(death.Event.EventRegDateEt).month, "am").month,
                EventRegisteredDay = convertor.getSplitted(death.Event.EventRegDateEt).day.ToString("D2"),
                EventRegisteredYear = convertor.getSplitted(death.Event.EventRegDateEt).year.ToString(),

                GeneratedMonthOr = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "or").month,
                GeneratedMonthAm = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "am").month,
                GeneratedDay = convertor.getSplitted(CreatedAtEt).day.ToString("D2"),
                GeneratedYear = convertor.getSplitted(CreatedAtEt).year.ToString(),

                CivileRegOfficerFullNameOr = death.Event.CivilRegOfficer?.FirstName?.Value<string>("or") + " "
                                           + death.Event.CivilRegOfficer?.MiddleName?.Value<string>("or") + " "
                                           + death.Event.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = death.Event.CivilRegOfficer?.FirstName?.Value<string>("am") + " "
                                           + death.Event.CivilRegOfficer?.MiddleName?.Value<string>("am") + " "
                                           + death.Event.CivilRegOfficer?.LastName?.Value<string>("am"),

                 CountryOr = deathAddressResponse?.CountryOr,
                CountryAm = deathAddressResponse?.CountryAm,
                RegionOr = deathAddressResponse?.RegionOr,
                RegionAm = deathAddressResponse?.RegionAm,
                ZoneOr = deathAddressResponse?.ZoneOr,
                ZoneAm = deathAddressResponse?.ZoneAm,
                WoredaOr = deathAddressResponse?.WoredaOr,
                WoredaAm = deathAddressResponse?.WoredaOr,
                KebeleOr = deathAddressResponse?.KebeleOr,
                KebeleAm = deathAddressResponse?.KebeleAm,


            };
        }
    }
}