using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive.DeathArchive;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;
using AppDiv.CRVS.Application.Interfaces.Archive;
using AppDiv.CRVS.Application.Mapper;

namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public class ReturnDeathArchive : IReturnDeathArchive
    {
        IDateAndAddressService _dateAndAddressService;
        public ReturnDeathArchive(IDateAndAddressService DateAndAddressService)
        {
            _dateAndAddressService = DateAndAddressService;
        }

        private DeathInfo GetEventInfo(Event death)
        {
            DeathInfo deathInfo = CustomMapper.Mapper.Map<DeathInfo>(ReturnPerson.GetEventInfo(death, _dateAndAddressService));
            // birthInfo.WeightAtBirth = death.BirthEvent.BirthNotification.WeightAtBirth;
            // birthInfo.DeliveryTypeOr = death.BirthEvent.BirthNotification.DeliveryTypeLookup?.Value?.Value<string>("or");
            // birthInfo.DeliveryTypeAm = death.BirthEvent.BirthNotification.DeliveryTypeLookup?.Value?.Value<string>("am");
            // birthInfo.SkilledProfessionalOr = death.BirthEvent.BirthNotification.SkilledProfLookup?.Value?.Value<string>("or");
            // birthInfo.SkilledProfessionalAm = death.BirthEvent.BirthNotification.SkilledProfLookup?.Value?.Value<string>("am");
            // birthInfo.TypeOfBirthOr = death.BirthEvent.TypeOfBirthLookup?.Value?.Value<string>("or");
            // birthInfo.TypeOfBirthAm = death.BirthEvent.TypeOfBirthLookup?.Value?.Value<string>("am");
            deathInfo.DeathNotificationSerialNumber = death.DeathEventNavigation.DeathNotification.DeathNotificationSerialNumber;
            return deathInfo;
        }

        private RegistrarArchive GetRegistrar(Registrar reg)
        {
            RegistrarArchive regInfo = CustomMapper.Mapper.Map<RegistrarArchive>(ReturnPerson.GetPerson(reg.RegistrarInfo, _dateAndAddressService));
            regInfo.RelationShipOr = reg.RelationshipLookup.Value?.Value<string>("or");
            regInfo.RelationShipAm = reg.RelationshipLookup.Value?.Value<string>("am");
            return regInfo;
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
                Deceased = ReturnPerson.GetPerson(death.EventOwener, _dateAndAddressService),
                EventInfo = GetEventInfo(death),
                Registrar = GetRegistrar(death.EventRegistrar),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(death.CivilRegOfficer, _dateAndAddressService)),
                // CertifcateId = death.CertificateId,
                // RegBookNo = death.RegBookNo,
                // BirthCertifcateId = death.DeathEventNavigation.BirthCertificateId,
                // FirstNameAm = death.EventOwener?.FirstName?.Value<string>("am"),
                // MiddleNameAm = death.EventOwener?.MiddleName?.Value<string>("am"),
                // LastNameAm = death.EventOwener?.LastName?.Value<string>("am"),
                // FirstNameOr = death.EventOwener?.FirstName?.Value<string>("or"),
                // MiddleNameOr = death.EventOwener?.MiddleName?.Value<string>("or"),
                // LastNameOr = death.EventOwener?.LastName?.Value<string>("or"),



            };
        }
    }
}