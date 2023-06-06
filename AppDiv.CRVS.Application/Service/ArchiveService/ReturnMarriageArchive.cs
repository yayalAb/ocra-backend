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

namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public class ReturnMarriageArchive : IReturnMarriageArchive
    {
        IDateAndAddressService _dateAndAddressService;
        private readonly ILookupFromId _lookupService;
        public ReturnMarriageArchive(IDateAndAddressService DateAndAddressService, ILookupFromId lookupService)
        {
            _lookupService = lookupService;
            _dateAndAddressService = DateAndAddressService;
        }

        private MarriageInfo GetEventInfo(Event marriage)
        {
            MarriageInfo marriageInfo = CustomMapper.Mapper.Map<MarriageInfo>(ReturnPerson.GetEventInfo(marriage, _dateAndAddressService));

            marriageInfo.BrideBirthCertificateId = marriage.MarriageEvent.BirthCertificateBrideId;
            marriageInfo.GroomBirthCertificateId = marriage.MarriageEvent.BirthCertificateGroomId;

            marriageInfo.MarriageTypeOr = marriage.MarriageEvent.MarriageType.Value?.Value<string>("or");
            marriageInfo.MarriageTypeAm = marriage.MarriageEvent.MarriageType.Value?.Value<string>("am");

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
            // (string am, string or)? address = (marriage?.EventAddressId == Guid.Empty
            //    || marriage?.EventAddressId == null) ? null :
            //    _dateAndAddressService.addressFormat(marriage.EventAddressId);

            // var convertor = new CustomDateConverter();
            // var CreatedAtEt = convertor.GregorianToEthiopic(marriage.CreatedAt);

            // (string[] am, string[] or) splitedAddress = _dateAndAddressService.SplitedAddress(address?.am, address?.or);
            return new MarriageArchiveDTO()
            {
                Groom = ReturnPerson.GetPerson(marriage.EventOwener, _dateAndAddressService, _lookupService),
                Bride = ReturnPerson.GetPerson(marriage.MarriageEvent.BrideInfo, _dateAndAddressService, _lookupService),
                EventInfo = GetEventInfo(marriage),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(marriage.CivilRegOfficer, _dateAndAddressService, _lookupService)),
                // CertifcateId = marriage?.CertificateId,
                // BirthCertificateGroomId = marriage.MarriageEvent.BirthCertificateGroomId,
                // BirthCertificateBrideId = marriage.MarriageEvent.BirthCertificateBrideId,
                // RegBookNo = marriage.RegBookNo,
                // BrideBirthCertifcateId = marriage.BirthCertificateBrideId,

            };
        }
        public MarriageArchiveDTO GetMarriagePreviewArchive(MarriageEvent marriage, string? BirthCertNo)
        {
            return new MarriageArchiveDTO()
            {
                Groom = ReturnPerson.GetPerson(marriage.Event.EventOwener, _dateAndAddressService, _lookupService),
                Bride = ReturnPerson.GetPerson(marriage.BrideInfo, _dateAndAddressService, _lookupService),
                EventInfo = GetEventInfo(marriage.Event),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(marriage.Event.CivilRegOfficer, _dateAndAddressService, _lookupService)),
                // CertifcateId = marriage?.CertificateId,
                // BirthCertificateGroomId = marriage.MarriageEvent.BirthCertificateGroomId,
                // BirthCertificateBrideId = marriage.MarriageEvent.BirthCertificateBrideId,
                // RegBookNo = marriage.RegBookNo,
                // BrideBirthCertifcateId = marriage.BirthCertificateBrideId,

            };
        }
    }
}