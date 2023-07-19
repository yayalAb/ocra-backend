using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Domain.Entities;
using static AppDiv.CRVS.Application.Contracts.Request.AdoptionPersonalINformationRequest;

namespace AppDiv.CRVS.Application.CouchModels
{
    public class AdoptionEventCouch : BaseEventCouch
    {
        public Guid? Id { get; set; }
        public Guid? BeforeAdoptionAddressId { get; set; }
        public string? BirthCertificateId { get; set; }
        public LanguageModel ApprovedName { get; set; }
        public LanguageModel? Reason { get; set; }
        public virtual AddAdoptionPersonalInfoRequest? AdoptiveMother { get; set; }
        public AddAdoptionPersonalInfoRequest? AdoptiveFather { get; set; }
        public virtual AddCourtCaseRequest CourtCase { get; set; }
        public virtual AddAdoptionEventRequest Event { get; set; }

    }
}