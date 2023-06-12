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

namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public class ReturnDivorceArchive : IReturnDivorceArchive
    {
        IDateAndAddressService _dateAndAddressService;
        private readonly CustomDateConverter convertor;
        private readonly ILookupFromId _lookupService;
        private readonly IPersonalInfoRepository _person;
        private readonly ISupportingDocumentRepository _supportingDocument;
        public ReturnDivorceArchive(IDateAndAddressService DateAndAddressService,
                                    ILookupFromId lookupService,
                                    IPersonalInfoRepository person,
                                    ISupportingDocumentRepository supportingDocument)
        {
            _dateAndAddressService = DateAndAddressService;
            _lookupService = lookupService;
            _supportingDocument = supportingDocument;
            _person = person;
            convertor = new CustomDateConverter();
        }

        private CourtArchive GetCourt(CourtCase court)
        {
            (string am, string or)? courtAddress = (court.Court.AddressId == Guid.Empty
               || court.Court.Address == null) ? null :
               _dateAndAddressService.addressFormat(court.Court.AddressId);
            return new CourtArchive
            {
                CourtNameOr = court?.Court?.Name?.Value<string>("or"),
                CourtNameAm = court?.Court?.Name?.Value<string>("am"),

                CourtAddressOr = courtAddress?.or,
                CourtAddressAm = courtAddress?.am,

                CourtConfirmationMonthOr = new EthiopicDateTime(convertor.getSplitted(court?.ConfirmedDateEt).month, "or").month,
                CourtConfirmationMonthAm = new EthiopicDateTime(convertor.getSplitted(court.ConfirmedDateEt).month, "am").month,
                CourtConfirmationDay = convertor.getSplitted(court?.ConfirmedDateEt).day.ToString(),
                CourtConfirmationYear = convertor.getSplitted(court?.ConfirmedDateEt).year.ToString(),

                CourtCaseNumber = court?.CourtCaseNumber
            };
        }

        private DivorceInfo GetEventInfo(Event divorce)
        {
            DivorceInfo divorceInfo = CustomMapper.Mapper.Map<DivorceInfo>(ReturnPerson.GetEventInfo(divorce, _dateAndAddressService));
            // marriageInfo.MarriageAddressOr = divorce.MarriageEvent.MarriageAddress.Value?.Value<string>("or");
            // marriageInfo.MarriageAddressAm = divorce.MarriageEvent.MarriageAddress.Value?.Value<string>("am");

            divorceInfo.MarriageMonthOr = new EthiopicDateTime(convertor.getSplitted(divorce?.DivorceEvent?.DateOfMarriageEt).month, "or")?.month;
            divorceInfo.MarriageMonthAm = new EthiopicDateTime(convertor.getSplitted(divorce?.DivorceEvent?.DateOfMarriageEt).month, "Am")?.month;
            divorceInfo.MarriageDay = convertor.getSplitted(divorce?.DivorceEvent?.DateOfMarriageEt).day.ToString();
            divorceInfo.MarriageYear = convertor.getSplitted(divorce?.DivorceEvent?.DateOfMarriageEt).year.ToString();

            divorceInfo.WifeBirthCertificateId = divorce?.DivorceEvent?.WifeBirthCertificateId;
            divorceInfo.HusbandBirthCertificateId = divorce?.DivorceEvent?.HusbandBirthCertificate;

            divorceInfo.DivorceReasonOr = divorce?.DivorceEvent?.DivorceReason?.Value<string>("or");
            divorceInfo.DivorceReasonAm = divorce?.DivorceEvent?.DivorceReason?.Value<string>("am");



            divorceInfo.NumberOfChildren = divorce?.DivorceEvent?.NumberOfChildren;

            return divorceInfo;
        }

        public DivorceArchiveDTO GetDivorceArchive(Event divorce, string? BirthCertNo)
        {

            var divorceInfo = new DivorceArchiveDTO()
            {
                Husband = ReturnPerson.GetPerson(divorce.EventOwener, _dateAndAddressService, _lookupService),
                Wife = ReturnPerson.GetPerson(divorce.DivorceEvent.DivorcedWife, _dateAndAddressService, _lookupService),
                EventInfo = GetEventInfo(divorce),
                Court = GetCourt(divorce?.DivorceEvent?.CourtCase),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(divorce.CivilRegOfficer, _dateAndAddressService, _lookupService)),
                EventSupportingDocuments = _supportingDocument.GetAll().Where(s => s.EventId == divorce.Id)
                                                .Where(s => s.Type.ToLower() != "webcam")
                                                .ProjectTo<SupportingDocumentDTO>(CustomMapper.Mapper.ConfigurationProvider).ToList(),
            };
            divorceInfo.PaymentExamptionSupportingDocuments = divorce?.PaymentExamption?.Id == null ? null
                : _supportingDocument.GetAll().Where(s => s.PaymentExamptionId == divorce.PaymentExamption.Id)
                            .ProjectTo<SupportingDocumentDTO>(CustomMapper.Mapper.ConfigurationProvider).ToList();
            return divorceInfo;

        }
        public DivorceArchiveDTO GetDivorcePreviewArchive(DivorceEvent divorce, string? BirthCertNo)
        {
            divorce.Event.DivorceEvent = divorce;
            if (divorce.Event.CivilRegOfficer == null && divorce.Event.CivilRegOfficerId != null)
            {
                divorce.Event.CivilRegOfficer = _person.GetAll().Where(p => p.Id == divorce.Event.CivilRegOfficerId).FirstOrDefault();
            }
            return new DivorceArchiveDTO()
            {
                Husband = ReturnPerson.GetPerson(divorce.Event.EventOwener, _dateAndAddressService, _lookupService),
                Wife = ReturnPerson.GetPerson(divorce.DivorcedWife, _dateAndAddressService, _lookupService),
                EventInfo = GetEventInfo(divorce.Event),
                Court = GetCourt(divorce?.CourtCase),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(divorce.Event.CivilRegOfficer, _dateAndAddressService, _lookupService)),
                EventSupportingDocuments = CustomMapper.Mapper.Map<IList<SupportingDocumentDTO>>(divorce?.Event?.EventSupportingDocuments),
                PaymentExamptionSupportingDocuments = CustomMapper.Mapper.Map<IList<SupportingDocumentDTO>>(divorce?.Event?.PaymentExamption?.SupportingDocuments),

            };
        }
    }
}