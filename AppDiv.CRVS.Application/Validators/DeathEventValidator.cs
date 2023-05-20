using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Application.Validators
{
    public class DeathEventValidator : AbstractValidator<AddDeathEventRequest>
    {
        private readonly (ILookupRepository Lookup, IPersonalInfoRepository Person) _repo;
        public DeathEventValidator((ILookupRepository Lookup, IPersonalInfoRepository Person) repo)
        {
            _repo = repo;
            RuleFor(p => p.FacilityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "FacilityLookupId");
            RuleFor(p => p.FacilityTypeLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "FacilityTypeLookupId");
            RuleFor(p => p.BirthCertificateId).NotEmpty().NotNull();
            RuleFor(p => p.PlaceOfFuneral).NotEmpty().NotNull();
            // RuleFor(p => p.Event.RegBookNo).NotEmpty().NotNull();
            // RuleFor(p => p.Event.CivilRegOfficeCode).NotEmpty().NotNull();
            RuleFor(p => p.Event.CertificateId).NotEmpty().NotNull();
            RuleFor(p => p.Event.EventRegDate).NotEmpty().NotNull().Must(date => date > new DateTime(1900, 1, 1));
            RuleFor(p => p.Event.CivilRegOfficerId.ToString()).NotEmpty().NotNull().ForeignKeyWithPerson(_repo.Person, "CivilRegOfficerId");
        }
    }




}