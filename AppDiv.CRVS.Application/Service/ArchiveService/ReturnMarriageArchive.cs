using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive.MarriageArchive;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;
using AppDiv.CRVS.Application.Interfaces.Archive;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Interfaces.Persistence;

namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public class ReturnMarriageArchive : IReturnMarriageArchive
    {
        IDateAndAddressService _dateAndAddressService;
        private readonly ILookupFromId _lookupService;
        private readonly IPersonalInfoRepository _person;
        private readonly ISupportingDocumentRepository _supportingDocument;
        public ReturnMarriageArchive(IDateAndAddressService DateAndAddressService,
                                    ILookupFromId lookupService,
                                    IPersonalInfoRepository person,
                                    ISupportingDocumentRepository supportingDocument)
        {
            _lookupService = lookupService;
            _person = person;
            _supportingDocument = supportingDocument;
            _dateAndAddressService = DateAndAddressService;
        }

        private MarriageInfo GetEventInfo(Event marriage)
        {
            MarriageInfo marriageInfo = CustomMapper.Mapper.Map<MarriageInfo>(ReturnPerson.GetEventInfo(marriage, _dateAndAddressService));

            marriageInfo.BrideBirthCertificateId = marriage.MarriageEvent.BirthCertificateBrideId;
            marriageInfo.GroomBirthCertificateId = marriage.MarriageEvent.BirthCertificateGroomId;

            marriageInfo.MarriageTypeOr = marriage.MarriageEvent.MarriageType.Value?.Value<string>("or") ?? _lookupService.GetLookupOr(marriage.MarriageEvent.MarriageTypeId);
            marriageInfo.MarriageTypeAm = marriage.MarriageEvent.MarriageType.Value?.Value<string>("am") ?? _lookupService.GetLookupAm(marriage.MarriageEvent.MarriageTypeId);

            foreach (var w in marriage.MarriageEvent.Witnesses)
            {
                if (w.WitnessFor.ToLower() == "bride")
                {
                    var brideWitness = CustomMapper.Mapper.Map<WitnessArchive>
                                                (ReturnPerson.GetPerson(w.WitnessPersonalInfo, _dateAndAddressService, _lookupService));
                    marriageInfo?.BrideWitnesses?.Add(brideWitness);
                }
                if (w.WitnessFor.ToLower() == "groom")
                {
                    var groomWitness = CustomMapper.Mapper.Map<WitnessArchive>
                                                (ReturnPerson.GetPerson(w.WitnessPersonalInfo, _dateAndAddressService, _lookupService));
                    marriageInfo?.GroomWitnesses?.Add(groomWitness);
                }
            }
            return marriageInfo;
        }

        public MarriageArchiveDTO GetMarriageArchive(Event marriage, string? BirthCertNo)
        {
            var marriageInfo = new MarriageArchiveDTO()
            {
                Groom = ReturnPerson.GetPerson(marriage.EventOwener, _dateAndAddressService, _lookupService),
                Bride = ReturnPerson.GetPerson(marriage.MarriageEvent.BrideInfo, _dateAndAddressService, _lookupService),
                EventInfo = GetEventInfo(marriage),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(marriage.CivilRegOfficer, _dateAndAddressService, _lookupService)),
                EventSupportingDocuments = _supportingDocument.GetAll().Where(s => s.EventId == marriage.Id)
                                                .Where(s => s.Type.ToLower() != "webcam").Select(s => s.Id).ToList(),

            };
            marriageInfo.PaymentExamptionSupportingDocuments = marriage?.PaymentExamption?.Id == null ? null
                : _supportingDocument.GetAll().Where(s => s.PaymentExamptionId == marriage.PaymentExamption.Id).Select(s => s.Id).ToList();
            return marriageInfo;

        }
        public MarriageArchiveDTO GetMarriagePreviewArchive(MarriageEvent marriage, string? BirthCertNo)
        {
            marriage.Event.MarriageEvent = marriage;
            if (marriage.Event.CivilRegOfficer == null && marriage.Event.CivilRegOfficerId != null)
            {
                marriage.Event.CivilRegOfficer = _person.GetAll().Where(p => p.Id == marriage.Event.CivilRegOfficerId).FirstOrDefault();
            }
            return new MarriageArchiveDTO()
            {
                Groom = ReturnPerson.GetPerson(marriage.Event.EventOwener, _dateAndAddressService, _lookupService),
                Bride = ReturnPerson.GetPerson(marriage.BrideInfo, _dateAndAddressService, _lookupService),
                EventInfo = GetEventInfo(marriage.Event),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(marriage.Event.CivilRegOfficer, _dateAndAddressService, _lookupService)),
                EventSupportingDocuments = marriage.Event.EventSupportingDocuments.Select(s => s.Id).ToList(),
                PaymentExamptionSupportingDocuments = marriage?.Event?.PaymentExamption?.SupportingDocuments?.Select(s => s.Id).ToList(),

            };
        }
    }
}