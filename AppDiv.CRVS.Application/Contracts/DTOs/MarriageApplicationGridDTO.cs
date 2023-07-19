using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;
using Application.Common.Mappings;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class MarriageApplicationGridDTO : IMapFrom<MarriageApplication>
    {
        public Guid? Id { get; set; }
        // public DateTime ApplicationDate { get; set; }
        public string? ApplicationDate { get; set; }

        // public Guid ApplicationAddressId { get; set; }
        public string? BrideFullName { get; set; }
        public string? GroomFullName { get; set; }

        public MarriageApplicationGridDTO()
        {

        }
        public MarriageApplicationGridDTO(MarriageApplication? application)
        {
            Id = application?.Id;
            ApplicationDate = application?.ApplicationDateEt;
            BrideFullName = application?.BrideInfo?.FirstNameLang + " " + application?.BrideInfo?.MiddleNameLang + " " + application?.BrideInfo?.LastNameLang;
            GroomFullName = application?.GroomInfo?.FirstNameLang + " " + application?.GroomInfo?.MiddleNameLang + " " + application?.GroomInfo?.LastNameLang;
        }
    }
}