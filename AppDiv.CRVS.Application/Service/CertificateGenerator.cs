using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Features.Certificates.Query;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Service
{
    public static class CertificateGenerator
    {
        public static Certificate GetCertificate(GenerateCertificateQuery request, (BirthEvent? birth, DeathEvent? death, AdoptionEvent? adoption, MarriageEvent? marriage, DivorceEvent? divorce) content)
        {
            // return GetBirthCertificate(content.birth);
            var certificate = new Certificate()
            {
                EventId = request.Id,
                // Content = JObject.FromObject(this.GetBirthCertificate(content.birth)),
                PrintCount = 1,
                Status = true,
                AuthenticationStatus = false,
                CertificateSerialNumber = request.CertificateSerialNumber
            };
            if (content.birth != null)
            {
                certificate.Content = JObject.FromObject(CertificateGenerator.GetBirthCertificate(content.birth));
            }
            if (content.death != null)
            {
                // certificate.Content = JObject.FromObject(new DeathCertificateDTO());
                certificate.Content = JObject.FromObject(CertificateGenerator.GetDeathCertificate(content.death));
            }
            if (content.adoption != null)
            {
                certificate.Content = JObject.FromObject(CertificateGenerator.GetAdoptionCertificate(content.adoption));
            }
            if (content.marriage != null)
            {
                certificate.Content = JObject.FromObject(CertificateGenerator.GetMarriageCertificate(content.marriage));
            }
            if (content.divorce != null)
            {
                certificate.Content = JObject.FromObject(CertificateGenerator.GetDivorceCertificate(content.divorce));
            }


            return certificate;

        }
        private static BirthCertificateDTO GetBirthCertificate(BirthEvent birth)
        {
            return new BirthCertificateDTO()
            {
                ChildFirstName = birth.Event.EventOwener?.FirstName,
                ChildMiddleName = birth.Event.EventOwener?.MiddleName,
                ChildLastName = birth.Event.EventOwener?.LastName,
                BirthDate = birth.Event.EventDate,
                BirthPlace = CustomMapper.Mapper.Map<AddressDTO>(birth.BirthPlace),
                ChildNationality = CustomMapper.Mapper.Map<LookupDTO>(birth.Event.EventOwener?.NationalityLookup),
                MotherFirstName = birth.Mother?.FirstName,
                MotherMiddleName = birth.Mother?.MiddleName,
                MotherLastName = birth.Mother?.LastName,
                MotherNationality = CustomMapper.Mapper.Map<LookupDTO>(birth.Mother?.NationalityLookup),
                FatherFirstName = birth.Father?.FirstName,
                FatherMiddleName = birth.Father?.MiddleName,
                FatherLastName = birth.Father?.LastName,
                FatherNationality = CustomMapper.Mapper.Map<LookupDTO>(birth.Father?.NationalityLookup),
                EventRegDate = birth.Event.EventRegDate,
                CivilRegOfficerFirstName = birth.Event.CivilRegOfficer?.FirstName,
                CivilRegOfficerMiddleName = birth.Event.CivilRegOfficer?.MiddleName,
                CivilRegOfficerLastName = birth.Event.CivilRegOfficer?.LastName

            };
        }

        private static AdoptionCertificateDTO GetAdoptionCertificate(AdoptionEvent adoption)
        {
            return new AdoptionCertificateDTO()
            {
                ChildFirstName = adoption.Event.EventOwener?.FirstName,
                ChildMiddleName = adoption.Event.EventOwener?.MiddleName,
                ChildLastName = adoption.Event.EventOwener?.LastName,
                BirthDate = adoption.Event.EventDate,
                BirthPlace = CustomMapper.Mapper.Map<AddressDTO>(adoption.Event.EventOwener?.BirthAddress),
                ChildNationality = CustomMapper.Mapper.Map<LookupDTO>(adoption.Event.EventOwener?.NationalityLookup),
                MotherFirstName = adoption.AdoptiveMother?.FirstName,
                MotherMiddleName = adoption.AdoptiveMother?.MiddleName,
                MotherLastName = adoption.AdoptiveMother?.LastName,
                MotherNationality = CustomMapper.Mapper.Map<LookupDTO>(adoption.AdoptiveMother?.NationalityLookup),
                FatherFirstName = adoption.AdoptiveFather?.FirstName,
                FatherMiddleName = adoption.AdoptiveFather?.MiddleName,
                FatherLastName = adoption.AdoptiveFather?.LastName,
                FatherNationality = CustomMapper.Mapper.Map<LookupDTO>(adoption.AdoptiveFather?.NationalityLookup),
                EventRegDate = adoption.Event.EventRegDate,
                CivilRegOfficerFirstName = adoption.Event.CivilRegOfficer?.FirstName,
                CivilRegOfficerMiddleName = adoption.Event.CivilRegOfficer?.MiddleName,
                CivilRegOfficerLastName = adoption.Event.CivilRegOfficer?.LastName

            };
        }

        private static MarriageCertificateDTO GetMarriageCertificate(MarriageEvent marriage)
        {
            return new MarriageCertificateDTO()
            {
                BrideFirstName = marriage.BrideInfo.FirstName,
                BrideMiddleName = marriage.BrideInfo.MiddleName,
                BrideLastName = marriage.BrideInfo.LastName,
                BrideNationality = CustomMapper.Mapper.Map<LookupDTO>(marriage.BrideInfo?.NationalityLookup),
                GroomFirstName = marriage.Event.EventOwener.FirstName,
                GroomMiddleName = marriage.Event.EventOwener.MiddleName,
                GroomLastName = marriage.Event.EventOwener.LastName,
                GroomNationality = CustomMapper.Mapper.Map<LookupDTO>(marriage.Event.EventOwener?.NationalityLookup),
                MarriageDate = marriage.Event.EventDate,
                MarriagePlace = CustomMapper.Mapper.Map<AddressDTO>(marriage.Event.EventAddress),
                EventRegDate = marriage.Event.EventRegDate,
                CivilRegOfficerFirstName = marriage.Event.CivilRegOfficer?.FirstName,
                CivilRegOfficerMiddleName = marriage.Event.CivilRegOfficer?.MiddleName,
                CivilRegOfficerLastName = marriage.Event.CivilRegOfficer?.LastName

            };
        }

        private static DivorceCertificateDTO GetDivorceCertificate(DivorceEvent Divorce)
        {
            return new DivorceCertificateDTO()
            {
                WifeFirstName = Divorce.DivorcedWife?.FirstName,
                WifeMiddleName = Divorce.DivorcedWife?.MiddleName,
                WifeLastName = Divorce.DivorcedWife?.LastName,
                WifeNationality = CustomMapper.Mapper.Map<LookupDTO>(Divorce.DivorcedWife?.NationalityLookup),
                HusbandFirstName = Divorce.Event.EventOwener?.FirstName,
                HusbandMiddleName = Divorce.Event.EventOwener?.MiddleName,
                HusbandLastName = Divorce.Event.EventOwener?.LastName,
                HusbandNationality = CustomMapper.Mapper.Map<LookupDTO>(Divorce.Event.EventOwener?.NationalityLookup),
                DivorceDate = Divorce.Event.EventDate,
                DivorcePlace = CustomMapper.Mapper.Map<AddressDTO>(Divorce.Event.EventAddress),
                EventRegDate = Divorce.Event.EventRegDate,
                CivilRegOfficerFirstName = Divorce.Event.CivilRegOfficer?.FirstName,
                CivilRegOfficerMiddleName = Divorce.Event.CivilRegOfficer?.MiddleName,
                CivilRegOfficerLastName = Divorce.Event.CivilRegOfficer?.LastName

            };
        }

        private static DeathCertificateDTO GetDeathCertificate(DeathEvent death)
        {
            return new DeathCertificateDTO()
            {
                FirstName = death.Event.EventOwener?.FirstName,
                MiddleName = death.Event.EventOwener?.MiddleName,
                LastName = death.Event.EventOwener?.LastName,
                Title = CustomMapper.Mapper.Map<LookupDTO>(death.Event.EventOwener?.TitleLookup),
                Gender = CustomMapper.Mapper.Map<LookupDTO>(death.Event.EventOwener?.SexLookup),
                BirthDate = death.Event.EventOwener?.BirthDate,
                DeathPlace = CustomMapper.Mapper.Map<AddressDTO>(death.Event.EventAddress),
                DeathDate = death.Event.EventDate,
                Nationality = CustomMapper.Mapper.Map<LookupDTO>(death.Event.EventOwener?.NationalityLookup),
                EventRegDate = death.Event.EventDate,
                CivilRegOfficerFirstName = death.Event.CivilRegOfficer?.FirstName,
                CivilRegOfficerMiddleName = death.Event.CivilRegOfficer?.MiddleName,
                CivilRegOfficerLastName = death.Event.CivilRegOfficer?.LastName

            };
        }
    }
}
