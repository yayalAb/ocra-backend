using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;
using AppDiv.CRVS.Application.Interfaces.Archive;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive.AdoptionArchive;
using AppDiv.CRVS.Application.Interfaces.Persistence;
namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public class ReturnAdoptionArchive : IReturnAdoptionArchive
    {
        IDateAndAddressService _DateAndAddressService;
        private readonly CustomDateConverter _convertor;
        private readonly ILookupFromId _lookupService;
        private readonly IPersonalInfoRepository _person;
        public ReturnAdoptionArchive(IDateAndAddressService DateAndAddressService, ILookupFromId lookupService, IPersonalInfoRepository person)
        {
            _DateAndAddressService = DateAndAddressService;
            _lookupService = lookupService;
            _person = person;
            _convertor = new CustomDateConverter();
        }

        private AdoptedChild GetChild(PersonalInfo adoptedChild)
        {
            AdoptedChild? child = CustomMapper.Mapper.Map<AdoptedChild>(ReturnPerson.GetPerson(adoptedChild, _DateAndAddressService, _lookupService));
            return child;
        }

        private Officer GetOfficer(PersonalInfo adoptionOfficer)
        {
            Officer? officer = CustomMapper.Mapper.Map<Officer>(ReturnPerson.GetPerson(adoptionOfficer, _DateAndAddressService, _lookupService));
            return officer;
        }

        private Person GetMother(PersonalInfo adoptiveMother)
        {
            Person? mother = ReturnPerson.GetPerson(adoptiveMother, _DateAndAddressService, _lookupService);
            return mother;
        }
        private Person GetFather(PersonalInfo adoptiveFather)
        {
            Person? father = ReturnPerson.GetPerson(adoptiveFather, _DateAndAddressService, _lookupService);
            return father;
        }
        private AdoptionInfo GetEventInfo(Event adoption)
        {
            AdoptionInfo adoptionInfo = CustomMapper.Mapper.Map<AdoptionInfo>(ReturnPerson.GetEventInfo(adoption, _DateAndAddressService));
            adoptionInfo.ReasonOr = adoption.AdoptionEvent.Reason?.Value<string>("or");
            adoptionInfo.ReasonAm = adoption.AdoptionEvent.Reason?.Value<string>("am");
            return adoptionInfo;
        }
        private AdoptionInfo GetEventInfoPreview(AdoptionEvent adoption)
        {
            AdoptionInfo adoptionInfo = CustomMapper.Mapper.Map<AdoptionInfo>(ReturnPerson.GetEventInfo(adoption.Event, _DateAndAddressService));
            adoptionInfo.ReasonOr = adoption.Reason?.Value<string>("or");
            adoptionInfo.ReasonAm = adoption.Reason?.Value<string>("am");
            return adoptionInfo;
        }
        private CourtArchive GetCourt(CourtCase court)
        {
            (string am, string or)? courtAddress = (court.Court.AddressId == Guid.Empty
               || court.Court?.Address == null) ? null :
               _DateAndAddressService.addressFormat(court.Court.AddressId);
            return new CourtArchive
            {
                CourtNameOr = court?.Court?.Name?.Value<string>("or"),
                CourtNameAm = court?.Court?.Name?.Value<string>("am"),

                CourtAddressOr = courtAddress?.or,
                CourtAddressAm = courtAddress?.am,

                CourtConfirmationMonthOr = new EthiopicDateTime(_convertor.getSplitted(court.ConfirmedDateEt).month, "or").month,
                CourtConfirmationMonthAm = new EthiopicDateTime(_convertor.getSplitted(court.ConfirmedDateEt).month, "am").month,
                CourtConfirmationDay = _convertor.getSplitted(court?.ConfirmedDateEt).day.ToString(),
                CourtConfirmationYear = _convertor.getSplitted(court?.ConfirmedDateEt).year.ToString(),

                CourtCaseNumber = court.CourtCaseNumber,
            };
        }
        public AdoptionArchiveDTO GetAdoptionArchive(Event adoption, string? BirthCertNo)
        {
            var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(DateTime.Now);

            return new AdoptionArchiveDTO()
            {
                Child = GetChild(adoption.EventOwener),
                Mother = GetMother(adoption.AdoptionEvent.AdoptiveMother),
                Father = GetFather(adoption.AdoptionEvent.AdoptiveFather),
                Court = GetCourt(adoption.AdoptionEvent.CourtCase),
                EventInfo = GetEventInfo(adoption),
                CivilRegistrarOfficer = GetOfficer(adoption.CivilRegOfficer),
            };
        }
        public AdoptionArchiveDTO GetAdoptionPreviewArchive(AdoptionEvent adoption, string? BirthCertNo)
        {
            var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(DateTime.Now);
            var adoptionArchive = new AdoptionArchiveDTO();
            // return new AdoptionArchiveDTO()
            // {
            adoptionArchive.Child = GetChild(adoption.Event.EventOwener);
            adoptionArchive.Mother = GetMother(adoption.AdoptiveMother);
            adoptionArchive.Father = GetFather(adoption.AdoptiveFather);
            adoptionArchive.Court = GetCourt(adoption.CourtCase);
            // adoptionArchive.EventInfo = GetEventInfoPreview(adoption);
            // if (adoption.Event.CivilRegOfficer == null)
            // {
            //     adoption.Event.CivilRegOfficer = _person.GetAll().Where(p => p.Id == adoption.Event.CivilRegOfficerId).FirstOrDefault();
            // }
            // adoptionArchive.CivilRegistrarOfficer = GetOfficer(adoption.Event.CivilRegOfficer);

            // };
            return adoptionArchive;
        }
    }
}