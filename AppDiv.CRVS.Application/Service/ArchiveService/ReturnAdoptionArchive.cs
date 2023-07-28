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
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public class ReturnAdoptionArchive : IReturnAdoptionArchive
    {
        IDateAndAddressService _DateAndAddressService;
        private readonly CustomDateConverter _convertor;
        private readonly ILookupFromId _lookupService;
        private readonly IPersonalInfoRepository _person;
        private readonly ISupportingDocumentRepository _supportingDocument;
        public ReturnAdoptionArchive(IDateAndAddressService DateAndAddressService,
                                    ILookupFromId lookupService,
                                    IPersonalInfoRepository person,
                                    ISupportingDocumentRepository supportingDocument)
        {
            _DateAndAddressService = DateAndAddressService;
            _lookupService = lookupService;
            _person = person;
            _supportingDocument = supportingDocument;
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
            adoptionInfo.BirthCertificateId = adoption.AdoptionEvent.BirthCertificateId;
            return adoptionInfo;
        }
        private AdoptionInfo GetEventInfoPreview(AdoptionEvent adoption)
        {
            AdoptionInfo adoptionInfo = CustomMapper.Mapper.Map<AdoptionInfo>(ReturnPerson.GetEventInfo(adoption.Event, _DateAndAddressService));
            adoptionInfo.ReasonOr = adoption?.Reason?.Value<string>("or");
            adoptionInfo.ReasonAm = adoption?.Reason?.Value<string>("am");
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

            var adoptionInfo = new AdoptionArchiveDTO()
            {
                Child = GetChild(adoption.EventOwener),
                Mother = GetMother(adoption.AdoptionEvent.AdoptiveMother),
                Father = GetFather(adoption.AdoptionEvent.AdoptiveFather),
                Court = GetCourt(adoption.AdoptionEvent.CourtCase),
                EventInfo = GetEventInfo(adoption),
                CivilRegistrarOfficer = GetOfficer(adoption.CivilRegOfficer),
                EventSupportingDocuments = _supportingDocument.GetAll().Where(s => s.EventId == adoption.Id)
                                                .ProjectTo<SupportingDocumentDTO>(CustomMapper.Mapper.ConfigurationProvider).ToList(),
            };
            adoptionInfo.PaymentExamptionSupportingDocuments = adoption?.PaymentExamption?.Id == null ? null
                    : _supportingDocument.GetAll().Where(s => s.PaymentExamptionId == adoption.PaymentExamption.Id)
                                    .ProjectTo<SupportingDocumentDTO>(CustomMapper.Mapper.ConfigurationProvider).ToList();
            return adoptionInfo;

        }
        public AdoptionArchiveDTO GetAdoptionPreviewArchive(AdoptionEvent adoption, string? BirthCertNo)
        {
            var child = adoption.Event.EventOwener == null ?
                                    _person.GetAll().Where(p => p.Id == adoption.Event.EventOwenerId)
                                                    .Include(m => m.NationalityLookup)
                                                    .Include(m => m.NationLookup)
                                                    .Include(m => m.ReligionLookup)
                                                    .Include(m => m.SexLookup)
                                                    .Include(m => m.BirthAddress)
                                                    .Include(m => m.ResidentAddress)
                                                    .FirstOrDefault() : adoption.Event.EventOwener;
            var mother = adoption.AdoptiveMother == null ?
                                    _person.GetAll().Where(p => p.Id == adoption.AdoptiveMotherId)
                                                    .Include(e => e.ResidentAddress)
                                                    .Include(e => e.BirthAddress)
                                                    .Include(e => e.MarraigeStatusLookup)
                                                    .Include(e => e.TypeOfWorkLookup)
                                                    .Include(e => e.NationalityLookup)
                                                    .Include(e => e.EducationalStatusLookup)
                                                    .Include(e => e.NationLookup)
                                                    .FirstOrDefault() : adoption.AdoptiveMother;
            var father = adoption.AdoptiveFather == null ?
                                    _person.GetAll().Where(p => p.Id == adoption.AdoptiveFatherId)
                                                    .Include(e => e.ResidentAddress)
                                                    .Include(e => e.BirthAddress)
                                                    .Include(e => e.MarraigeStatusLookup)
                                                    .Include(e => e.TypeOfWorkLookup)
                                                    .Include(e => e.NationalityLookup)
                                                    .Include(e => e.EducationalStatusLookup)
                                                    .Include(e => e.NationLookup)
                                                    .FirstOrDefault() : adoption.AdoptiveFather;
            var convertor = new CustomDateConverter();
            // var CreatedAtEt = convertor.GregorianToEthiopic(DateTime.Now);
            var adoptionArchive = new AdoptionArchiveDTO();
            adoption.Event.AdoptionEvent = adoption;
            // return new AdoptionArchiveDTO()
            // {
            adoptionArchive.Child = child == null ? null : GetChild(child);
            adoptionArchive.Mother = mother == null ? null : GetMother(mother);
            adoptionArchive.Father = father == null ? null : GetFather(father);
            adoptionArchive.Court = GetCourt(adoption.CourtCase);
            adoptionArchive.EventInfo = GetEventInfo(adoption.Event);
            adoptionArchive.EventSupportingDocuments = CustomMapper.Mapper.Map<IList<SupportingDocumentDTO>>(adoption.Event?.EventSupportingDocuments);
            adoptionArchive.PaymentExamptionSupportingDocuments = CustomMapper.Mapper.Map<IList<SupportingDocumentDTO>>(adoption?.Event?.PaymentExamption?.SupportingDocuments);
            if (adoption.Event.CivilRegOfficer == null && adoption.Event.CivilRegOfficerId != null)
            {
                adoption.Event.CivilRegOfficer = _person.GetAll().Where(p => p.Id == adoption.Event.CivilRegOfficerId).FirstOrDefault();
            }
            adoptionArchive.CivilRegistrarOfficer = adoption.Event.CivilRegOfficer == null ? null : GetOfficer(adoption.Event.CivilRegOfficer);

            // };
            return adoptionArchive;
        }
    }
}