

using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.CouchModels
{
    public class MarriageEventCouch :BaseEventCouch
    {
        public Guid Id2 {get;set;}
        public Guid MarriageTypeId { get; set; }
        public Guid? ApplicationId { get; set; }
        public string? BirthCertificateGroomId { get; set; }
        public string? BirthCertificateBrideId { get; set; }
        public bool HasCamera { get; set; } = false;
        public bool HasVideo { get; set; } = false;
        public virtual BrideInfoDTO BrideInfo { get; set; }
        public virtual AddEventForMarriageRequest Event { get; set; }
        public virtual ICollection<AddWitnessRequest> Witnesses { get; set; }
        
    }
}