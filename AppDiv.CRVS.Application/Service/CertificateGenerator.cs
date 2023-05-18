using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Features.Certificates.Query;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Application.Service
{
    public class CertificateGenerator : ICertificateGenerator
    {
        IDateAndAddressService _DateAndAddressService;
        private readonly ILogger<CertificateGenerator> _Ilogger;
        public CertificateGenerator(IDateAndAddressService DateAndAddressService, ILogger<CertificateGenerator> Ilogger)
        {
            _DateAndAddressService = DateAndAddressService;
            _Ilogger = Ilogger;
        }

        public Certificate GetCertificate(GenerateCertificateQuery request, (BirthEvent? birth, DeathEvent? death, AdoptionEvent? adoption, MarriageEvent? marriage, DivorceEvent? divorce) content, string BirhtCertId)
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
                certificate.Content = JObject.FromObject(this.GetBirthCertificate(content.birth));
            }
            if (content.death != null)
            {
                // certificate.Content = JObject.FromObject(new DeathCertificateDTO());
                certificate.Content = JObject.FromObject(this.GetDeathCertificate(content.death));
            }
            if (content.adoption != null)
            {
                certificate.Content = JObject.FromObject(this.GetAdoptionCertificate(content.adoption, BirhtCertId));
            }
            if (content.marriage != null)
            {
                certificate.Content = JObject.FromObject(this.GetMarriageCertificate(content.marriage));
            }
            if (content.divorce != null)
            {
                certificate.Content = JObject.FromObject(this.GetDivorceCertificate(content.divorce));
            }


            return certificate;

        }
        private BirthCertificateDTO GetBirthCertificate(BirthEvent birth)
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

        private AdoptionCertificateDTO GetAdoptionCertificate(AdoptionEvent adoption, string? BirthCertNo)
        {
            return new AdoptionCertificateDTO()
            {
                CertifcateId = adoption.Event.CertificateId,
                RegBookNo = adoption.Event.RegBookNo,
                BirthCertifcateId = adoption.BirthCertificateId,
                ChildFirstNameAm = adoption.Event.EventOwener?.FirstName?.Value<string>("am"),
                ChildMiddleNameAm = adoption.Event.EventOwener?.MiddleName?.Value<string>("am"),
                ChildLastNameAm = adoption.Event.EventOwener?.LastName?.Value<string>("am"),
                ChildFirstNameOr = adoption.Event.EventOwener?.FirstName?.Value<string>("or"),
                ChildMiddleNameOr = adoption.Event.EventOwener?.MiddleName?.Value<string>("or"),
                ChildLastNameOr = adoption.Event.EventOwener?.LastName?.Value<string>("or"),
                GenderAm = adoption.Event?.EventOwener?.SexLookup?.Value?.Value<string>("am"),
                GenderOr = adoption.Event?.EventOwener?.SexLookup?.Value?.Value<string>("or"),

                BirthMonth = adoption.Event.EventDate.Month.ToString(),
                BirthDay = adoption.Event.EventDate.Month.ToString(),
                BirthYear = adoption.Event.EventDate.Month.ToString(),
                BirthAddressAm = (adoption.Event?.EventOwener?.BirthAddressId == Guid.Empty || adoption.Event?.EventOwener?.BirthAddressId == null) ? null : _DateAndAddressService.addressFormat(adoption.Event.EventOwener.BirthAddressId).Item1,
                BirthAddressOr = (adoption.Event?.EventOwener?.BirthAddressId == Guid.Empty || adoption.Event?.EventOwener?.BirthAddressId == null) ? null : _DateAndAddressService.addressFormat(adoption.Event.EventOwener.BirthAddressId).Item2,
                NationalityOr = adoption.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                NationalityAm = adoption.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                MotherFullNameOr = adoption.AdoptiveMother?.FirstName?.Value<string>("or")
                + " " + adoption.AdoptiveMother?.MiddleName?.Value<string>("or") + " " + adoption.AdoptiveMother?.LastName?.Value<string>("or"),
                MotherFullNameAm = adoption.AdoptiveMother?.FirstName?.Value<string>("am")
                + " " + adoption.AdoptiveMother?.MiddleName?.Value<string>("am") + " " + adoption.AdoptiveMother?.LastName?.Value<string>("am"),
                MotherNationalityOr = adoption.AdoptiveMother?.NationalityLookup?.Value?.Value<string>("or"),
                MotherNationalityAm = adoption.AdoptiveMother?.NationalityLookup?.Value?.Value<string>("am"),

                FatherFullNameOr = adoption.AdoptiveFather?.FirstName?.Value<string>("or")
                + " " + adoption.AdoptiveFather?.MiddleName?.Value<string>("or") + " " + adoption.AdoptiveFather?.LastName?.Value<string>("or"),
                FatherFullNameAm = adoption.AdoptiveFather?.FirstName?.Value<string>("am")
                + " " + adoption.AdoptiveFather?.MiddleName?.Value<string>("am") + " " + adoption.AdoptiveFather?.LastName?.Value<string>("am"),
                FatherNationalityOr = adoption.AdoptiveFather?.NationalityLookup?.Value?.Value<string>("or"),
                FatherNationalityAm = adoption.AdoptiveFather?.NationalityLookup?.Value?.Value<string>("am"),

                EventRegisteredMonth = adoption.Event.EventRegDate.Month.ToString(),
                EventRegisteredDay = adoption.Event.EventRegDate.Day.ToString(),
                EventRegisteredYear = adoption.Event.EventRegDate.Year.ToString(),
                GeneratedMonth = adoption.Event.CreatedAt.Month.ToString(),
                GeneratedDay = adoption.Event.CreatedAt.Day.ToString(),
                GeneratedYear = adoption.Event.CreatedAt.Year.ToString(),
                CivileRegOfficerFullNameOr = adoption.Event.CivilRegOfficer?.FirstName?.Value<string>("or")
                + " " + adoption.Event.CivilRegOfficer?.MiddleName?.Value<string>("or") + " " + adoption.Event.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = adoption.Event.CivilRegOfficer?.FirstName?.Value<string>("am")
                + " " + adoption.Event.CivilRegOfficer?.MiddleName?.Value<string>("am") + " " + adoption.Event.CivilRegOfficer?.LastName?.Value<string>("am"),

            };
        }
        private MarriageCertificateDTO GetMarriageCertificate(MarriageEvent marriage)
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


        private DivorceCertificateDTO GetDivorceCertificate(DivorceEvent Divorce)
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

        private DeathCertificateDTO GetDeathCertificate(DeathEvent death)
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

// FullNameAm = adoption.Event.EventOwener?.FirstName.Value<string>("am") + " " + adoption.Event.EventOwener?.MiddleName?.Value<string>("am") + " " + adoption.Event.EventOwener?.MiddleName?.Value<string>("am"),
//                 FullNameOr = adoption.Event.EventOwener?.FirstName.Value<string>("or") + " " + adoption.Event.EventOwener?.MiddleName?.Value<string>("or") + " " + adoption.Event.EventOwener?.MiddleName?.Value<string>("or"),