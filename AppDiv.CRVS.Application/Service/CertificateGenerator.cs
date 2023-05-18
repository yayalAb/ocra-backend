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
            (string Am, string Or) address = _DateAndAddressService.addressFormat(birth.Event?.EventAddress?.Id);
            return new BirthCertificateDTO()
            {
                CertifcateId = birth.Event.CertificateId,
                RegBookNo = birth.Event.RegBookNo,
                // BirthCertifcateId = birth.BirthCertificateId,
                ChildFirstNameAm = birth.Event.EventOwener?.FirstName?.Value<string>("am"),
                ChildMiddleNameAm = birth.Event.EventOwener?.MiddleName?.Value<string>("am"),
                ChildLastNameAm = birth.Event.EventOwener?.LastName?.Value<string>("am"),
                ChildFirstNameOr = birth.Event.EventOwener?.FirstName?.Value<string>("or"),
                ChildMiddleNameOr = birth.Event.EventOwener?.MiddleName?.Value<string>("or"),
                ChildLastNameOr = birth.Event.EventOwener?.LastName?.Value<string>("or"),

                GenderAm = birth.Event?.EventOwener?.SexLookup?.Value?.Value<string>("am"),
                GenderOr = birth.Event?.EventOwener?.SexLookup?.Value?.Value<string>("or"),

                BirthMonth = birth.Event.EventDate.Month.ToString(),
                BirthDay = birth.Event.EventDate.Month.ToString(),
                BirthYear = birth.Event.EventDate.Month.ToString(),

                // BirthAddressAm = birth.Event?.EventAddress?.Id.ToString(),
                BirthAddressAm = address.Am,
                BirthAddressOr = address.Or,
                NationalityOr = birth.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                NationalityAm = birth.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                MotherFullNameOr = birth.Mother?.FirstName?.Value<string>("or") + " "
                                 + birth.Mother?.MiddleName?.Value<string>("or") + " "
                                 + birth.Mother?.LastName?.Value<string>("or"),
                MotherFullNameAm = birth.Mother?.FirstName?.Value<string>("am") + " "
                                 + birth.Mother?.MiddleName?.Value<string>("am") + " "
                                 + birth.Mother?.LastName?.Value<string>("am"),
                MotherNationalityOr = birth.Mother?.NationalityLookup?.Value?.Value<string>("or"),
                MotherNationalityAm = birth.Mother?.NationalityLookup?.Value?.Value<string>("am"),

                FatherFullNameOr = birth.Father?.FirstName?.Value<string>("or") + " "
                                 + birth.Father?.MiddleName?.Value<string>("or") + " "
                                 + birth.Father?.LastName?.Value<string>("or"),
                FatherFullNameAm = birth.Father?.FirstName?.Value<string>("am") + " "
                                 + birth.Father?.MiddleName?.Value<string>("am") + " "
                                 + birth.Father?.LastName?.Value<string>("am"),
                FatherNationalityOr = birth.Father?.NationalityLookup?.Value?.Value<string>("or"),
                FatherNationalityAm = birth.Father?.NationalityLookup?.Value?.Value<string>("am"),

                EventRegisteredMonth = birth.Event.EventRegDate.Month.ToString(),
                EventRegisteredDay = birth.Event.EventRegDate.Day.ToString(),
                EventRegisteredYear = birth.Event.EventRegDate.Year.ToString(),

                GeneratedMonth = birth.Event.CreatedAt.Month.ToString(),
                GeneratedDay = birth.Event.CreatedAt.Day.ToString(),
                GeneratedYear = birth.Event.CreatedAt.Year.ToString(),

                CivileRegOfficerFullNameOr = birth.Event.CivilRegOfficer?.FirstName?.Value<string>("or") + " "
                                           + birth.Event.CivilRegOfficer?.MiddleName?.Value<string>("or") + " "
                                           + birth.Event.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = birth.Event.CivilRegOfficer?.FirstName?.Value<string>("am") + " "
                                           + birth.Event.CivilRegOfficer?.MiddleName?.Value<string>("am") + " "
                                           + birth.Event.CivilRegOfficer?.LastName?.Value<string>("am"),

            };
        }

        private AdoptionCertificateDTO GetAdoptionCertificate(AdoptionEvent adoption, string? BirthCertNo)
        {
            string AddressAm = (adoption.Event?.EventOwener?.BirthAddressId == Guid.Empty
                    || adoption.Event?.EventOwener?.BirthAddressId == null) ? null :
                    _DateAndAddressService.addressFormat(adoption.Event.EventOwener.BirthAddressId).Item1;
            string AddressOr = (adoption.Event?.EventOwener?.BirthAddressId == Guid.Empty
               || adoption.Event?.EventOwener?.BirthAddressId == null) ? null :
               _DateAndAddressService.addressFormat(adoption.Event.EventOwener.BirthAddressId).Item2;

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
                BirthAddressAm = AddressAm,
                BirthAddressOr = AddressOr,
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
            (string Am, string Or) address = _DateAndAddressService.addressFormat(marriage.Event?.EventAddress?.Id);
            return new MarriageCertificateDTO()
            {
                CertifcateId = marriage.Event.CertificateId,
                RegBookNo = marriage.Event.RegBookNo,
                // BrideBirthCertifcateId = marriage.BirthCertificateBrideId,
                BrideFirstNameAm = marriage.BrideInfo.FirstName?.Value<string>("am"),
                BrideMiddleNameAm = marriage.BrideInfo.MiddleName?.Value<string>("am"),
                BrideLastNameAm = marriage.BrideInfo.LastName?.Value<string>("am"),
                BrideFirstNameOr = marriage.BrideInfo.FirstName?.Value<string>("or"),
                BrideMiddleNameOr = marriage.BrideInfo.MiddleName?.Value<string>("or"),
                BrideLastNameOr = marriage.BrideInfo.LastName?.Value<string>("or"),

                BrideNationalityOr = marriage.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                BrideNationalityAm = marriage.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                // GroomBirthCertifcateId = marriage.BirthCertificateGroomId,
                GroomFirstNameAm = marriage.Event.EventOwener?.FirstName?.Value<string>("am"),
                GroomMiddleNameAm = marriage.Event.EventOwener?.MiddleName?.Value<string>("am"),
                GroomLastNameAm = marriage.Event.EventOwener?.LastName?.Value<string>("am"),
                GroomFirstNameOr = marriage.Event.EventOwener?.FirstName?.Value<string>("or"),
                GroomMiddleNameOr = marriage.Event.EventOwener?.MiddleName?.Value<string>("or"),
                GroomLastNameOr = marriage.Event.EventOwener?.LastName?.Value<string>("or"),

                GroomNationalityOr = marriage.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                GroomNationalityAm = marriage.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                MarriageMonth = marriage.Event.EventDate.Month.ToString(),
                MarriageDay = marriage.Event.EventDate.Month.ToString(),
                MarriageYear = marriage.Event.EventDate.Month.ToString(),

                // BirthAddressAm = birth.Event?.EventAddress?.Id.ToString(),
                MarriageAddressAm = address.Am,
                MarriageAddressOr = address.Or,

                EventRegisteredMonth = marriage.Event.EventRegDate.Month.ToString(),
                EventRegisteredDay = marriage.Event.EventRegDate.Day.ToString(),
                EventRegisteredYear = marriage.Event.EventRegDate.Year.ToString(),

                GeneratedMonth = marriage.Event.CreatedAt.Month.ToString(),
                GeneratedDay = marriage.Event.CreatedAt.Day.ToString(),
                GeneratedYear = marriage.Event.CreatedAt.Year.ToString(),

                CivileRegOfficerFullNameOr = marriage.Event.CivilRegOfficer?.FirstName?.Value<string>("or") + " "
                                           + marriage.Event.CivilRegOfficer?.MiddleName?.Value<string>("or") + " "
                                           + marriage.Event.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = marriage.Event.CivilRegOfficer?.FirstName?.Value<string>("am") + " "
                                           + marriage.Event.CivilRegOfficer?.MiddleName?.Value<string>("am") + " "
                                           + marriage.Event.CivilRegOfficer?.LastName?.Value<string>("am"),


            };
        }


        private DivorceCertificateDTO GetDivorceCertificate(DivorceEvent divorce)
        {
            (string Am, string Or) address = _DateAndAddressService.addressFormat(divorce.Event?.EventAddress?.Id);
            return new DivorceCertificateDTO()
            {
                CertifcateId = divorce.Event.CertificateId,
                RegBookNo = divorce.Event.RegBookNo,
                // BrideBirthCertifcateId = divorce.BirthCertificateBrideId,
                WifeFirstNameAm = divorce.DivorcedWife.FirstName?.Value<string>("am"),
                WifeMiddleNameAm = divorce.DivorcedWife.MiddleName?.Value<string>("am"),
                WifeLastNameAm = divorce.DivorcedWife.LastName?.Value<string>("am"),
                WifeFirstNameOr = divorce.DivorcedWife.FirstName?.Value<string>("or"),
                WifeMiddleNameOr = divorce.DivorcedWife.MiddleName?.Value<string>("or"),
                WifeLastNameOr = divorce.DivorcedWife.LastName?.Value<string>("or"),

                WifeNationalityOr = divorce.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                WifeNationalityAm = divorce.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                HusbandFirstNameAm = divorce.Event.EventOwener?.FirstName?.Value<string>("am"),
                HusbandMiddleNameAm = divorce.Event.EventOwener?.MiddleName?.Value<string>("am"),
                HusbandLastNameAm = divorce.Event.EventOwener?.LastName?.Value<string>("am"),
                HusbandFirstNameOr = divorce.Event.EventOwener?.FirstName?.Value<string>("or"),
                HusbandMiddleNameOr = divorce.Event.EventOwener?.MiddleName?.Value<string>("or"),
                HusbandLastNameOr = divorce.Event.EventOwener?.LastName?.Value<string>("or"),

                HusbandNationalityOr = divorce.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                HusbandNationalityAm = divorce.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                DivorceMonth = divorce.Event.EventDate.Month.ToString(),
                DivorceDay = divorce.Event.EventDate.Month.ToString(),
                DivorceYear = divorce.Event.EventDate.Month.ToString(),

                // BirthAddressAm = birth.Event?.EventAddress?.Id.ToString(),
                DivorceAddressAm = address.Am,
                DivorceAddressOr = address.Or,

                EventRegisteredMonth = divorce.Event.EventRegDate.Month.ToString(),
                EventRegisteredDay = divorce.Event.EventRegDate.Day.ToString(),
                EventRegisteredYear = divorce.Event.EventRegDate.Year.ToString(),

                GeneratedMonth = divorce.Event.CreatedAt.Month.ToString(),
                GeneratedDay = divorce.Event.CreatedAt.Day.ToString(),
                GeneratedYear = divorce.Event.CreatedAt.Year.ToString(),

                CivileRegOfficerFullNameOr = divorce.Event.CivilRegOfficer?.FirstName?.Value<string>("or") + " "
                                           + divorce.Event.CivilRegOfficer?.MiddleName?.Value<string>("or") + " "
                                           + divorce.Event.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = divorce.Event.CivilRegOfficer?.FirstName?.Value<string>("am") + " "
                                           + divorce.Event.CivilRegOfficer?.MiddleName?.Value<string>("am") + " "
                                           + divorce.Event.CivilRegOfficer?.LastName?.Value<string>("am"),


            };
        }

        private DeathCertificateDTO GetDeathCertificate(DeathEvent death)
        {
            (string Am, string Or) address = _DateAndAddressService.addressFormat(death.Event?.EventAddress?.Id);
            return new DeathCertificateDTO()
            {
                CertifcateId = death.Event.CertificateId,
                RegBookNo = death.Event.RegBookNo,
                BirthCertifcateId = death.BirthCertificateId,
                FirstNameAm = death.Event.EventOwener?.FirstName?.Value<string>("am"),
                MiddleNameAm = death.Event.EventOwener?.MiddleName?.Value<string>("am"),
                LastNameAm = death.Event.EventOwener?.LastName?.Value<string>("am"),
                FirstNameOr = death.Event.EventOwener?.FirstName?.Value<string>("or"),
                MiddleNameOr = death.Event.EventOwener?.MiddleName?.Value<string>("or"),
                LastNameOr = death.Event.EventOwener?.LastName?.Value<string>("or"),

                GenderAm = death.Event?.EventOwener?.SexLookup?.Value?.Value<string>("am"),
                GenderOr = death.Event?.EventOwener?.SexLookup?.Value?.Value<string>("or"),

                BirthMonth = death.Event.EventDate.Month.ToString(),
                BirthDay = death.Event.EventDate.Month.ToString(),
                BirthYear = death.Event.EventDate.Month.ToString(),

                // BirthAddressAm = birth.Event?.EventAddress?.Id.ToString(),
                DeathPlaceAm = address.Am,
                DeathPlaceOr = address.Or,

                DeathMonth = death.Event.EventDate.Month.ToString(),
                DeathDay = death.Event.EventDate.Month.ToString(),
                DeathYear = death.Event.EventDate.Month.ToString(),

                NationalityOr = death.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("or"),
                NationalityAm = death.Event?.EventOwener?.NationalityLookup?.Value?.Value<string>("am"),

                EventRegisteredMonth = death.Event.EventRegDate.Month.ToString(),
                EventRegisteredDay = death.Event.EventRegDate.Day.ToString(),
                EventRegisteredYear = death.Event.EventRegDate.Year.ToString(),

                GeneratedMonth = death.Event.CreatedAt.Month.ToString(),
                GeneratedDay = death.Event.CreatedAt.Day.ToString(),
                GeneratedYear = death.Event.CreatedAt.Year.ToString(),

                CivileRegOfficerFullNameOr = death.Event.CivilRegOfficer?.FirstName?.Value<string>("or") + " "
                                           + death.Event.CivilRegOfficer?.MiddleName?.Value<string>("or") + " "
                                           + death.Event.CivilRegOfficer?.LastName?.Value<string>("or"),
                CivileRegOfficerFullNameAm = death.Event.CivilRegOfficer?.FirstName?.Value<string>("am") + " "
                                           + death.Event.CivilRegOfficer?.MiddleName?.Value<string>("am") + " "
                                           + death.Event.CivilRegOfficer?.LastName?.Value<string>("am"),

            };
        }
    }


}

// FullNameAm = adoption.Event.EventOwener?.FirstName.Value<string>("am") + " " + adoption.Event.EventOwener?.MiddleName?.Value<string>("am") + " " + adoption.Event.EventOwener?.MiddleName?.Value<string>("am"),
//                 FullNameOr = adoption.Event.EventOwener?.FirstName.Value<string>("or") + " " + adoption.Event.EventOwener?.MiddleName?.Value<string>("or") + " " + adoption.Event.EventOwener?.MiddleName?.Value<string>("or"),