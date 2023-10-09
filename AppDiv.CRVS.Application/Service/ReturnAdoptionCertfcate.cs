using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Service
{
    public class ReturnAdoptionCertfcate : IReturnAdoptionCertfcate
    {
        IDateAndAddressService _DateAndAddressService;
        private readonly IReportRepostory _reportRepostory;
        public ReturnAdoptionCertfcate( IReportRepostory reportRepostory,IDateAndAddressService DateAndAddressService)
        {
            _DateAndAddressService = DateAndAddressService;
            _reportRepostory=reportRepostory;
        }
        AdoptionCertificateDTO IReturnAdoptionCertfcate.GetAdoptionCertificate(AdoptionEvent adoption, string? BirthCertNo)
        {
            var BirthAddress=  _reportRepostory.ReturnAddress(adoption?.Event?.EventOwener?.BirthAddressId.ToString()).Result;
            JArray BirthAddressjsonObject = JArray.FromObject(BirthAddress);
            FormatedAddressDto BirthAddressResponse = BirthAddressjsonObject.ToObject<List<FormatedAddressDto>>().FirstOrDefault();           
            
            (string? am, string? or)? address = _DateAndAddressService.stringAddress(BirthAddressResponse);
            
            var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(DateTime.Now);

            // var mon=monthname.
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

                BirthMonthOr = new EthiopicDateTime(convertor.getSplitted(adoption.Event.EventOwener.BirthDateEt).month, "or").month,
                BirthMonthAm = new EthiopicDateTime(convertor.getSplitted(adoption.Event.EventOwener.BirthDateEt).month, "Am").month,
                BirthDay = convertor.getSplitted(adoption.Event.EventOwener.BirthDateEt).day.ToString("D2"),
                BirthYear = convertor.getSplitted(adoption.Event.EventOwener.BirthDateEt).year.ToString(),
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
                EventRegisteredMonthOr = new EthiopicDateTime(convertor.getSplitted(adoption.Event.EventRegDateEt).month, "or").month,
                EventRegisteredMonthAm = new EthiopicDateTime(convertor.getSplitted(adoption.Event.EventRegDateEt).month, "am").month,
                EventRegisteredDay = convertor.getSplitted(adoption.Event.EventRegDateEt).day.ToString("D2"),
                EventRegisteredYear = convertor.getSplitted(adoption.Event.EventRegDateEt).year.ToString(),


                GeneratedMonthOr = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "or").month,
                GeneratedMonthAm = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "am").month,
                GeneratedDay = convertor.getSplitted(CreatedAtEt).day.ToString("D2"),
                GeneratedYear = convertor.getSplitted(CreatedAtEt).year.ToString(),


                CivileRegOfficerFullNameOr = adoption.Event.CivilRegOfficer?.FirstName?.Value<string>("or")
                    + " " + adoption.Event.CivilRegOfficer?.MiddleName?.Value<string>("or") + " " + adoption.Event.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = adoption.Event.CivilRegOfficer?.FirstName?.Value<string>("am")
                    + " " + adoption.Event.CivilRegOfficer?.MiddleName?.Value<string>("am") + " " + adoption.Event.CivilRegOfficer?.LastName?.Value<string>("am"),
                //splited address
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