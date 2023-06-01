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
        public JObject GetArchive(GenerateArchiveQuery request, Event? content, string BirhtCertId)
        {
            var archive = new object();

            return content.EventType switch
            {
                "Birth" => JObject.FromObject(this.GetBirthArchive(content, BirhtCertId)),
                "Death" => JObject.FromObject(this.GetDeathArchive(content, BirhtCertId)),
                "Adoption" => JObject.FromObject(this.GetAdoptionArchive(content, BirhtCertId)),
                "Marriage" => JObject.FromObject(this.GetMarriageArchive(content, BirhtCertId)),
                "Divorce" => JObject.FromObject(this.GetDivorceArchive(content, BirhtCertId))
            };
        }

        private BirthArchiveDTO GetBirthArchive(Event birth, string BirthCertNo)
        {
            return _returnBirthArchive.GetBirthArchive(birth, BirthCertNo);
        }

        private AdoptionArchiveDTO GetAdoptionArchive(Event adoption, string? BirthCertNo)
        {
            return _returnAdoptionArchive.GetAdoptionArchive(adoption, BirthCertNo);
        }
        private MarriageArchiveDTO GetMarriageArchive(Event marriage, string? BirthCertNo)
        {
            return _returnMarriageArchive.GetMarriageArchive(marriage, BirthCertNo);
        }


        private DivorceArchiveDTO GetDivorceArchive(Event divorce, string? BirthCertNo)
        {
            return _returnDivorceArchive.GetDivorceArchive(divorce, BirthCertNo);
        }

        private DeathArchiveDTO GetDeathArchive(Event death, string? BirthCertNo)
        {
            return _returnDeathArchive.GetDeathArchive(death, BirthCertNo);
        }
    }
}