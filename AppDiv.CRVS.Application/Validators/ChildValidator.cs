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
            RuleFor(p => p.Id.ToString()).NotGuidEmpty().ForeignKeyWithPerson(_repo, "EventOwener.Id")
                .When(p => !string.IsNullOrEmpty(p.Id.ToString()) && p.Id != Guid.Empty);
            RuleFor(p => p.FirstName.or).NotEmpty().NotNull();
            RuleFor(p => p.FirstName.am).NotEmpty().NotNull();
            // RuleFor(p => p.MiddleName.or).Must(f => f == father.FirstName.or).WithMessage("The child's father name and his father first name does not match.").NotEmpty().NotNull();
            // RuleFor(p => p.MiddleName.am).Must(f => f == father.FirstName.am).WithMessage("The child's father's name and his father's first name do not match.").NotEmpty().NotNull();
            // RuleFor(p => p.LastName.or).Must(f => f == father.MiddleName.or).WithMessage("The child's grandfather's name and his father's father's name do not match.").NotEmpty().NotNull();
            // RuleFor(p => p.LastName.am).Must(f => f == father.MiddleName.am).WithMessage("The child's grandfather's name and his father's father's name do not match.").NotEmpty().NotNull();
            RuleFor(p => p.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "SexLookupId");
            RuleFor(p => p.BirthDateEt).NotEmpty().NotNull().IsValidDate("Child Birth Date");
            RuleFor(p => p.PhoneNumber).NotEmpty()
                        .Matches(new Regex(@"^(\+251|0)?(7|9)\d{8}$")).WithMessage("Invalid phone number format.")
                        .When(p => p.PhoneNumber != null);
            RuleFor(p => p.NationalityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo, "NationalityLookupId");
            RuleFor(p => p.BirthAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo, "BirthAddressId");
        }
    }

}