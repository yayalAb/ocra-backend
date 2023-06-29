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
using AutoMapper.QueryableExtensions;
using AppDiv.CRVS.Application.Contracts.DTOs;

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
                CauseOfDeath = death?.CauseOfDeathArray.Select(c => c?.Value<string>("reason")).ToList(),
                // CauseOfDeathTwo = (string?)death?.CauseOfDeathArray?.ElementAtOrDefault(1)?.Value<string>("reason"),
                // CauseOfDeathThree = (string?)death?.CauseOfDeathArray?.ElementAtOrDefault(2)?.Value<string>("reason"),
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
            deceasedInfo.TitileAm = deceased?.TitleLookup?.Value?.Value<string>("am") ?? _lookupService.GetLookupAm(deceased?.TitleLookupId);
            deceasedInfo.TitileOr = deceased?.TitleLookup?.Value?.Value<string>("or") ?? _lookupService.GetLookupOr(deceased?.TitleLookupId);
            return deceasedInfo;
        }
        public DeathArchiveDTO GetDeathArchive(Event death, string? BirthCertNo)
        {

            var deathInfo = new DeathArchiveDTO()
            {
                Deceased = GetDeceased(death.EventOwener),
                EventInfo = GetEventInfo(death),
                Notification = GetNotification(death.DeathEventNavigation.DeathNotification),
                Registrar = GetRegistrar(death?.EventRegistrar),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(death.CivilRegOfficer, _dateAndAddressService, _lookupService)),
                EventSupportingDocuments = _supportingDocument.GetAll()
                                                .Where(s => s.EventId == death.Id).Where(s => s.Type.ToLower() != "webcam")
                                                .ProjectTo<SupportingDocumentDTO>(CustomMapper.Mapper.ConfigurationProvider).ToList()
            };
            deathInfo.PaymentExamptionSupportingDocuments = death?.PaymentExamption?.Id == null ? null
                : _supportingDocument.GetAll().Where(s => s.PaymentExamptionId == death.PaymentExamption.Id)
                        .ProjectTo<SupportingDocumentDTO>(CustomMapper.Mapper.ConfigurationProvider).ToList();
            return deathInfo;
        }
        public DeathArchiveDTO GetDeathPreviewArchive(DeathEvent death, string? BirthCertNo)
        {
            death.Event.DeathEventNavigation = death;
            if (death.Event.CivilRegOfficer == null && death.Event.CivilRegOfficerId != null)
            {
                death.Event.CivilRegOfficer = _person.GetAll().Where(p => p.Id == death.Event.CivilRegOfficerId).FirstOrDefault();
            }
            return new DeathArchiveDTO()
            {
                Deceased = GetDeceased(death.Event.EventOwener),
                EventInfo = GetEventInfo(death.Event),
                Notification = GetNotification(death.DeathNotification),
                Registrar = GetRegistrar(death?.Event?.EventRegistrar),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(death.Event.CivilRegOfficer, _dateAndAddressService, _lookupService)),
                EventSupportingDocuments = CustomMapper.Mapper.Map<IList<SupportingDocumentDTO>>(death.Event?.EventSupportingDocuments),
                PaymentExamptionSupportingDocuments = CustomMapper.Mapper.Map<IList<SupportingDocumentDTO>>(death?.Event?.PaymentExamption?.SupportingDocuments),
            };

        }
    }
}