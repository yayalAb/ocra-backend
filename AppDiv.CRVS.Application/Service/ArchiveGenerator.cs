using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Features.Archives.Query;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Archive;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Service
{
    public class ArchiveGenerator : IArchiveGenerator
    {
        private readonly IDateAndAddressService _dateAndAddressService;
        private readonly IReturnAdoptionArchive _returnAdoptionArchive;
        private readonly IReturnDeathArchive _returnDeathArchive;
        private readonly IReturnBirthArchive _returnBirthArchive;
        private readonly IReturnMarriageArchive _returnMarriageArchive;
        private readonly IReturnDivorceArchive _returnDivorceArchive;

        private readonly ILogger<ArchiveGenerator> _Ilogger;
        public ArchiveGenerator(IDateAndAddressService dateAndAddressService,
                                IReturnAdoptionArchive returnAdoptionArchive,
                                IReturnDeathArchive returnDeathArchive,
                                IReturnBirthArchive returnBirthArchive,
                                IReturnMarriageArchive returnMarriageArchive,
                                IReturnDivorceArchive returnDivorceArchive,
                                ILogger<ArchiveGenerator> Ilogger)
        {
            _dateAndAddressService = dateAndAddressService;
            _returnAdoptionArchive = returnAdoptionArchive;
            _returnDeathArchive = returnDeathArchive;
            _returnBirthArchive = returnBirthArchive;
            _returnMarriageArchive = returnMarriageArchive;
            _returnDivorceArchive = returnDivorceArchive;
            _Ilogger = Ilogger;
        }

        public JObject GetBirthArchivePreview(BirthEvent birth, string BirthCertNo,bool isCorrectionRequest=false)
        {
            return JObject.FromObject(_returnBirthArchive.GetBirthPreviewArchive(birth, BirthCertNo,isCorrectionRequest));
        }

        public JObject GetAdoptionArchivePreview(AdoptionEvent adoption, string? BirthCertNo,bool isCorrectionRequest=false)
        {
            return JObject.FromObject(_returnAdoptionArchive.GetAdoptionPreviewArchive(adoption, BirthCertNo,isCorrectionRequest));
        }
        public JObject GetMarriageArchivePreview(MarriageEvent marriage, string? BirthCertNo,bool isCorrectionRequest=false)
        {
            return JObject.FromObject(_returnMarriageArchive.GetMarriagePreviewArchive(marriage, BirthCertNo,isCorrectionRequest));
        }


        public JObject GetDivorceArchivePreview(DivorceEvent divorce, string? BirthCertNo,bool isCorrectionRequest=false)
        {
            return JObject.FromObject(_returnDivorceArchive.GetDivorcePreviewArchive(divorce, BirthCertNo,isCorrectionRequest));
        }

        public JObject GetDeathArchivePreview(DeathEvent death, string? BirthCertNo,bool isCorrectionRequest=false)
        {
            return JObject.FromObject(_returnDeathArchive.GetDeathPreviewArchive(death, BirthCertNo,isCorrectionRequest));
        }
        // public JObject GetArchivePreview(JObject? content, string BirhtCertId)
        // {
        //     var archive = new object();

        //     return content.EventType switch
        //     {
        //         "Birth" => JObject.FromObject(this.GetBirthArchive(content, BirhtCertId)),
        //         "Death" => JObject.FromObject(this.GetDeathArchive(content, BirhtCertId)),
        //         "Adoption" => JObject.FromObject(this.GetAdoptionArchive(content, BirhtCertId)),
        //         "Marriage" => JObject.FromObject(this.GetMarriageArchive(content, BirhtCertId)),
        //         "Divorce" => JObject.FromObject(this.GetDivorceArchive(content, BirhtCertId))
        //     };
        // }
        public JObject GetArchive(GenerateArchiveQuery request, Event? content, string BirhtCertId,bool IsCorrection=false)
        {
            var archive = new object();

            return content.EventType switch
            {
                "Birth" => JObject.FromObject(this.GetBirthArchive(content, BirhtCertId,IsCorrection)),
                "Death" => JObject.FromObject(this.GetDeathArchive(content, BirhtCertId,IsCorrection)),
                "Adoption" => JObject.FromObject(this.GetAdoptionArchive(content, BirhtCertId,IsCorrection)),
                "Marriage" => JObject.FromObject(this.GetMarriageArchive(content, BirhtCertId,IsCorrection)),
                "Divorce" => JObject.FromObject(this.GetDivorceArchive(content, BirhtCertId,IsCorrection))
            };
        }




        private BirthArchiveDTO GetBirthArchive(Event birth, string BirthCertNo, bool IsCorrection=false)
        {
            return _returnBirthArchive.GetBirthArchive(birth, BirthCertNo,IsCorrection);
        }

        private AdoptionArchiveDTO GetAdoptionArchive(Event adoption, string? BirthCertNo,bool IsCorrection=false)
        {
            return _returnAdoptionArchive.GetAdoptionArchive(adoption, BirthCertNo,IsCorrection);
        }
        private MarriageArchiveDTO GetMarriageArchive(Event marriage, string? BirthCertNo,bool IsCorrection=false)
        {
            return _returnMarriageArchive.GetMarriageArchive(marriage, BirthCertNo,IsCorrection);
        }


        private DivorceArchiveDTO GetDivorceArchive(Event divorce, string? BirthCertNo,bool IsCorrection=false)
        {
            return _returnDivorceArchive.GetDivorceArchive(divorce, BirthCertNo,IsCorrection);
        }

        private DeathArchiveDTO GetDeathArchive(Event death, string? BirthCertNo,bool IsCorrection=false)
        {
            return _returnDeathArchive.GetDeathArchive(death, BirthCertNo,IsCorrection);
        }
    }
}