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
            RuleFor(p => p.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo, "EventOwener.Id")
                    .When(p => !string.IsNullOrEmpty(p.Id.ToString()) && p.Id != Guid.Empty);
            RuleFor(p => p.FirstName.or).NotEmpty().NotNull();//.Matches("^[a-zA-Z']+${1,50}")
                // .WithMessage("Name should only contain alphabets and apostrophes, and be between 1 and 50 characters long.");
            RuleFor(p => p.MiddleName.am).NotEmpty().NotNull();
            RuleFor(p => p.MiddleName.or).NotEmpty().NotNull();
            // .Matches("^[a-zA-Z']+${1,50}")
            //     .WithMessage("Name should only contain alphabets and apostrophes, and be between 1 and 50 characters long.");
            RuleFor(p => p.LastName.am).NotEmpty().NotNull();
            RuleFor(p => p.LastName.or).NotEmpty().NotNull();
            // .Matches("^[a-zA-Z']+${1,50}")
            //     .WithMessage("Name should only contain alphabets and apostrophes, and be between 1 and 50 characters long.");
            RuleFor(p => p.FirstName.am).NotEmpty().NotNull();
            RuleFor(p => p.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "SexLookupId");
            RuleFor(p => p.TitleLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "TitleLookupId");
            RuleFor(p => p.NationalityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "NationalityLookupId");
            RuleFor(p => p.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo, "ResidentAddressId");
            RuleFor(p => p.ReligionLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "ReligionLookupId");
            RuleFor(p => p.EducationalStatusLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "EducationalStatusLookupId");
            RuleFor(p => p.TypeOfWorkLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "TypeOfWorkLookupId");
            RuleFor(p => p.MarriageStatusLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "MarriageStatusLookupId");
            RuleFor(p => p.NationLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "NationLookupId");
            RuleFor(p => p.PhoneNumber).NotEmpty()
                        .Matches(new Regex(@"^(\+251)?\d{9}$")).WithMessage("Invalid phone number format.")
                        .When(p => p.PhoneNumber != null);
        }
    }




}