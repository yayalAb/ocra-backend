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
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public class ReturnDeathArchive : IReturnDeathArchive
    {
        IDateAndAddressService _dateAndAddressService;
        private readonly ILookupFromId _lookupService;
        private readonly IPersonalInfoRepository _person;
        private readonly ISupportingDocumentRepository _supportingDocument;
        public ReturnDeathArchive(IDateAndAddressService DateAndAddressService,
                                ILookupFromId lookupService,
                                IPersonalInfoRepository person,
                                ISupportingDocumentRepository supportingDocument)
        {
            _dateAndAddressService = DateAndAddressService;
            _lookupService = lookupService;
            _supportingDocument = supportingDocument;
            _person = person;
        }
        private DeathInfo GetEventInfo(Event death)
        {
            DeathInfo deathInfo = CustomMapper.Mapper.Map<DeathInfo>(ReturnPerson.GetEventInfo(death, _dateAndAddressService));
            deathInfo.BirthCertificateId = death?.DeathEventNavigation?.BirthCertificateId;
            deathInfo.PlaceOfFuneral = death?.DeathEventNavigation?.PlaceOfFuneral;
            deathInfo.DuringDeathAm = death?.DeathEventNavigation?.DuringDeathLookup?.Value?.Value<string>("am") ?? _lookupService.GetLookupOr(death?.DeathEventNavigation?.DuringDeathId);
            deathInfo.DuringDeathOr = death?.DeathEventNavigation?.DuringDeathLookup?.Value?.Value<string>("or") ?? _lookupService.GetLookupAm(death?.DeathEventNavigation?.DuringDeathId);
            deathInfo.FacilityTypeAm = death?.DeathEventNavigation?.FacilityTypeLookup?.Value?.Value<string>("am") ?? _lookupService.GetLookupOr(death?.DeathEventNavigation?.FacilityTypeLookupId);
            deathInfo.FacilityTypeOr = death?.DeathEventNavigation?.FacilityTypeLookup?.Value?.Value<string>("or") ?? _lookupService.GetLookupAm(death?.DeathEventNavigation?.FacilityTypeLookupId);
            return deathInfo;

        }
        private DeathNotificationArchive GetNotification(DeathNotification death)
        {
            // var causeOfDeath = JArray.Parse(death?.CauseOfDeath);
            return new DeathNotificationArchive
            {
                CauseOfDeathOne = (string?)death?.CauseOfDeathArray?.ElementAtOrDefault(0)?.Value<string>("reason"),
                CauseOfDeathTwo = (string?)death?.CauseOfDeathArray?.ElementAtOrDefault(1)?.Value<string>("reason"),
                CauseOfDeathThree = (string?)death?.CauseOfDeathArray?.ElementAtOrDefault(2)?.Value<string>("reason"),
                CauseOfDeathInfoTypeOr = death?.CauseOfDeathInfoTypeLookup?.Value?.Value<string>("or") ?? _lookupService.GetLookupOr(death?.CauseOfDeathInfoTypeLookupId),
                CauseOfDeathInfoTypeAm = death?.CauseOfDeathInfoTypeLookup?.Value?.Value<string>("am") ?? _lookupService.GetLookupAm(death?.CauseOfDeathInfoTypeLookupId),
                DeathNotificationSerialNumber = death?.DeathNotificationSerialNumber,
            };
        }

        private RegistrarArchive GetRegistrar(Registrar reg)
        {
            RegistrarArchive regInfo = CustomMapper.Mapper.Map<RegistrarArchive>(ReturnPerson.GetPerson(reg.RegistrarInfo, _dateAndAddressService, _lookupService));
            regInfo.RelationShipOr = reg?.RelationshipLookup?.Value?.Value<string>("or") ?? _lookupService.GetLookupOr(reg?.RelationshipLookupId);
            regInfo.RelationShipAm = reg?.RelationshipLookup?.Value?.Value<string>("am") ?? _lookupService.GetLookupAm(reg?.RelationshipLookupId);
            return regInfo;
        }
        private DeceasedPerson GetDeceased(PersonalInfo deceased)
        {
            DeceasedPerson deceasedInfo = CustomMapper.Mapper.Map<DeceasedPerson>(ReturnPerson.GetPerson(deceased, _dateAndAddressService, _lookupService));
            deceasedInfo.Age = DateTime.Today.Year - deceased?.BirthDate?.Year;
            return deceasedInfo;
        }
        public DeathArchiveDTO GetDeathArchive(Event death, string? BirthCertNo)
        {

            return new DeathArchiveDTO()
            {
                Deceased = GetDeceased(death.EventOwener),
                EventInfo = GetEventInfo(death),
                Notification = GetNotification(death.DeathEventNavigation.DeathNotification),
                Registrar = GetRegistrar(death?.EventRegistrar),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(death.CivilRegOfficer, _dateAndAddressService, _lookupService)),
                EventSupportingDocuments = _supportingDocument.GetAll().Where(s => s.EventId == death.Id).Select(s => s.Id).ToList(),
                PaymentExamptionSupportingDocuments = _supportingDocument.GetAll().Where(s => s.PaymentExamptionId == death.PaymentExamption.Id).Select(s => s.Id).ToList(),

            };
        }
        public DeathArchiveDTO GetDeathPreviewArchive(DeathEvent death, string? BirthCertNo)
        {
            death.Event.DeathEventNavigation = death;
            return new DeathArchiveDTO()
            {
                Deceased = GetDeceased(death.Event.EventOwener),
                EventInfo = GetEventInfo(death.Event),
                Notification = GetNotification(death.DeathNotification),
                Registrar = GetRegistrar(death?.Event?.EventRegistrar),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(death.Event.CivilRegOfficer, _dateAndAddressService, _lookupService)),
                EventSupportingDocuments = death.Event.EventSupportingDocuments.Select(s => s.Id).ToList(),
                PaymentExamptionSupportingDocuments = death.Event.PaymentExamption.SupportingDocuments.Select(s => s.Id).ToList(),
            };
        }
    }
}