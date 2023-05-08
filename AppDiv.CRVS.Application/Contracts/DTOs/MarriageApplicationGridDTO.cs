using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class MarriageApplicationGridDTO
    {
        public Guid Id { get; set; }
        public DateTime ApplicationDate { get; set; }
        public Guid ApplicationAddressId { get; set; }
        public Guid BrideInfoId { get; set; }
        public Guid GroomInfoId { get; set; }
        public Guid CivilRegOfficerId { get; set; }

        // public virtual Address ApplicationAddress { get; set; }
        // public virtual PersonalInfo BrideInfo { get; set; }
        // public virtual PersonalInfo GroomInfo { get; set; }
        // public virtual PersonalInfo CivilRegOfficer { get; set; }
        // public virtual MarriageEvent MarriageEvent { get; set; }
    }
}