using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive.BirthArchive;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;
using AppDiv.CRVS.Application.Interfaces.Archive;
using AppDiv.CRVS.Application.Mapper;

namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public class ReturnBirthArchive : IReturnBirthArchive
    {
        IDateAndAddressService _dateAndAddressService;
        public ReturnBirthArchive(IDateAndAddressService DateAndAddressService)
        {
            _dateAndAddressService = DateAndAddressService;
        }

        private BirthInfo GetEventInfo(Event birth)
        {
            BirthInfo birthInfo = CustomMapper.Mapper.Map<BirthInfo>(ReturnPerson.GetEventInfo(birth, _dateAndAddressService));
            birthInfo.TypeOfBirthOr = birth.BirthEvent.TypeOfBirthLookup?.Value?.Value<string>("or");
            birthInfo.TypeOfBirthAm = birth.BirthEvent.TypeOfBirthLookup?.Value?.Value<string>("am");
            birthInfo.BirthPlaceOr = birth.BirthEvent.BirthPlace?.Value?.Value<string>("or");
            birthInfo.BirthPlaceAm = birth.BirthEvent.BirthPlace?.Value?.Value<string>("am");
            return birthInfo;
        }
        private BirthNotificationArchive GetNotification(BirthNotification notification)
        {
            return new BirthNotificationArchive
            {
                WeightAtBirth = notification?.WeightAtBirth,
                DeliveryTypeOr = notification?.DeliveryTypeLookup?.Value?.Value<string>("or"),
                DeliveryTypeAm = notification?.DeliveryTypeLookup?.Value?.Value<string>("am"),
                SkilledProfessionalOr = notification?.SkilledProfLookup?.Value?.Value<string>("or"),
                SkilledProfessionalAm = notification?.SkilledProfLookup?.Value?.Value<string>("am"),
                NotificationSerialNumber = notification?.NotficationSerialNumber,
            };
        }


        private RegistrarArchive? GetRegistrar(Registrar reg)
        {
            RegistrarArchive? regInfo = CustomMapper.Mapper.Map<RegistrarArchive>(ReturnPerson.GetPerson(reg?.RegistrarInfo, _dateAndAddressService));
            regInfo.RelationShipOr = reg?.RelationshipLookup.Value?.Value<string>("or");
            regInfo.RelationShipAm = reg?.RelationshipLookup.Value?.Value<string>("am");
            return regInfo;
        }

        public BirthArchiveDTO GetBirthArchive(Event birth, string? BirthCertNo)
        {
            (string am, string or)? address = (birth.EventAddressId == Guid.Empty
               || birth?.EventAddressId == null) ? null :
               _dateAndAddressService.addressFormat(birth.EventAddressId);
            var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(birth.CreatedAt);

            (string[] am, string[] or) splitedAddress = _dateAndAddressService.SplitedAddress(address?.am, address?.or);
            return new BirthArchiveDTO()
            {
                Child = CustomMapper.Mapper.Map<Child>
                                            (ReturnPerson.GetPerson(birth.EventOwener, _dateAndAddressService)),
                Mother = ReturnPerson.GetPerson(birth.BirthEvent.Mother, _dateAndAddressService),
                Father = ReturnPerson.GetPerson(birth.BirthEvent.Father, _dateAndAddressService),
                EventInfo = GetEventInfo(birth),
                Notification = GetNotification(birth.BirthEvent.BirthNotification),
                Registrar = GetRegistrar(birth.EventRegistrar),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(birth.CivilRegOfficer, _dateAndAddressService)),

                // CertifcateId = birth.CertificateId,
                // RegBookNo = birth.RegBookNo,
                // // BirthCertifcateId = birth.BirthCertificateId,
                // ChildFirstNameAm = birth.EventOwener?.FirstName?.Value<string>("am"),
                // ChildMiddleNameAm = birth.EventOwener?.MiddleName?.Value<string>("am"),
                // ChildLastNameAm = birth.EventOwener?.LastName?.Value<string>("am"),
                // ChildFirstNameOr = birth.EventOwener?.FirstName?.Value<string>("or"),
                // ChildMiddleNameOr = birth.EventOwener?.MiddleName?.Value<string>("or"),
                // ChildLastNameOr = birth.EventOwener?.LastName?.Value<string>("or"),

                // GenderAm = birth?.EventOwener?.SexLookup?.Value?.Value<string>("am"),
                // GenderOr = birth?.EventOwener?.SexLookup?.Value?.Value<string>("or"),

                // BirthMonthOr = new EthiopicDateTime(convertor.getSplitted(birth.EventOwener.BirthDateEt).month, "or").month,
                // BirthMonthAm = new EthiopicDateTime(convertor.getSplitted(birth.EventOwener.BirthDateEt).month, "Am").month,
                // BirthDay = convertor.getSplitted(birth.EventOwener.BirthDateEt).day.ToString(),
                // BirthYear = convertor.getSplitted(birth.EventOwener.BirthDateEt).year.ToString(),

                // // BirthAddressAm = birth?.EventAddress?.Id.ToString(),
                // BirthAddressAm = address?.am,
                // BirthAddressOr = address?.or,
                // NationalityOr = birth?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                // NationalityAm = birth?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                // MotherFullNameOr = birth.BirthEvent.Mother?.FirstName?.Value<string>("or") + " "
                //                  + birth.BirthEvent.Mother?.MiddleName?.Value<string>("or") + " "
                //                  + birth.BirthEvent.Mother?.LastName?.Value<string>("or"),
                // MotherFullNameAm = birth.BirthEvent.Mother?.FirstName?.Value<string>("am") + " "
                //                  + birth.BirthEvent.Mother?.MiddleName?.Value<string>("am") + " "
                //                  + birth.BirthEvent.Mother?.LastName?.Value<string>("am"),
                // MotherNationalityOr = birth.BirthEvent.Mother?.NationalityLookup?.Value?.Value<string>("or"),
                // MotherNationalityAm = birth.BirthEvent.Mother?.NationalityLookup?.Value?.Value<string>("am"),

                // FatherFullNameOr = birth.BirthEvent.Father?.FirstName?.Value<string>("or") + " "
                //                  + birth.BirthEvent.Father?.MiddleName?.Value<string>("or") + " "
                //                  + birth.BirthEvent.Father?.LastName?.Value<string>("or"),
                // FatherFullNameAm = birth.BirthEvent.Father?.FirstName?.Value<string>("am") + " "
                //                  + birth.BirthEvent.Father?.MiddleName?.Value<string>("am") + " "
                //                  + birth.BirthEvent.Father?.LastName?.Value<string>("am"),
                // FatherNationalityOr = birth.BirthEvent.Father?.NationalityLookup?.Value?.Value<string>("or"),
                // FatherNationalityAm = birth.BirthEvent.Father?.NationalityLookup?.Value?.Value<string>("am"),

                // EventRegisteredMonthOr = new EthiopicDateTime(convertor.getSplitted(birth.EventRegDateEt).month, "or").month,
                // EventRegisteredMonthAm = new EthiopicDateTime(convertor.getSplitted(birth.EventRegDateEt).month, "am").month,
                // EventRegisteredDay = convertor.getSplitted(birth.EventRegDateEt).day.ToString(),
                // EventRegisteredYear = convertor.getSplitted(birth.EventRegDateEt).year.ToString(),

                // GeneratedMonthOr = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "or").month,
                // GeneratedMonthAm = new EthiopicDateTime(convertor.getSplitted(CreatedAtEt).month, "am").month,
                // GeneratedDay = convertor.getSplitted(CreatedAtEt).day.ToString(),
                // GeneratedYear = convertor.getSplitted(CreatedAtEt).year.ToString(),

                // CivileRegOfficerFullNameOr = birth.CivilRegOfficer?.FirstName?.Value<string>("or") + " "
                //                            + birth.CivilRegOfficer?.MiddleName?.Value<string>("or") + " "
                //                            + birth.CivilRegOfficer?.LastName?.Value<string>("or"),
                // CivileRegOfficerFullNameAm = birth.CivilRegOfficer?.FirstName?.Value<string>("am") + " "
                //                            + birth.CivilRegOfficer?.MiddleName?.Value<string>("am") + " "
                //                            + birth.CivilRegOfficer?.LastName?.Value<string>("am"),

                // CountryOr = splitedAddress.or.ElementAtOrDefault(0),
                // CountryAm = splitedAddress.am.ElementAtOrDefault(0),
                // RegionOr = splitedAddress.or.ElementAtOrDefault(1),
                // RegionAm = splitedAddress.am.ElementAtOrDefault(1),
                // ZoneOr = splitedAddress.or.ElementAtOrDefault(2),
                // ZoneAm = splitedAddress.am.ElementAtOrDefault(2),
                // WoredaOr = splitedAddress.or.ElementAtOrDefault(3),
                // WoredaAm = splitedAddress.am.ElementAtOrDefault(3),
                // CityOr = splitedAddress.or.ElementAtOrDefault(4),
                // CityAm = splitedAddress.am.ElementAtOrDefault(4),
                // KebeleOr = splitedAddress.or.ElementAtOrDefault(5),
                // KebeleAm = splitedAddress.am.ElementAtOrDefault(5),

                // archive

                // Child = Fill.Filler<Person, PersonalInfo>(new Person(), birth.EventOwener),

                // ChildWeightAtBirth = birth.BirthEvent.BirthNotification.WeightAtBirth,

                // DeliveryTypeAm = birth.BirthEvent.BirthNotification.DeliveryTypeLookup.Value?.Value<string>("am"),
                // DeliveryTypeOr = birth.BirthEvent.BirthNotification.DeliveryTypeLookup.Value?.Value<string>("or"),

                // SkilledProfessionalOr = birth.BirthEvent.BirthNotification.SkilledProfLookup.Value?.Value<string>("or"),
                // SkilledProfessionalAm = birth.BirthEvent.BirthNotification.SkilledProfLookup.Value?.Value<string>("am"),

                // TypeOfBirthOr = birth.BirthEvent.TypeOfBirthLookup.Value?.Value<string>("or"),
                // TypeOfBirthAm = birth.BirthEvent.TypeOfBirthLookup.Value?.Value<string>("am"),

                // NotificationSerialNumber = birth.BirthEvent.BirthNotification.NotficationSerialNumber,

                // MotherNationalId = birth.BirthEvent.Mother.NationalId,

                // MotherBirthMonthOr = new EthiopicDateTime(convertor.getSplitted(birth.BirthEvent.Mother.BirthDateEt).month, "or").month,
                // MotherBirthMonthAm = new EthiopicDateTime(convertor.getSplitted(birth.BirthEvent.Mother.BirthDateEt).month, "Am").month,
                // MotherBirthDay = convertor.getSplitted(birth.EventOwener.BirthDateEt).day.ToString(),
                // MotherBirthYear = convertor.getSplitted(birth.EventOwener.BirthDateEt).year.ToString(),
            };
        }
    }
}