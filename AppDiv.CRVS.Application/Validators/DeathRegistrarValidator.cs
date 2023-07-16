using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using AppDiv.CRVS.Application.Contracts.Request;
using System.Text.RegularExpressions;

namespace AppDiv.CRVS.Application.Validators
{
    public class DeathRegistrarValidator : AbstractValidator<RegistrarForDeathRequest>
    {
        private readonly IEventRepository _repo;
        public DeathRegistrarValidator(IEventRepository repo)
        {
            _repo = repo;
            RuleFor(p => p.RegistrarInfo.Id.ToString())
                    .ForeignKeyWithPerson(repo, "EventRegistrar.RegistrarInfo.Id")
                    .When(p => (!string.IsNullOrEmpty(p.RegistrarInfo.Id.ToString()) && p.RegistrarInfo.Id != Guid.Empty));
            RuleFor(p => p.RelationshipLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "Registrar.RelationshipLookupId")
            .When(p => p.RelationshipLookupId != null);
            RuleFor(p => p.RegistrarInfo.FirstName.or).NotEmpty().NotNull();
            RuleFor(p => p.RegistrarInfo.FirstName.am).NotEmpty().NotNull();
            RuleFor(p => p.RegistrarInfo.MiddleName.or).NotEmpty().NotNull();
            RuleFor(p => p.RegistrarInfo.MiddleName.am).NotEmpty().NotNull();
            RuleFor(p => p.RegistrarInfo.LastName.or).NotEmpty().NotNull();
            RuleFor(p => p.RegistrarInfo.LastName.am).NotEmpty().NotNull();
            RuleFor(p => p.RegistrarInfo.SexLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "RegistrarInfo.SexLookupId")
            .When(p => p.RegistrarInfo.SexLookupId != null);
            RuleFor(p => p.RegistrarInfo.ResidentAddressId.ToString()).NotEmpty().NotNull().ForeignKeyWithAddress(_repo, "RegistrarInfo.ResidentAddressId")
            .When(p => p.RegistrarInfo.ResidentAddressId != null);
            RuleFor(p => p.RegistrarInfo.PhoneNumber).NotEmpty()
                        .Matches(new Regex(@"^(\+251)?\d{9}$")).WithMessage("Invalid phone number format.")
                        .When(p => p.RegistrarInfo.PhoneNumber != null);
            // RuleFor(p => p.RegistrarInfo.BirthDateEt).NotEmpty().NotNull().IsAbove18("Registrar age");
        }
    }

}