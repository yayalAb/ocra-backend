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
using Org.BouncyCastle.Asn1.Cms;
using System.ComponentModel;
using Microsoft.Extensions.Localization;

namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public class ReturnDeathArchive : IReturnDeathArchive
    {
        IDateAndAddressService _dateAndAddressService;
        private readonly ILookupFromId _lookupService;
        private readonly IPersonalInfoRepository _person;
        private readonly ISupportingDocumentRepository _supportingDocument;
        private readonly IReportRepostory _reportRepostory;
        public ReturnDeathArchive(IDateAndAddressService DateAndAddressService,
                                ILookupFromId lookupService,
                                IPersonalInfoRepository person,
                                ISupportingDocumentRepository supportingDocument,
                                IReportRepostory reportRepostory)
        {
            _dateAndAddressService = DateAndAddressService;
            _lookupService = lookupService;
            _supportingDocument = supportingDocument;
            _person = person;
            _reportRepostory=reportRepostory;
        }
        private DeathInfo GetEventInfo(Event death)
        {
             var EventAddress=  _reportRepostory.ReturnAddress(death?.EventAddressId.ToString()).Result;
            JArray EventAddressjsonObject = JArray.FromObject(EventAddress);
            FormatedAddressDto EventAddressResponse = EventAddressjsonObject.ToObject<List<FormatedAddressDto>>().FirstOrDefault();
            bool isCityAdmin=_dateAndAddressService.IsCityAdmin(death?.EventAddressId);
            DeathInfo deathInfo = CustomMapper.Mapper.Map<DeathInfo>(ReturnPerson.GetEventInfo(death, _dateAndAddressService,_reportRepostory));
            deathInfo.BirthCertificateId = death?.DeathEventNavigation?.BirthCertificateId;
            deathInfo.PlaceOfFuneralAm = death?.DeathEventNavigation?.PlaceOfFuneral?.Value<string>("am");
            deathInfo.PlaceOfFuneralOr = death?.DeathEventNavigation?.PlaceOfFuneral?.Value<string>("or");
            var duringDeath = _lookupService.GetLookup(death?.DeathEventNavigation?.DuringDeathId);
            deathInfo.DuringDeathAm = death?.DeathEventNavigation?.DuringDeathLookup?.Value?.Value<string>("am") ?? duringDeath?.Value?.Value<string>("am");
            deathInfo.DuringDeathOr = death?.DeathEventNavigation?.DuringDeathLookup?.Value?.Value<string>("or") ?? duringDeath?.Value?.Value<string>("or");
            var facilityType = _lookupService.GetLookup(death?.DeathEventNavigation?.FacilityTypeLookupId);
            deathInfo.FacilityTypeAm = death?.DeathEventNavigation?.FacilityTypeLookup?.Value?.Value<string>("am") ?? facilityType?.Value?.Value<string>("am");
            deathInfo.FacilityTypeOr = death?.DeathEventNavigation?.FacilityTypeLookup?.Value?.Value<string>("or") ?? facilityType?.Value?.Value<string>("or");
            deathInfo.EventCountryOr = EventAddressResponse?.CountryOr;
            deathInfo.EventCountryAm = EventAddressResponse?.CountryAm;
            deathInfo.EventRegionOr = EventAddressResponse?.RegionOr;
            deathInfo.EventRegionAm = EventAddressResponse?.RegionAm;
            deathInfo.EventZoneOr =!isCityAdmin?EventAddressResponse?.ZoneOr:null;
            deathInfo.EventZoneAm =!isCityAdmin? EventAddressResponse?.ZoneAm:null;
            deathInfo.EventSubcityOr =isCityAdmin?EventAddressResponse?.WoredaOr:null;
            deathInfo.EventSubcityAm =isCityAdmin? EventAddressResponse?.WoredaAm:null;
            deathInfo.EventWoredaOr = EventAddressResponse?.WoredaOr;
            deathInfo.EventWoredaAm = EventAddressResponse?.WoredaAm;
            deathInfo.EventKebeleOr = EventAddressResponse?.KebeleOr;
            deathInfo.EventKebeleAm = EventAddressResponse?.KebeleAm;
            return deathInfo;

        }
        private DeathNotificationArchive GetNotification(DeathNotification death)
        {
            // var causeOfDeath = JArray.Parse(death?.CauseOfDeath);
            var couseOfDeathInfoType = _lookupService.GetLookup(death?.CauseOfDeathInfoTypeLookupId);
            return new DeathNotificationArchive
            {
                CauseOfDeath = death?.CauseOfDeathArray.Select(c => c?.Value<string>("reason")).ToList(),
                // CauseOfDeathTwo = (string?)death?.CauseOfDeathArray?.ElementAtOrDefault(1)?.Value<string>("reason"),
                // CauseOfDeathThree = (string?)death?.CauseOfDeathArray?.ElementAtOrDefault(2)?.Value<string>("reason"),
                CauseOfDeathInfoTypeOr = death?.CauseOfDeathInfoTypeLookup?.Value?.Value<string>("or") ?? couseOfDeathInfoType?.Value?.Value<string>("or"),
                CauseOfDeathInfoTypeAm = death?.CauseOfDeathInfoTypeLookup?.Value?.Value<string>("am") ?? couseOfDeathInfoType?.Value?.Value<string>("am"),
                DeathNotificationSerialNumber = death?.DeathNotificationSerialNumber,
            };
        }

        private RegistrarArchive GetRegistrar(Registrar reg, bool IsCoorection=false)
        {
            if (reg is null) return new RegistrarArchive();
            RegistrarArchive regInfo = CustomMapper.Mapper.Map<RegistrarArchive>(ReturnPerson.GetPerson(reg?.RegistrarInfo, _dateAndAddressService, _lookupService,_reportRepostory));
            var relationship = _lookupService.GetLookup(reg?.RelationshipLookupId);
            regInfo.RelationShipOr = reg?.RelationshipLookup?.Value?.Value<string>("or") ?? relationship?.Value?.Value<string>("or");
            regInfo.RelationShipAm = reg?.RelationshipLookup?.Value?.Value<string>("am") ?? relationship?.Value?.Value<string>("am");
            return regInfo;
        }
        private DeceasedPerson GetDeceased(PersonalInfo deceased, bool IsCorrection)
        {
            if(deceased is null) return new DeceasedPerson();
            DeceasedPerson deceasedInfo = CustomMapper.Mapper.Map<DeceasedPerson>(ReturnPerson.GetPerson(deceased, _dateAndAddressService, _lookupService,_reportRepostory,IsCorrection));
            var title = _lookupService.GetLookup(deceased?.TitleLookupId);
            deceasedInfo.TitileAm = deceased?.TitleLookup?.Value?.Value<string>("am") ?? title?.Value?.Value<string>("am");
            deceasedInfo.TitileOr = deceased?.TitleLookup?.Value?.Value<string>("or") ?? title?.Value?.Value<string>("or");
            return deceasedInfo;
        }
        public DeathArchiveDTO GetDeathArchive(Event death, string? BirthCertNo, bool IsCorrection=false)
        {

            var deathInfo = new DeathArchiveDTO()
            {
                Deceased = GetDeceased(death.EventOwener,IsCorrection),
                EventInfo = GetEventInfo(death),
                Notification = GetNotification(death.DeathEventNavigation.DeathNotification),
                Registrar = GetRegistrar(death?.EventRegistrar,IsCorrection),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(death.CivilRegOfficer, _dateAndAddressService, _lookupService,_reportRepostory,IsCorrection)),
                EventSupportingDocuments = _supportingDocument.GetAll()
                                                .Where(s => s.EventId == death.Id)
                                                .ProjectTo<SupportingDocumentDTO>(CustomMapper.Mapper.ConfigurationProvider).ToList()
            };
           
            if(death.EventDate!=null&&death.EventOwener.BirthDate!=null){
                DateTime BirthDate=death.EventOwener.BirthDate ?? DateTime.Now;
                TimeSpan timestamp=death.EventDate-BirthDate;
                long seconds=(long)timestamp.TotalDays;
                if((long)timestamp.TotalDays / 365==0){
                       if((long)timestamp.TotalDays /30==0){
                           deathInfo.Deceased.Age = ((long)timestamp.TotalDays).ToString()+" Day";
                       }else{
                            deathInfo.Deceased.Age = ((long)timestamp.TotalDays /30).ToString()+" Month";
                       }
                        
                }else{
                    deathInfo.Deceased.Age=((long)timestamp.TotalDays / 365 ).ToString();
                }
            }
            deathInfo.PaymentExamptionSupportingDocuments = death?.PaymentExamption?.Id == null ? null
                : _supportingDocument.GetAll().Where(s => s.PaymentExamptionId == death.PaymentExamption.Id)
                        .ProjectTo<SupportingDocumentDTO>(CustomMapper.Mapper.ConfigurationProvider).ToList();
            return deathInfo;
        }
        public DeathArchiveDTO GetDeathPreviewArchive(DeathEvent death, string? BirthCertNo, bool IsCorrection = true)
        {
            death.Event.DeathEventNavigation = death;
            if (death.Event.CivilRegOfficer == null && death.Event.CivilRegOfficerId != null)
            {
                death.Event.CivilRegOfficer = _person.GetAll().Where(p => p.Id == death.Event.CivilRegOfficerId).FirstOrDefault();
            }
            return new DeathArchiveDTO()
            {
                Deceased = GetDeceased(death.Event.EventOwener,IsCorrection),
                EventInfo = GetEventInfo(death.Event),
                Notification = GetNotification(death.DeathNotification),
                Registrar = GetRegistrar(death?.Event?.EventRegistrar,IsCorrection),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(death.Event.CivilRegOfficer, _dateAndAddressService, _lookupService,_reportRepostory)),
                EventSupportingDocuments = CustomMapper.Mapper.Map<IList<SupportingDocumentDTO>>(death.Event?.EventSupportingDocuments),
                PaymentExamptionSupportingDocuments = CustomMapper.Mapper.Map<IList<SupportingDocumentDTO>>(death?.Event?.PaymentExamption?.SupportingDocuments),
            };

        }
    }
}