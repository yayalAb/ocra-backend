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
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;

namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public class ReturnDeathArchive : IReturnDeathArchive
    {
        IDateAndAddressService _dateAndAddressService;
        private readonly ILookupFromId _lookupService;
        private readonly IPersonalInfoRepository _person;
        public ReturnDeathArchive(IDateAndAddressService DateAndAddressService, ILookupFromId lookupService, IPersonalInfoRepository person)
        {
            _dateAndAddressService = DateAndAddressService;
            _lookupService = lookupService;
            _person = person;
        }

        private DeathInfo GetEventInfo(Event death)
        {
            DeathInfo deathInfo = CustomMapper.Mapper.Map<DeathInfo>(ReturnPerson.GetEventInfo(death, _dateAndAddressService));
            deathInfo.BirthCertificateId = death?.DeathEventNavigation?.BirthCertificateId;
            deathInfo.PlaceOfFuneral = death?.DeathEventNavigation?.PlaceOfFuneral;
            deathInfo.DuringDeathAm = death?.DeathEventNavigation?.DuringDeathLookup?.Value?.Value<string>("am");
            deathInfo.DuringDeathOr = death?.DeathEventNavigation?.DuringDeathLookup?.Value?.Value<string>("or");
            return deathInfo;

        }
        private DeathNotificationArchive GetNotification(DeathNotification death)
        {
            return new DeathNotificationArchive
            {
                CauseOfDeath = death?.CauseOfDeath,
                CauseOfDeathInfoTypeOr = death?.CauseOfDeathInfoTypeLookup?.Value?.Value<string>("or"),
                CauseOfDeathInfoTypeAm = death?.CauseOfDeathInfoTypeLookup?.Value?.Value<string>("am"),
                DeathNotificationSerialNumber = death?.DeathNotificationSerialNumber,
            };
        }

        private RegistrarArchive GetRegistrar(Registrar reg)
        {
            RegistrarArchive regInfo = CustomMapper.Mapper.Map<RegistrarArchive>(ReturnPerson.GetPerson(reg.RegistrarInfo, _dateAndAddressService, _lookupService));
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
                Deceased = ReturnPerson.GetPerson(death.EventOwener, _dateAndAddressService, _lookupService),
                EventInfo = GetEventInfo(death),
                Notification = GetNotification(death.DeathEventNavigation.DeathNotification),
                Registrar = GetRegistrar(death.EventRegistrar),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(death.CivilRegOfficer, _dateAndAddressService, _lookupService)),


            };
        }
        public DeathArchiveDTO GetDeathPreviewArchive(DeathEvent death, string? BirthCertNo)
        {
            (string am, string or)? address = (death?.Event.EventAddressId == Guid.Empty
               || death?.Event.EventAddressId == null) ? null :
               _dateAndAddressService.addressFormat(death.Event.EventAddressId);

            (string am, string or)? resident = (death?.Event.EventOwener.ResidentAddressId == Guid.Empty
               || death?.Event.EventOwener.ResidentAddressId == null) ? null :
               _dateAndAddressService.addressFormat(death.Event.EventOwener.ResidentAddressId);

            (string am, string or)? regAddress = (death?.Event.EventRegistrar.RegistrarInfo?.ResidentAddressId == Guid.Empty
               || death?.Event.EventRegistrar.RegistrarInfo?.ResidentAddressId == null) ? null :
               _dateAndAddressService.addressFormat(death.Event.EventRegistrar.RegistrarInfo.ResidentAddressId);
            death.Event.DeathEventNavigation = death;


            var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(death.CreatedAt);

            (string[] am, string[] or) splitedAddress = _dateAndAddressService.SplitedAddress(address?.am, address?.or);
            return new DeathArchiveDTO()
            {
                Deceased = ReturnPerson.GetPerson(death.Event.EventOwener, _dateAndAddressService, _lookupService),
                EventInfo = GetEventInfo(death.Event),
                Notification = GetNotification(death.DeathNotification),
                Registrar = GetRegistrar(death.Event.EventRegistrar),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(death.Event.CivilRegOfficer, _dateAndAddressService, _lookupService)),
            };
        }
    }
}