using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Application.Validators
{
    public class BirthEventValidator : AbstractValidator<AddBirthEventRequest>
    {
        private readonly (ILookupRepository Lookup, IPersonalInfoRepository Person) _repo;
        public BirthEventValidator((ILookupRepository Lookup, IPersonalInfoRepository Person) repo)
        {
            _repo = repo;
            RuleFor(p => p.FacilityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "FacilityLookupId");
            RuleFor(p => p.FacilityTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "FacilityTypeLookupId");
            RuleFor(p => p.BirthPlaceId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "BirthPlaceId");
            RuleFor(p => p.TypeOfBirthLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "TypeOfBirthLookupId");
            RuleFor(p => p.Event.RegBookNo).NotEmpty().NotNull();
            RuleFor(p => p.Event.CivilRegOfficeCode).NotEmpty().NotNull();
            RuleFor(p => p.Event.CertificateId).NotEmpty().NotNull();
            RuleFor(p => p.Event.CivilRegOfficerId.ToString()).NotEmpty().NotNull().ForeignKeyWithPerson(_repo.Person, "CivilRegOfficerId");
        }
    }




}