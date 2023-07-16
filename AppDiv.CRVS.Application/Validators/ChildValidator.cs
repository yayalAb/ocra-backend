using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using System.Text.RegularExpressions;
using AppDiv.CRVS.Utility.Services;

namespace AppDiv.CRVS.Application.Validators
{
    public class ChildValidator : AbstractValidator<ChildInfoDTO>
    {
        private readonly IEventRepository _repo;
        public ChildValidator(IEventRepository repo)
        {
            var dateConverter = new CustomDateConverter();
            _repo = repo;
            RuleFor(p => p.Id.ToString()).NotEmpty().NotNull().ForeignKeyWithPerson(_repo, "EventOwener.Id")
                .When(p => !string.IsNullOrEmpty(p.Id.ToString()) && p.Id != Guid.Empty);
            RuleFor(p => p.FirstName.or).NotEmpty().NotNull();
            // .Matches("^[a-zA-Z']+${1,50}")
            //     .WithMessage("Name should only contain alphabets and apostrophes, and be between 1 and 50 characters long.");
            RuleFor(p => p.FirstName.am).NotEmpty().NotNull();
            // RuleFor(p => p.MiddleName.or).Must(f => f == father.FirstName.or).WithMessage("The child's father name and his father first name does not match.").NotEmpty().NotNull();
            // RuleFor(p => p.MiddleName.am).Must(f => f == father.FirstName.am).WithMessage("The child's father's name and his father's first name do not match.").NotEmpty().NotNull();
            // RuleFor(p => p.LastName.or).Must(f => f == father.MiddleName.or).WithMessage("The child's grandfather's name and his father's father's name do not match.").NotEmpty().NotNull();
            // RuleFor(p => p.LastName.am).Must(f => f == father.MiddleName.am).WithMessage("The child's grandfather's name and his father's father's name do not match.").NotEmpty().NotNull();
            RuleFor(p => p.SexLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "SexLookupId")
            .When(p => p.SexLookupId != null);
            // RuleFor(p => p.BirthDateEt).NotEmpty().NotNull().IsValidDate("Child Birth Date");
            RuleFor(p => p.PhoneNumber).NotEmpty()
                        .Matches(new Regex(@"^(\+251)?\d{9}$")).WithMessage("Invalid phone number format.")
                        .When(p => p.PhoneNumber != null);
            RuleFor(p => p.NationalityLookupId.ToString()).NotEmpty().NotNull().ForeignKeyWithLookup(_repo, "NationalityLookupId");
            // .When(p => p.NationalityLookupId != null);
            RuleFor(p => p.BirthAddressId.ToString()).NotEmpty().NotNull().ForeignKeyWithAddress(_repo, "BirthAddressId")
            .When(p => p.BirthAddressId != null);
        }
    }

}