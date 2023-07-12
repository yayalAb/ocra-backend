using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Application.Validators
{
    public class BirthEventValidator : AbstractValidator<AddBirthEventRequest>
    {
        private readonly IEventRepository _repo;
        public BirthEventValidator(IEventRepository repo)
        {
            _repo = repo;
            RuleFor(p => p.FacilityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "FacilityLookupId")
            .When(p => p.FacilityLookupId != null);
            RuleFor(p => p.FacilityTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "FacilityTypeLookupId")
            .When(p => p.FacilityTypeLookupId != null);
            RuleFor(p => p.BirthPlaceId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "BirthPlaceId")
            .When(p => p.BirthPlaceId != null);
            RuleFor(p => p.TypeOfBirthLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "TypeOfBirthLookupId")
            .When(p => p.TypeOfBirthLookupId != null);
            RuleFor(p => p.Event.EventRegDateEt).NotEmpty().NotNull().IsValidRegistrationDate("Event EventRegDateEt");
            // RuleFor(p => p.Event.CivilRegOfficeCode).NotEmpty().NotNull();
            // RuleFor(p => p.Event.CertificateId).NotEmpty().NotNull().ValidCertificate(repo, "Event.CertificateId");
            RuleFor(p => p.Event.CivilRegOfficerId.ToString()).NotEmpty().NotNull().ForeignKeyWithPerson(_repo, "CivilRegOfficerId");
        }
    }

}