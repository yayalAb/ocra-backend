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
    public class ReturnAdoptionArchive
    {
        IDateAndAddressService _DateAndAddressService;
        public ReturnAdoptionArchive(IDateAndAddressService DateAndAddressService)
        {
            _DateAndAddressService = DateAndAddressService;
        }
        AdoptionArchiveDTO GetAdoptionArchive(Event adoption, string? BirthCertNo)
        {
            (string am, string or)? address = (adoption.EventOwener?.BirthAddressId == Guid.Empty
               || adoption.EventOwener?.BirthAddressId == null) ? null :
               _DateAndAddressService.addressFormat(adoption.EventOwener.BirthAddressId);
            (string[] am, string[] or) splitedAddress = _DateAndAddressService.SplitedAddress(address?.am, address?.or);

            var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(DateTime.Now);

            // var mon=monthname.
            return new AdoptionArchiveDTO()
            {
                CertifcateId = adoption.CertificateId,
                RegBookNo = adoption.RegBookNo,
                BirthCertifcateId = (adoption.AdoptionEvent.BirthCertificateId == null || string.IsNullOrEmpty(adoption.AdoptionEvent.BirthCertificateId)) ? BirthCertNo : adoption.AdoptionEvent.BirthCertificateId,
                ChildFirstNameAm = adoption.EventOwener?.FirstName?.Value<string>("am"),
                ChildMiddleNameAm = adoption.EventOwener?.MiddleName?.Value<string>("am"),
                ChildLastNameAm = adoption.EventOwener?.LastName?.Value<string>("am"),
                ChildFirstNameOr = adoption.EventOwener?.FirstName?.Value<string>("or"),
                ChildMiddleNameOr = adoption.EventOwener?.MiddleName?.Value<string>("or"),
                ChildLastNameOr = adoption.EventOwener?.LastName?.Value<string>("or"),
                GenderAm = adoption.EventOwener?.SexLookup?.Value?.Value<string>("am"),
                GenderOr = adoption.EventOwener?.SexLookup?.Value?.Value<string>("or"),

                BirthMonthOr = new EthiopicDateTime(convertor.getSplitted(adoption.EventDateEt).month, "or").month,
                BirthMonthAm = new EthiopicDateTime(convertor.getSplitted(adoption.EventDateEt).month, "Am").month,
                BirthDay = convertor.getSplitted(adoption.EventDateEt).day.ToString(),
                BirthYear = convertor.getSplitted(adoption.EventDateEt).year.ToString(),
                BirthAddressAm = address?.am,
                BirthAddressOr = address?.or,
                NationalityOr = adoption.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                NationalityAm = adoption.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                MotherFullNameOr = adoption.AdoptionEvent.AdoptiveMother?.FirstName?.Value<string>("or")
                    + " " + adoption.AdoptionEvent.AdoptiveMother?.MiddleName?.Value<string>("or") + " " + adoption.AdoptionEvent.AdoptiveMother?.LastName?.Value<string>("or"),
                MotherFullNameAm = adoption.AdoptionEvent.AdoptiveMother?.FirstName?.Value<string>("am")
                    + " " + adoption.AdoptionEvent.AdoptiveMother?.MiddleName?.Value<string>("am") + " " + adoption.AdoptionEvent.AdoptiveMother?.LastName?.Value<string>("am"),
                MotherNationalityOr = adoption.AdoptionEvent.AdoptiveMother?.NationalityLookup?.Value?.Value<string>("or"),
                MotherNationalityAm = adoption.AdoptionEvent.AdoptiveMother?.NationalityLookup?.Value?.Value<string>("am"),

                FatherFullNameOr = adoption.AdoptionEvent.AdoptiveFather?.FirstName?.Value<string>("or")
                    + " " + adoption.AdoptionEvent.AdoptiveFather?.MiddleName?.Value<string>("or") + " " + adoption.AdoptionEvent.AdoptiveFather?.LastName?.Value<string>("or"),
                FatherFullNameAm = adoption.AdoptionEvent.AdoptiveFather?.FirstName?.Value<string>("am")
                    + " " + adoption.AdoptionEvent.AdoptiveFather?.MiddleName?.Value<string>("am") + " " + adoption.AdoptionEvent.AdoptiveFather?.LastName?.Value<string>("am"),
                FatherNationalityOr = adoption.AdoptionEvent.AdoptiveFather?.NationalityLookup?.Value?.Value<string>("or"),
                FatherNationalityAm = adoption.AdoptionEvent.AdoptiveFather?.NationalityLookup?.Value?.Value<string>("am"),
                EventRegisteredMonthOr = new EthiopicDateTime(convertor.getSplitted(adoption.EventRegDateEt).month, "or").month,
                EventRegisteredMonthAm = new EthiopicDateTime(convertor.getSplitted(adoption.EventRegDateEt).month, "am").month,
                EventRegisteredDay = convertor.getSplitted(adoption.EventRegDateEt).day.ToString(),
                EventRegisteredYear = convertor.getSplitted(adoption.EventRegDateEt).year.ToString(),


                GeneratedMonthOr = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "or").month,
                GeneratedMonthAm = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "am").month,
                GeneratedDay = convertor.getSplitted(CreatedAtEt).day.ToString(),
                GeneratedYear = convertor.getSplitted(CreatedAtEt).year.ToString(),


                CivileRegOfficerFullNameOr = adoption.CivilRegOfficer?.FirstName?.Value<string>("or")
                    + " " + adoption.CivilRegOfficer?.MiddleName?.Value<string>("or") + " " + adoption.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = adoption.CivilRegOfficer?.FirstName?.Value<string>("am")
                    + " " + adoption.CivilRegOfficer?.MiddleName?.Value<string>("am") + " " + adoption.CivilRegOfficer?.LastName?.Value<string>("am"),
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