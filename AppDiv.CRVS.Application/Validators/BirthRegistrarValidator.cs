using System;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using AppDiv.CRVS.Application.Contracts.Request;
using System.Text.RegularExpressions;

namespace AppDiv.CRVS.Application.Validators
{
    public class BirthRegistrarValidator : AbstractValidator<RegistrarForBirthRequest>
    {
        private readonly IEventRepository _repo;
        public BirthRegistrarValidator(IEventRepository repo)
        {
            _repo = repo;
            RuleFor(p => p).NotNull();
            RuleFor(p => p.RegistrarInfo.Id.ToString()).ForeignKeyWithPerson(_repo, "EventRegistrar.RegistrarInfo.Id")
                            .When(p => (!string.IsNullOrEmpty(p.RegistrarInfo.Id.ToString()) && p.RegistrarInfo.Id != Guid.Empty));
            RuleFor(p => p.RelationshipLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Registrar.RelationshipLookupId");
            RuleFor(p => p.RegistrarInfo.FirstName.or).NotEmpty().NotNull();
            RuleFor(p => p.RegistrarInfo.FirstName.am).NotEmpty().NotNull();
            RuleFor(p => p.RegistrarInfo.MiddleName.or).NotEmpty().NotNull();
            RuleFor(p => p.RegistrarInfo.MiddleName.am).NotEmpty().NotNull();
            RuleFor(p => p.RegistrarInfo.LastName.or).NotEmpty().NotNull();
            RuleFor(p => p.RegistrarInfo.LastName.am).NotEmpty().NotNull();
            RuleFor(p => p.RegistrarInfo.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Registrar.SexLookupId")
            .When(p => p.RegistrarInfo.SexLookupId != null);
            RuleFor(p => p.RegistrarInfo.NationalId!.ToString()).NotNull().NotEmpty()
            .When(p => p.RegistrarInfo.NationalId != null);
            RuleFor(p => p.RegistrarInfo.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo, "Registrar.ResidentAddressId")
            .When(p => p.RegistrarInfo.ResidentAddressId != null);
            RuleFor(p => p.RegistrarInfo.BirthDateEt).NotEmpty().NotNull()
            .IsAbove18("Registrar age");
            RuleFor(p => p.RegistrarInfo.PhoneNumber).NotEmpty()
                        .Matches(new Regex(@"^(\+251)?\d{9}$")).WithMessage("Invalid phone number format.")
                        .When(p => p.RegistrarInfo.PhoneNumber != null);
        }
    }

}