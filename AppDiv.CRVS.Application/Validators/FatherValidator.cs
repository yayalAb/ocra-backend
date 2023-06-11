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
            RuleFor(p => p.FirstName.or).NotEmpty().NotNull();
            // .Matches("^[a-zA-Z']+${1,50}")
            //     .WithMessage("Name should only contain alphabets and apostrophes, and be between 1 and 50 characters long.");
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
            // RuleFor(p => p.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Father.SexLookupId");
            RuleFor(p => p.NationalityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Father.NationalityLookupId");
            RuleFor(p => p.ReligionLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Father.ReligionLookupId");
            RuleFor(p => p.EducationalStatusLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Father.EducationalStatusLookupId");
            RuleFor(p => p.TypeOfWorkLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Father.TypeOfWorkLookupId");
            RuleFor(p => p.MarriageStatusLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Father.MarriageStatusLookupId");
            RuleFor(p => p.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo, "Father.ResidentAddressId");
            RuleFor(p => p.NationLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "Father.NationLookupId");
            RuleFor(p => p.BirthDateEt).NotEmpty().NotNull()
            .IsValidDate("Father Birth date");//.IsAbove18("Father age");
            RuleFor(p => p.PhoneNumber).NotEmpty()
                        .Matches(new Regex(@"^(\+251|0)?(7|9)\d{8}$")).WithMessage("Invalid phone number format.")
                        .When(p => p.PhoneNumber != null);
            // .Must(date => date < DateTime.Now && date > new DateTime(1900, 1, 1));
            RuleFor(p => p.BirthAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo, "Father.BirthAddressId");
        }
    }
}