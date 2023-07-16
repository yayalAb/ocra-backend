using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using AppDiv.CRVS.Application.Contracts.DTOs;
using System.Text.RegularExpressions;

namespace AppDiv.CRVS.Application.Validators
{
    public class DeadValidator : AbstractValidator<DeadPersonalInfoDTO>
    {
        private readonly IEventRepository _repo;
        public DeadValidator(IEventRepository repo)
        {
            _repo = repo;
            RuleFor(p => p.Id.ToString()).NotEmpty().NotNull().ForeignKeyWithPerson(_repo, "EventOwener.Id")
                    .When(p => !string.IsNullOrEmpty(p.Id.ToString()) && p.Id != Guid.Empty);
            RuleFor(p => p.MiddleName.am).NotEmpty().NotNull();
            RuleFor(p => p.MiddleName.or).NotEmpty().NotNull();
            RuleFor(p => p.LastName.am).NotEmpty().NotNull();
            RuleFor(p => p.LastName.or).NotEmpty().NotNull();
            RuleFor(p => p.FirstName.am).NotEmpty().NotNull();
            RuleFor(p => p.SexLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "SexLookupId")
            .When(p => p.SexLookupId != null);
            RuleFor(p => p.TitleLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "TitleLookupId")
            .When(p => p.TitleLookupId != null);
            RuleFor(p => p.NationalityLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "NationalityLookupId");
            // .When(p => p.SexLookupId != null);
            RuleFor(p => p.ResidentAddressId.ToString()).NotEmpty().NotNull().ForeignKeyWithAddress(_repo, "ResidentAddressId")
            .When(p => p.ResidentAddressId != null);
            RuleFor(p => p.ReligionLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "ReligionLookupId")
            .When(p => p.ReligionLookupId != null);
            RuleFor(p => p.EducationalStatusLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "EducationalStatusLookupId")
            .When(p => p.EducationalStatusLookupId != null);
            RuleFor(p => p.TypeOfWorkLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "TypeOfWorkLookupId")
            .When(p => p.TypeOfWorkLookupId != null);
            RuleFor(p => p.MarriageStatusLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "MarriageStatusLookupId")
            .When(p => p.MarriageStatusLookupId != null);
            RuleFor(p => p.NationLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "NationLookupId")
            .When(p => p.NationLookupId != null);
            RuleFor(p => p.PhoneNumber).NotEmpty()
                        .Matches(new Regex(@"^(\+251)?\d{9}$")).WithMessage("Invalid phone number format.")
                        .When(p => p.PhoneNumber != null);
        }
    }




}