using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Application.Exceptions;

namespace AppDiv.CRVS.Application.Validators
{
    public class BirthEventValidator : AbstractValidator<AddBirthEventRequest>
    {
        private readonly IEventRepository _repo;
        private readonly ILookupRepository _lookupRepo;

        public BirthEventValidator(IEventRepository repo, ILookupRepository lookupRepo)
        {
            _repo = repo;
            _lookupRepo = lookupRepo;
            RuleFor(p => p.FacilityLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "FacilityLookupId")
            .When(p => p.FacilityLookupId != null);
            RuleFor(p => p.FacilityTypeLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "FacilityTypeLookupId")
            .When(p => p.FacilityTypeLookupId != null);
            RuleFor(p => p.BirthPlaceId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "BirthPlaceId")
            .When(p => p.BirthPlaceId != null);
            RuleFor(p => p.TypeOfBirthLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "TypeOfBirthLookupId")
            .When(p => p.TypeOfBirthLookupId != null);
            RuleFor(p => p.Event.EventRegDateEt).NotEmpty().NotNull().IsValidRegistrationDate("Event EventRegDateEt");
            // RuleFor(p => p.Event.CivilRegOfficeCode).NotEmpty().NotNull();
            // RuleFor(p => p.Event.CertificateId).NotEmpty().NotNull().ValidCertificate(repo, "Event.CertificateId");
            RuleFor(p => p.Event.CivilRegOfficerId.ToString()).NotEmpty().NotNull().ForeignKeyWithPerson(_repo, "CivilRegOfficerId");

            RuleFor(p => p.Event.EventSupportingDocuments)
            .Must((e, p) => ValidationService.HaveGuardianSupportingDoc(p , _lookupRepo))
            .WithMessage("guardian supporting document must be attached if informant type is legal guardian ")
            .When(p => p.Event.InformantType == "guardian");

        }

       
    }

}