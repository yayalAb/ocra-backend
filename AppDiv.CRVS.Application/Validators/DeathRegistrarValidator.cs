using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Application.Validators
{
    public class DeathRegistrarValidator : AbstractValidator<RegistrarForDeathRequest>
    {
        private readonly (ILookupRepository Lookup, IAddressLookupRepository Address) _repo;
        public DeathRegistrarValidator((ILookupRepository Lookup, IAddressLookupRepository Address) repo)
        {
            _repo = repo;
            RuleFor(p => p.RelationshipLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo.Lookup, "Registrar.RelationshipLookupId");
            RuleFor(p => p.RegistrarInfo.FirstName.or).NotEmpty().NotNull();
            RuleFor(p => p.RegistrarInfo.FirstName.am).NotEmpty().NotNull();
            RuleFor(p => p.RegistrarInfo.MiddleName.or).NotEmpty().NotNull();
            RuleFor(p => p.RegistrarInfo.MiddleName.am).NotEmpty().NotNull();
            RuleFor(p => p.RegistrarInfo.LastName.or).NotEmpty().NotNull();
            RuleFor(p => p.RegistrarInfo.LastName.am).NotEmpty().NotNull();
            RuleFor(p => p.RegistrarInfo.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "RegistrarInfo.SexLookupId");
            RuleFor(p => p.RegistrarInfo.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "RegistrarInfo.ResidentAddressId");
            // RuleFor(p => p.RegistrarInfo.BirthDateEt).NotEmpty().NotNull().IsAbove18("Registrar age");
        }
    }

}