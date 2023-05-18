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
        private readonly IDateAndAddressService _DateAndAddressService;
        private readonly IReturnAdoptionCertfcate _ReturnAdoptionCertfcate;
        private readonly IReturnDeathCertificate _ReturnDeathCertificate;
        private readonly IReturnBirthCertificate _ReturnBirthCertificate;
        private readonly IReturnMarriageCertificate _ReturnMarriageCertificate;
        private readonly IReturnDivorceCertificate _ReturnDivorceCertificate;

        private readonly ILogger<CertificateGenerator> _Ilogger;
        public CertificateGenerator(IDateAndAddressService DateAndAddressService,
                                    IReturnAdoptionCertfcate ReturnAdoptionCertfcate,
                                    IReturnDeathCertificate ReturnDeathCertificate,
                                    IReturnBirthCertificate ReturnBirthCertificate,
                                    IReturnMarriageCertificate ReturnMarriageCertificate,
                                    IReturnDivorceCertificate ReturnDivorceCertificate,
                                    ILogger<CertificateGenerator> Ilogger)
        {
            _DateAndAddressService = DateAndAddressService;
            _ReturnAdoptionCertfcate = ReturnAdoptionCertfcate;
            _ReturnDeathCertificate = ReturnDeathCertificate;
            _ReturnBirthCertificate = ReturnBirthCertificate;
            _ReturnMarriageCertificate = ReturnMarriageCertificate;
            _ReturnDivorceCertificate = ReturnDivorceCertificate;
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
                certificate.Content = JObject.FromObject(this.GetBirthCertificate(content.birth, BirhtCertId));
            }
            if (content.death != null)
            {
                // certificate.Content = JObject.FromObject(new DeathCertificateDTO());
                certificate.Content = JObject.FromObject(this.GetDeathCertificate(content.death, BirhtCertId));
            }
            if (content.adoption != null)
            {
                certificate.Content = JObject.FromObject(this.GetAdoptionCertificate(content.adoption, BirhtCertId));
            }
            if (content.marriage != null)
            {
                certificate.Content = JObject.FromObject(this.GetMarriageCertificate(content.marriage, BirhtCertId));
            }
            if (content.divorce != null)
            {
                certificate.Content = JObject.FromObject(this.GetDivorceCertificate(content.divorce, BirhtCertId));
            }


            return certificate;

        }
        private BirthCertificateDTO GetBirthCertificate(BirthEvent birth, string BirthCertNo)
        {
            return _ReturnBirthCertificate.GetBirthCertificate(birth, BirthCertNo);
        }

        private AdoptionCertificateDTO GetAdoptionCertificate(AdoptionEvent adoption, string? BirthCertNo)
        {
            return _ReturnAdoptionCertfcate.GetAdoptionCertificate(adoption, BirthCertNo);
        }
        private MarriageCertificateDTO GetMarriageCertificate(MarriageEvent marriage, string? BirthCertNo)
        {
            return _ReturnMarriageCertificate.GetMarriageCertificate(marriage, BirthCertNo);
        }


        private DivorceCertificateDTO GetDivorceCertificate(DivorceEvent divorce, string? BirthCertNo)
        {
            return _ReturnDivorceCertificate.GetDivorceCertificate(divorce, BirthCertNo);
        }

        private DeathCertificateDTO GetDeathCertificate(DeathEvent death, string? BirthCertNo)
        {
            return _ReturnDeathCertificate.GetDeathCertificate(death, BirthCertNo);
        }
    }


}

// FullNameAm = adoption.Event.EventOwener?.FirstName.Value<string>("am") + " " + adoption.Event.EventOwener?.MiddleName?.Value<string>("am") + " " + adoption.Event.EventOwener?.MiddleName?.Value<string>("am"),
//                 FullNameOr = adoption.Event.EventOwener?.FirstName.Value<string>("or") + " " + adoption.Event.EventOwener?.MiddleName?.Value<string>("or") + " " + adoption.Event.EventOwener?.MiddleName?.Value<string>("or"),