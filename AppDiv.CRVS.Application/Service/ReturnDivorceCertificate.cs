using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Service
{
    public class ReturnDivorceCertificate : IReturnDivorceCertificate
    {
        IDateAndAddressService _DateAndAddressService;
        private readonly IReportRepostory _reportRepostory;
        public ReturnDivorceCertificate(IReportRepostory reportRepostory,IDateAndAddressService DateAndAddressService)
        {
            _DateAndAddressService = DateAndAddressService;
            _reportRepostory=reportRepostory;
        }

        public DivorceCertificateDTO GetDivorceCertificate(DivorceEvent divorce, string? BirthCertNo)
        {
            var eventAddress=  _reportRepostory.ReturnAddress(divorce.Event?.EventAddressId.ToString()).Result;
            JArray eventAddressjsonObject = JArray.FromObject(eventAddress);
            FormatedAddressDto eventAddressResponse = eventAddressjsonObject.ToObject<List<FormatedAddressDto>>().FirstOrDefault();           
            (string? am, string? or)? address = _DateAndAddressService.stringAddress(eventAddressResponse);

            // wife birth address
            var wifeBirthAddressRes=  _reportRepostory.ReturnAddress(divorce.DivorcedWife?.BirthAddressId .ToString()).Result;
            JArray wifeBirthAddressjsonObject = JArray.FromObject(wifeBirthAddressRes);
            FormatedAddressDto wifeBirthAddressResponse = wifeBirthAddressjsonObject.ToObject<List<FormatedAddressDto>>().FirstOrDefault();           
            (string? am, string? or)? wifeBirthAddress = _DateAndAddressService.stringAddress(wifeBirthAddressResponse);

            // husband birth address
            var husbandBirthAddressRes=  _reportRepostory.ReturnAddress(divorce.Event.EventOwener?.BirthAddressId.ToString()).Result;
            JArray husbandBirthAddressjsonObject = JArray.FromObject(husbandBirthAddressRes);
            FormatedAddressDto husbandBirthAddressResponse = husbandBirthAddressjsonObject.ToObject<List<FormatedAddressDto>>().FirstOrDefault();           
            
            (string? am, string? or)? husbandBirthAddress = _DateAndAddressService.stringAddress(husbandBirthAddressResponse);
     
            var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(divorce.Event.CreatedAt);

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