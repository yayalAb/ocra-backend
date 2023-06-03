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

namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public class ReturnAdoptionArchive : IReturnAdoptionArchive
    {
        IDateAndAddressService _DateAndAddressService;
        private readonly CustomDateConverter _convertor;
        // private readonly IReturnPerson _person;
        public ReturnAdoptionArchive(IDateAndAddressService DateAndAddressService)
        // , IReturnPerson person)
        {
            _DateAndAddressService = DateAndAddressService;
            _convertor = new CustomDateConverter();
            // _person = person;
        }

        private AdoptedChild GetChild(PersonalInfo adoptedChild)
        {
            AdoptedChild? child = CustomMapper.Mapper.Map<AdoptedChild>(ReturnPerson.GetPerson(adoptedChild, _DateAndAddressService));
            // child.ReasonAm = adoption.DivorceEvent.DivorceReason?.Value<string>("am");
            // child.ReasonOr = adoption.DivorceEvent.DivorceReason?.Value<string>("or");
            return child;
        }

        private Officer GetOfficer(PersonalInfo adoptionOfficer)
        {
            Officer? officer = CustomMapper.Mapper.Map<Officer>(ReturnPerson.GetPerson(adoptionOfficer, _DateAndAddressService));
            // Person? officer = ReturnPerson.GetPerson(adoptionOfficer, _DateAndAddressService);
            // officer.RegistrationMonthOr = new EthiopicDateTime(_convertor.getSplitted(adoption.EventRegDateEt).month, "or").month;
            // officer.RegistrationMonthAm = new EthiopicDateTime(_convertor.getSplitted(adoption.EventRegDateEt).month, "am").month;
            // officer.RegistrationDay = _convertor.getSplitted(adoption.EventRegDateEt).day.ToString();
            // officer.RegistrationYear = _convertor.getSplitted(adoption.EventRegDateEt).year.ToString();
            return officer;
        }

        private Person GetMother(PersonalInfo adoptiveMother)
        {
            Person? mother = ReturnPerson.GetPerson(adoptiveMother, _DateAndAddressService);
            return mother;
        }
        private Person GetFather(PersonalInfo adoptiveFather)
        {
            Person? father = ReturnPerson.GetPerson(adoptiveFather, _DateAndAddressService);
            return father;
        }
        private AdoptionInfo GetEventInfo(Event adoption)
        {
            AdoptionInfo adoptionInfo = CustomMapper.Mapper.Map<AdoptionInfo>(ReturnPerson.GetEventInfo(adoption, _DateAndAddressService));
            adoptionInfo.ReasonOr = adoption.AdoptionEvent.Reason?.Value<string>("or");
            adoptionInfo.ReasonAm = adoption.AdoptionEvent.Reason?.Value<string>("am");
            return adoptionInfo;
        }
        private CourtArchive GetCourt(CourtCase court)
        {
            (string am, string or)? courtAddress = (court.Court.AddressId == Guid.Empty
               || court.Court?.Address == null) ? null :
               _DateAndAddressService.addressFormat(court.Court.AddressId);
            return new CourtArchive
            {
                CourtNameOr = court.Court.Name?.Value<string>("or"),
                CourtNameAm = court.Court.Name?.Value<string>("am"),

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
            // (string am, string or)? address = (adoption.EventOwener?.BirthAddressId == Guid.Empty
            //    || adoption.EventOwener?.BirthAddressId == null) ? null :
            //    _DateAndAddressService.addressFormat(adoption.EventOwener.BirthAddressId);
            // (string[] am, string[] or) splitedAddress = _DateAndAddressService.SplitedAddress(address?.am, address?.or);

            var convertor = new CustomDateConverter();
            var CreatedAtEt = convertor.GregorianToEthiopic(DateTime.Now);

            // var mon=monthname.
            return new AdoptionArchiveDTO()
            {
                Child = GetChild(adoption.EventOwener),
                Mother = GetMother(adoption.AdoptionEvent.AdoptiveMother),
                Father = GetFather(adoption.AdoptionEvent.AdoptiveFather),
                Court = GetCourt(adoption.AdoptionEvent.CourtCase),
                EventInfo = GetEventInfo(adoption),
                CivilRegistrarOfficer = GetOfficer(adoption.CivilRegOfficer),

                // CertifcateId = adoption.CertificateId,
                // RegBookNo = adoption.RegBookNo,
                // BirthCertifcateId = (adoption.AdoptionEvent.BirthCertificateId == null || string.IsNullOrEmpty(adoption.AdoptionEvent.BirthCertificateId)) ? BirthCertNo : adoption.AdoptionEvent.BirthCertificateId,

            };
        }
    }
}