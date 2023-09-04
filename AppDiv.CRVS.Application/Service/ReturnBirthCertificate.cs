using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Service
{
    public class ReturnBirthCertificate : IReturnBirthCertificate
    {
        IDateAndAddressService _DateAndAddressService;
        private readonly IReportRepostory _reportRepostory;
        public ReturnBirthCertificate(IReportRepostory reportRepostory,IDateAndAddressService DateAndAddressService)
        {
            _DateAndAddressService = DateAndAddressService;
            _reportRepostory = reportRepostory;
        }

        public BirthCertificateDTO GetBirthCertificate(BirthEvent birth, string? BirthCertNo)
        {
              var BirthAddress=  _reportRepostory.ReturnAddress(birth?.Event?.EventAddressId.ToString()).Result;
            JArray BirthAddressjsonObject = JArray.FromObject(BirthAddress);
            FormatedAddressDto BirthAddressResponse = BirthAddressjsonObject.ToObject<List<FormatedAddressDto>>().FirstOrDefault();           
            
            (string? am, string? or)? address = _DateAndAddressService.stringAddress(BirthAddressResponse);
            
            var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(birth.Event.CreatedAt);

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

                BirthMonthOr = new EthiopicDateTime(convertor.getSplitted(birth?.Event?.EventDateEt).month, "or")?.month,
                BirthMonthAm = new EthiopicDateTime(convertor.getSplitted(birth?.Event?.EventDateEt).month, "Am")?.month,
                BirthDay = convertor.getSplitted(birth?.Event?.EventDateEt).day.ToString("D2"),
                BirthYear = convertor.getSplitted(birth?.Event?.EventDateEt).year.ToString(),

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

                EventRegisteredMonthOr = new EthiopicDateTime(convertor.getSplitted(birth.Event.EventRegDateEt).month, "or").month,
                EventRegisteredMonthAm = new EthiopicDateTime(convertor.getSplitted(birth.Event.EventRegDateEt).month, "am").month,
                EventRegisteredDay = convertor.getSplitted(birth.Event.EventRegDateEt).day.ToString("D2"),
                EventRegisteredYear = convertor.getSplitted(birth.Event.EventRegDateEt).year.ToString(),

                GeneratedMonthOr = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "or").month,
                GeneratedMonthAm = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "am").month,
                GeneratedDay = convertor.getSplitted(CreatedAtEt).day.ToString("D2"),
                GeneratedYear = convertor.getSplitted(CreatedAtEt).year.ToString(),

                CivileRegOfficerFullNameOr = birth.Event.CivilRegOfficer?.FirstName?.Value<string>("or") + " "
                                           + birth.Event.CivilRegOfficer?.MiddleName?.Value<string>("or") + " "
                                           + birth.Event.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = birth.Event.CivilRegOfficer?.FirstName?.Value<string>("am") + " "
                                           + birth.Event.CivilRegOfficer?.MiddleName?.Value<string>("am") + " "
                                           + birth.Event.CivilRegOfficer?.LastName?.Value<string>("am"),

                CountryOr = BirthAddressResponse?.CountryOr,
                CountryAm = BirthAddressResponse?.CountryAm,
                RegionOr = BirthAddressResponse?.RegionOr,
                RegionAm = BirthAddressResponse?.RegionAm,
                ZoneOr = BirthAddressResponse?.ZoneOr,
                ZoneAm = BirthAddressResponse?.ZoneAm,
                WoredaOr = BirthAddressResponse?.WoredaOr,
                WoredaAm = BirthAddressResponse?.WoredaOr,
                KebeleOr = BirthAddressResponse?.KebeleOr,
                KebeleAm = BirthAddressResponse?.KebeleAm,

            };
        }
    }
}