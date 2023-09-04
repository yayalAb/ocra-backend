using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Service
{
    public class ReturnMarriageCertificate : IReturnMarriageCertificate
    {
        IDateAndAddressService _DateAndAddressService;
        private readonly IReportRepostory  _reportRepostory;
        public ReturnMarriageCertificate(IReportRepostory reportRepostory,IDateAndAddressService DateAndAddressService)
        {
            _DateAndAddressService = DateAndAddressService;
            _reportRepostory=reportRepostory;
        }

        public MarriageCertificateDTO GetMarriageCertificate(MarriageEvent marriage, string? BirthCertNo)
        {
             var eventAddress=  _reportRepostory.ReturnAddress(marriage.Event?.EventAddressId.ToString()).Result;
            JArray eventAddressjsonObject = JArray.FromObject(eventAddress);
            FormatedAddressDto eventAddressResponse = eventAddressjsonObject.ToObject<List<FormatedAddressDto>>().FirstOrDefault();           
            (string? am, string? or)? address = _DateAndAddressService.stringAddress(eventAddressResponse);

            var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(marriage.Event.CreatedAt);

            return new MarriageCertificateDTO()
            {
                CertifcateId = marriage.Event?.CertificateId,
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
                BrideBirthDay = convertor.getSplitted(marriage.BrideInfo?.BirthDateEt).day.ToString("D2"),
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
                GroomBirthDay = convertor.getSplitted(marriage.Event?.EventOwener?.BirthDateEt).day.ToString("D2"),
                GroomBirthYear = convertor.getSplitted(marriage.Event?.EventOwener?.BirthDateEt).year.ToString(),

                GroomNationalityOr = marriage.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                GroomNationalityAm = marriage.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                MarriageMonthOr = new EthiopicDateTime(convertor.getSplitted(marriage.Event?.EventDateEt).month, "or").month,
                MarriageMonthAm = new EthiopicDateTime(convertor.getSplitted(marriage.Event?.EventDateEt).month, "Am").month,
                MarriageDay = convertor.getSplitted(marriage.Event?.EventDateEt).day.ToString("D2"),
                MarriageYear = convertor.getSplitted(marriage.Event?.EventDateEt).year.ToString(),

                // BirthAddressAm = marriage.Event?.EventAddress?.Id.ToString(),
                MarriageAddressAm = address?.am,
                MarriageAddressOr = address?.or,

                EventRegisteredMonthOr = new EthiopicDateTime(convertor.getSplitted(marriage.Event.EventRegDateEt).month, "or").month,
                EventRegisteredMonthAm = new EthiopicDateTime(convertor.getSplitted(marriage.Event.EventRegDateEt).month, "am").month,
                EventRegisteredDay = convertor.getSplitted(marriage.Event.EventRegDateEt).day.ToString("D2"),
                EventRegisteredYear = convertor.getSplitted(marriage.Event.EventRegDateEt).year.ToString(),

                GeneratedMonthOr = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "or").month,
                GeneratedMonthAm = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "am").month,
                GeneratedDay = convertor.getSplitted(CreatedAtEt).day.ToString("D2"),
                GeneratedYear = convertor.getSplitted(CreatedAtEt).year.ToString(),

                CivileRegOfficerFullNameOr = marriage.Event.CivilRegOfficer?.FirstName?.Value<string>("or") + " "
                                           + marriage.Event.CivilRegOfficer?.MiddleName?.Value<string>("or") + " "
                                           + marriage.Event.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = marriage.Event.CivilRegOfficer?.FirstName?.Value<string>("am") + " "
                                           + marriage.Event.CivilRegOfficer?.MiddleName?.Value<string>("am") + " "
                                           + marriage.Event.CivilRegOfficer?.LastName?.Value<string>("am"),

                CountryOr = eventAddressResponse?.CountryOr,
                CountryAm = eventAddressResponse?.CountryAm,
                RegionOr = eventAddressResponse?.RegionOr,
                RegionAm = eventAddressResponse?.RegionAm,
                ZoneOr = eventAddressResponse?.ZoneOr,
                ZoneAm = eventAddressResponse?.ZoneAm,
                WoredaOr = eventAddressResponse?.WoredaOr,
                WoredaAm = eventAddressResponse?.WoredaOr,
                KebeleOr = eventAddressResponse?.KebeleOr,
                KebeleAm = eventAddressResponse?.KebeleAm,

            };
        }
    }
}