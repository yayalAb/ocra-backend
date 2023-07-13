using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using System.Text.RegularExpressions;
using FluentValidation;

namespace AppDiv.CRVS.Application.Validators
{
    public class FatherValidator : AbstractValidator<FatherInfoDTO>
    {
        private readonly IEventRepository _repo;
        public FatherValidator(IEventRepository repo)
        {
            _repo = repo;
            RuleFor(p => p.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo, "Father.Id")
                    .When(p => !string.IsNullOrEmpty(p.Id.ToString()) && p.Id != Guid.Empty);
            // Validation for father name
            RuleFor(p => p.FirstName.or).NotEmpty().NotNull();
            RuleFor(p => p.FirstName.am).NotEmpty().NotNull();
            RuleFor(p => p.MiddleName.am).NotEmpty().NotNull();
            RuleFor(p => p.MiddleName.or).NotEmpty().NotNull();
            RuleFor(p => p.LastName.am).NotEmpty().NotNull();
            RuleFor(p => p.LastName.or).NotEmpty().NotNull();

            RuleFor(p => p.NationalId).NotGuidEmpty()
            .When(p => p.NationalId != null);
            RuleFor(p => p.NationalityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Father.NationalityLookupId");
            // .When(p => p.NationalityLookupId != null);
            RuleFor(p => p.ReligionLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Father.ReligionLookupId")
            .When(p => p.ReligionLookupId != null);
            RuleFor(p => p.EducationalStatusLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Father.EducationalStatusLookupId")
            .When(p => p.EducationalStatusLookupId != null);
            RuleFor(p => p.TypeOfWorkLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Father.TypeOfWorkLookupId")
            .When(p => p.TypeOfWorkLookupId != null);
            RuleFor(p => p.MarriageStatusLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Father.MarriageStatusLookupId")
            .When(p => p.MarriageStatusLookupId != null);
            RuleFor(p => p.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo, "Father.ResidentAddressId")
            .When(p => p.ResidentAddressId != null);
            RuleFor(p => p.NationLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Father.NationLookupId")
            .When(p => p.NationLookupId != null);
            RuleFor(p => p.BirthDateEt).NotEmpty().NotNull()
            .IsValidDate("Father Birth date");//.IsAbove18("Father age");
            RuleFor(p => p.PhoneNumber).NotEmpty()
                        .Matches(new Regex(@"^(\+251)?\d{9}$")).WithMessage("Invalid phone number format.")
                        .When(p => p.PhoneNumber != null);
            RuleFor(p => p.BirthAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo, "Father.BirthAddressId")
            .When(p => p.BirthAddressId != null);
        }
    }
}