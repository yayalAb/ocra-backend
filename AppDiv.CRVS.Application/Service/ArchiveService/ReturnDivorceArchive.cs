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
using AppDiv.CRVS.Application.Mapper;

namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public class ReturnDivorceArchive : IReturnDivorceArchive
    {
        IDateAndAddressService _dateAndAddressService;
        private readonly CustomDateConverter convertor;
        private readonly ILookupFromId _lookupService;
        public ReturnDivorceArchive(IDateAndAddressService DateAndAddressService, ILookupFromId lookupService)
        {
            _lookupService = lookupService;
            _dateAndAddressService = DateAndAddressService;
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

            divorceInfo.DivorceReasonOr = divorce?.DivorceEvent?.DivorceReason?.Value<string>("or");
            divorceInfo.DivorceReasonAm = divorce?.DivorceEvent?.DivorceReason?.Value<string>("am");

            divorceInfo.Court = GetCourt(divorce?.DivorceEvent?.CourtCase);

            divorceInfo.NumberOfChildren = divorce?.DivorceEvent?.NumberOfChildren;

            return divorceInfo;
        }

        public DivorceArchiveDTO GetDivorceArchive(Event divorce, string? BirthCertNo)
        {

            return new DivorceArchiveDTO()
            {
                Husband = ReturnPerson.GetPerson(divorce.EventOwener, _dateAndAddressService, _lookupService),
                Wife = ReturnPerson.GetPerson(divorce.DivorceEvent.DivorcedWife, _dateAndAddressService, _lookupService),
                EventInfo = GetEventInfo(divorce),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(divorce.CivilRegOfficer, _dateAndAddressService, _lookupService)),

            };
        }
        public DivorceArchiveDTO GetDivorcePreviewArchive(DivorceEvent divorce, string? BirthCertNo)
        {

            return new DivorceArchiveDTO()
            {
                Husband = ReturnPerson.GetPerson(divorce.Event.EventOwener, _dateAndAddressService, _lookupService),
                Wife = ReturnPerson.GetPerson(divorce.DivorcedWife, _dateAndAddressService, _lookupService),
                EventInfo = GetEventInfo(divorce.Event),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(divorce.Event.CivilRegOfficer, _dateAndAddressService, _lookupService)),

            };
        }
    }
}