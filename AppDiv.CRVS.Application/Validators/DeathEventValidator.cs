using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Application.Validators
{
    public class DeathEventValidator : AbstractValidator<AddDeathEventRequest>
    {
        private readonly IEventRepository _repo;
        public DeathEventValidator(IEventRepository repo)
        {
            _repo = repo;
            RuleFor(p => p.FacilityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "FacilityLookupId");
            RuleFor(p => p.FacilityTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "FacilityTypeLookupId");
            // RuleFor(p => p.BirthCertificateId).NotEmpty().NotNull();
            RuleFor(p => p.DuringDeathId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "DuringDeathId")
                    .When(p => p.DuringDeathId != null);
            RuleFor(p => p.PlaceOfFuneral).NotEmpty().NotNull();
            // RuleFor(p => p.Event.RegBookNo).NotEmpty().NotNull();
            // RuleFor(p => p.Event.CivilRegOfficeCode).NotEmpty().NotNull();
            RuleFor(p => p.Event.CertificateId).NotEmpty().NotNull().ValidCertificate(repo, "Event.CertificateId");
            RuleFor(p => p.Event.EventRegDateEt).NotEmpty().NotNull().IsValidRegistrationDate("Event EventRegDateEt");
            RuleFor(p => p.Event.CivilRegOfficerId.ToString()).NotEmpty().NotNull().ForeignKeyWithPerson(_repo, "CivilRegOfficerId");
        }
    }
}