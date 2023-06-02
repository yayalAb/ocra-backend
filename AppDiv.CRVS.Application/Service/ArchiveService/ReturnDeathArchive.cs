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
    public class ReturnDeathArchive : IReturnDeathArchive
    {
        IDateAndAddressService _dateAndAddressService;
        public ReturnDeathArchive(IDateAndAddressService DateAndAddressService)
        {
            _dateAndAddressService = DateAndAddressService;
        }

        public DeathArchiveDTO GetDeathArchive(Event death, string? BirthCertNo)
        {
            (string am, string or)? address = (death?.EventAddressId == Guid.Empty
               || death?.EventAddressId == null) ? null :
               _dateAndAddressService.addressFormat(death.EventAddressId);

            (string am, string or)? resident = (death?.EventOwener.ResidentAddressId == Guid.Empty
               || death?.EventOwener.ResidentAddressId == null) ? null :
               _dateAndAddressService.addressFormat(death.EventOwener.ResidentAddressId);

            (string am, string or)? regAddress = (death?.EventRegistrar.RegistrarInfo?.ResidentAddressId == Guid.Empty
               || death?.EventRegistrar.RegistrarInfo?.ResidentAddressId == null) ? null :
               _dateAndAddressService.addressFormat(death.EventRegistrar.RegistrarInfo.ResidentAddressId);



            var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(death.CreatedAt);

            (string[] am, string[] or) splitedAddress = _dateAndAddressService.SplitedAddress(address?.am, address?.or);
            return new DeathArchiveDTO()
            {
                CertifcateId = death.CertificateId,
                RegBookNo = death.RegBookNo,
                BirthCertifcateId = death.DeathEventNavigation.BirthCertificateId,
                FirstNameAm = death.EventOwener?.FirstName?.Value<string>("am"),
                MiddleNameAm = death.EventOwener?.MiddleName?.Value<string>("am"),
                LastNameAm = death.EventOwener?.LastName?.Value<string>("am"),
                FirstNameOr = death.EventOwener?.FirstName?.Value<string>("or"),
                MiddleNameOr = death.EventOwener?.MiddleName?.Value<string>("or"),
                LastNameOr = death.EventOwener?.LastName?.Value<string>("or"),

                TitleAm = death?.EventOwener?.TitleLookup?.Value?.Value<string>("am"),
                TitleOr = death?.EventOwener?.TitleLookup?.Value?.Value<string>("or"),

                GenderAm = death?.EventOwener?.SexLookup?.Value?.Value<string>("am"),
                GenderOr = death?.EventOwener?.SexLookup?.Value?.Value<string>("or"),

                BirthMonthOr = new EthiopicDateTime(convertor.getSplitted(death.EventOwener.BirthDateEt).month, "or").month,
                BirthMonthAm = new EthiopicDateTime(convertor.getSplitted(death.EventOwener.BirthDateEt).month, "Am").month,
                BirthDay = convertor.getSplitted(death.EventOwener.BirthDateEt).day.ToString(),
                BirthYear = convertor.getSplitted(death.EventOwener.BirthDateEt).year.ToString(),

                // BirthAddressAm = death?.EventAddress?.Id.ToString(),
                DeathPlaceAm = address?.am,
                DeathPlaceOr = address?.or,

                DeathMonthOr = new EthiopicDateTime(convertor.getSplitted(death.EventDateEt).month, "or").month,
                DeathMonthAm = new EthiopicDateTime(convertor.getSplitted(death.EventDateEt).month, "Am").month,
                DeathDay = convertor.getSplitted(death.EventDateEt).day.ToString(),
                DeathYear = convertor.getSplitted(death.EventDateEt).year.ToString(),

                // DeathMonth = death.EventDate.Month.ToString(),
                // DeathDay = death.EventDate.Day.ToString(),
                // DeathYear = death.EventDate.Year.ToString(),

                NationalityOr = death?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                NationalityAm = death?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                EventRegisteredMonthOr = new EthiopicDateTime(convertor.getSplitted(death.EventRegDateEt).month, "or").month,
                EventRegisteredMonthAm = new EthiopicDateTime(convertor.getSplitted(death.EventRegDateEt).month, "am").month,
                EventRegisteredDay = convertor.getSplitted(death.EventRegDateEt).day.ToString(),
                EventRegisteredYear = convertor.getSplitted(death.EventRegDateEt).year.ToString(),

                GeneratedMonthOr = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "or").month,
                GeneratedMonthAm = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "am").month,
                GeneratedDay = convertor.getSplitted(CreatedAtEt).day.ToString(),
                GeneratedYear = convertor.getSplitted(CreatedAtEt).year.ToString(),

                CivileRegOfficerFullNameOr = death.CivilRegOfficer?.FirstName?.Value<string>("or") + " "
                                           + death.CivilRegOfficer?.MiddleName?.Value<string>("or") + " "
                                           + death.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = death.CivilRegOfficer?.FirstName?.Value<string>("am") + " "
                                           + death.CivilRegOfficer?.MiddleName?.Value<string>("am") + " "
                                           + death.CivilRegOfficer?.LastName?.Value<string>("am"),

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


                // archive data

                ResidentAddressAm = resident?.am,
                ResidentAddressOr = resident?.or,

                NationAm = death.EventOwener.NationLookup.Value?.Value<string>("am"),
                NationOr = death.EventOwener.NationLookup.Value?.Value<string>("or"),

                ReligionAm = death.EventOwener.ReligionLookup.Value?.Value<string>("am"),
                ReligionOr = death.EventOwener.ReligionLookup.Value?.Value<string>("or"),

                EducationalStatusAm = death.EventOwener.EducationalStatusLookup.Value?.Value<string>("am"),
                EducationalStatusOr = death.EventOwener.EducationalStatusLookup.Value?.Value<string>("or"),

                TypeOfWorkAm = death.EventOwener.TypeOfWorkLookup.Value?.Value<string>("am"),
                TypeOfWorkOr = death.EventOwener.TypeOfWorkLookup.Value?.Value<string>("or"),

                MarriageStatusAm = death.EventOwener.MarraigeStatusLookup.Value?.Value<string>("am"),
                MarriageStatusOr = death.EventOwener.MarraigeStatusLookup.Value?.Value<string>("or"),

                PlaceOfFuneralAm = death.DeathEventNavigation.PlaceOfFuneral,
                PlaceOfFuneralOr = death.DeathEventNavigation.PlaceOfFuneral,

                RegistrarFirstNameAm = death.EventRegistrar?.RegistrarInfo?.FirstName.Value<string>("am"),
                RegistrarFirstNameOr = death.EventRegistrar?.RegistrarInfo?.FirstName.Value<string>("or"),
                RegistrarMiddleNameAm = death.EventRegistrar?.RegistrarInfo?.MiddleName.Value<string>("am"),
                RegistrarMiddleNameOr = death.EventRegistrar?.RegistrarInfo?.MiddleName.Value<string>("or"),
                RegistrarLastNameAm = death.EventRegistrar?.RegistrarInfo?.LastName.Value<string>("am"),
                RegistrarLastNameOr = death.EventRegistrar?.RegistrarInfo?.LastName.Value<string>("or"),

                RegistrarResidentAddressAm = regAddress?.am,
                RegistrarResidentAddressOr = regAddress?.or,

                RegistrarRelationShipAm = death.EventRegistrar?.RelationshipLookup.Value?.Value<string>("am"),
                RegistrarRelationShipOr = death.EventRegistrar?.RelationshipLookup.Value?.Value<string>("or"),

                RegistrarNationalId = death.EventRegistrar?.RegistrarInfo?.NationalId,

                DuringDeathAm = death.DeathEventNavigation.DuringDeathLookup.Value?.Value<string>("am"),
                DuringDeathOr = death.DeathEventNavigation.DuringDeathLookup.Value?.Value<string>("or"),

                CauseOfDeath = death.DeathEventNavigation.DeathNotification.CauseOfDeath,
                // CauseOfDeathOr = death.DeathEventNavigation.DeathNotification.DuringDeathLookup.Value?.Value<string>("or"),
                CauseOfDeathInfoTypeAm = death.DeathEventNavigation.DeathNotification.CauseOfDeathInfoTypeLookup.Value?.Value<string>("am"),
                CauseOfDeathInfoTypeOr = death.DeathEventNavigation.DeathNotification.CauseOfDeathInfoTypeLookup.Value?.Value<string>("or"),


                DeathNotificationSerialNumber = death.DeathEventNavigation.DeathNotification.DeathNotificationSerialNumber,





            };
        }
    }
}