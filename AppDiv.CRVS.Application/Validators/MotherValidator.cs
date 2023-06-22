using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using System.Text.RegularExpressions;
using FluentValidation;

namespace AppDiv.CRVS.Application.Validators
{
    public class MotherValidator : AbstractValidator<MotherInfoDTO>
    {
        private readonly IEventRepository _repo;
        public MotherValidator(IEventRepository repo)
        {
            _repo = repo;
            RuleFor(p => p.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo, "Mother.Id")
                                .When(p => (!string.IsNullOrEmpty(p.Id.ToString()) && p.Id != Guid.Empty));
            RuleFor(p => p.FirstName.or).NotEmpty().NotNull();
            // .Matches("^[a-zA-Z']+${1,50}")
            // .WithMessage("Name should only contain alphabets and apostrophes, and be between 1 and 50 characters long.");
            RuleFor(p => p.FirstName.am).NotEmpty().NotNull();
            RuleFor(p => p.MiddleName.am).NotEmpty().NotNull();
            RuleFor(p => p.MiddleName.or).NotEmpty().NotNull();
            // .Matches("^[a-zA-Z']+${1,50}")
            //     .WithMessage("Name should only contain alphabets and apostrophes, and be between 1 and 50 characters long.");
            RuleFor(p => p.LastName.am).NotEmpty().NotNull();
            RuleFor(p => p.LastName.or).NotEmpty().NotNull();
            // .Matches("^[a-zA-Z']+${1,50}")
            //     .WithMessage("Name should only contain alphabets and apostrophes, and be between 1 and 50 characters long.");
            RuleFor(p => p.NationalId).NotGuidEmpty();
            RuleFor(p => p.BirthDateEt).NotEmpty().NotNull()
                .IsValidDate("Mother Birth date");//.IsAbove18("Mother age");
            // RuleFor(p => p.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Mother.SexLookupId");
            RuleFor(p => p.NationalityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Mother.NationalityLookupId");
            RuleFor(p => p.ReligionLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Mother.ReligionLookupId");
            RuleFor(p => p.EducationalStatusLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Mother.EducationalStatusLookupId");
            RuleFor(p => p.TypeOfWorkLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Mother.TypeOfWorkLookupId");
            RuleFor(p => p.MarriageStatusLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Mother.MarriageStatusLookupId");
            RuleFor(p => p.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo, "Mother.ResidentAddressId");
            RuleFor(p => p.NationLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Mother.NationLookupId");
            RuleFor(p => p.PhoneNumber).NotEmpty()
                        .Matches(new Regex(@"^(\+251)\d{10}$")).WithMessage("Invalid phone number format.")
                        .When(p => p.PhoneNumber != null);
        }
    }
}