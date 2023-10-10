using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive.DivorceArchive;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;
using AppDiv.CRVS.Application.Interfaces.Archive;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AutoMapper.QueryableExtensions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public class ReturnDivorceArchive : IReturnDivorceArchive
    {
        IDateAndAddressService _dateAndAddressService;
        private readonly CustomDateConverter convertor;
        private readonly ILookupFromId _lookupService;
        private readonly IPersonalInfoRepository _person;
        private readonly ISupportingDocumentRepository _supportingDocument;
        private readonly IReportRepostory _reportRepostory;
        public ReturnDivorceArchive(IDateAndAddressService DateAndAddressService,
                                    ILookupFromId lookupService,
                                    IPersonalInfoRepository person,
                                    ISupportingDocumentRepository supportingDocument,
                                    IReportRepostory reportRepostory)
        {
            _dateAndAddressService = DateAndAddressService;
            _lookupService = lookupService;
            _supportingDocument = supportingDocument;
            _person = person;
            convertor = new CustomDateConverter();
            _reportRepostory=reportRepostory;
        }

        private CourtArchive GetCourt(CourtCase court)
        {
            var EventAddress=  _reportRepostory.ReturnAddress(court?.Court?.AddressId.ToString()).Result;
            JArray EventAddressjsonObject = JArray.FromObject(EventAddress);
            FormatedAddressDto EventAddressResponse = EventAddressjsonObject.ToObject<List<FormatedAddressDto>>().FirstOrDefault();
            
            if (court is null) return new CourtArchive();
            (string am, string or)? courtAddress = _dateAndAddressService.stringAddress(EventAddressResponse);
            return new CourtArchive
            {
                CourtNameOr = court?.Court?.Name?.Value<string>("or"),
                CourtNameAm = court?.Court?.Name?.Value<string>("am"),

                CourtAddressOr = courtAddress?.or,
                CourtAddressAm = courtAddress?.am,

                CourtConfirmationMonthOr = new EthiopicDateTime(convertor.getSplitted(court?.ConfirmedDateEt).month, "or").month,
                CourtConfirmationMonthAm = new EthiopicDateTime(convertor.getSplitted(court.ConfirmedDateEt).month, "am").month,
                CourtConfirmationDay = convertor.getSplitted(court?.ConfirmedDateEt).day.ToString("D2"),
                CourtConfirmationYear = convertor.getSplitted(court?.ConfirmedDateEt).year.ToString(),

                CourtCaseNumber = court?.CourtCaseNumber
            };
        }

        private DivorceInfo GetEventInfo(Event divorce)
        {
            DivorceInfo divorceInfo = CustomMapper.Mapper.Map<DivorceInfo>(ReturnPerson.GetEventInfo(divorce, _dateAndAddressService,_reportRepostory));
            var EventAddress=  _reportRepostory.ReturnAddress(divorce?.EventAddressId.ToString()).Result;
            JArray EventAddressjsonObject = JArray.FromObject(EventAddress);
            FormatedAddressDto EventAddressResponse = EventAddressjsonObject.ToObject<List<FormatedAddressDto>>().FirstOrDefault();
            (string am, string or)? marriageAddress = _dateAndAddressService.stringAddress(EventAddressResponse);
           
            divorceInfo.MarriageAddressOr = marriageAddress?.or;
            divorceInfo.MarriageAddressAm = marriageAddress?.am;
            divorceInfo.MarriageMonthOr = new EthiopicDateTime(convertor.getSplitted(divorce?.DivorceEvent?.DateOfMarriageEt).month, "or")?.month;
            divorceInfo.MarriageMonthAm = new EthiopicDateTime(convertor.getSplitted(divorce?.DivorceEvent?.DateOfMarriageEt).month, "Am")?.month;
            divorceInfo.MarriageDay = convertor.getSplitted(divorce?.DivorceEvent?.DateOfMarriageEt).day.ToString("D2");
            divorceInfo.MarriageYear = convertor.getSplitted(divorce?.DivorceEvent?.DateOfMarriageEt).year.ToString();

            divorceInfo.WifeBirthCertificateId = divorce?.DivorceEvent?.WifeBirthCertificateId;
            divorceInfo.HusbandBirthCertificateId = divorce?.DivorceEvent?.HusbandBirthCertificate;

            divorceInfo.DivorceReasonOr = divorce?.DivorceEvent?.DivorceReason?.Value<string>("or");
            divorceInfo.DivorceReasonAm = divorce?.DivorceEvent?.DivorceReason?.Value<string>("am");
            divorceInfo.NumberOfChildren = divorce?.DivorceEvent?.NumberOfChildren;

            return divorceInfo;
        }

        public DivorceArchiveDTO GetDivorceArchive(Event divorce, string? BirthCertNo, bool IsCorrection=false)
        {

            var divorceInfo = new DivorceArchiveDTO()
            {
                Husband = ReturnPerson.GetPerson(divorce.EventOwener, _dateAndAddressService, _lookupService,_reportRepostory,IsCorrection),
                Wife = ReturnPerson.GetPerson(divorce.DivorceEvent.DivorcedWife, _dateAndAddressService, _lookupService,_reportRepostory,IsCorrection),
                EventInfo = GetEventInfo(divorce),
                Court = GetCourt(divorce?.DivorceEvent?.CourtCase),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(divorce.CivilRegOfficer, _dateAndAddressService, _lookupService,_reportRepostory,IsCorrection)),
                EventSupportingDocuments = _supportingDocument.GetAll().Where(s => s.EventId == divorce.Id)
                                                .ProjectTo<SupportingDocumentDTO>(CustomMapper.Mapper.ConfigurationProvider).ToList(),
            };
            divorceInfo.PaymentExamptionSupportingDocuments = divorce?.PaymentExamption?.Id == null ? null
                : _supportingDocument.GetAll().Where(s => s.PaymentExamptionId == divorce.PaymentExamption.Id)
                            .ProjectTo<SupportingDocumentDTO>(CustomMapper.Mapper.ConfigurationProvider).ToList();
            return divorceInfo;

        }
        public DivorceArchiveDTO GetDivorcePreviewArchive(DivorceEvent divorce, string? BirthCertNo, bool IsCorrection=true)
        {
            divorce.Event.DivorceEvent = divorce;
            if (divorce.Event.CivilRegOfficer == null && divorce.Event.CivilRegOfficerId != null)
            {
                divorce.Event.CivilRegOfficer = _person.GetAll().Where(p => p.Id == divorce.Event.CivilRegOfficerId).FirstOrDefault();
            }
            return new DivorceArchiveDTO()
            {
                Husband = ReturnPerson.GetPerson(divorce.Event.EventOwener, _dateAndAddressService, _lookupService,_reportRepostory,IsCorrection),
                Wife = ReturnPerson.GetPerson(divorce.DivorcedWife, _dateAndAddressService, _lookupService,_reportRepostory,IsCorrection),
                EventInfo = GetEventInfo(divorce.Event),
                Court = GetCourt(divorce?.CourtCase),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(divorce.Event.CivilRegOfficer, _dateAndAddressService, _lookupService,_reportRepostory,IsCorrection)),
                EventSupportingDocuments = CustomMapper.Mapper.Map<IList<SupportingDocumentDTO>>(divorce?.Event?.EventSupportingDocuments),
                PaymentExamptionSupportingDocuments = CustomMapper.Mapper.Map<IList<SupportingDocumentDTO>>(divorce?.Event?.PaymentExamption?.SupportingDocuments),

            };
        }
    }
}