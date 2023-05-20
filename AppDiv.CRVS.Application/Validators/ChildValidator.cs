using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;

namespace AppDiv.CRVS.Application.Validators
{
    public class ChildValidator : AbstractValidator<ChildInfoDTO>
    {
        private readonly (ILookupRepository Lookup, IAddressLookupRepository Address) _repo;
        public ChildValidator((ILookupRepository Lookup, IAddressLookupRepository Address) repo, FatherInfoDTO father)
        {
            _repo = repo;
            RuleFor(p => p.FirstName.or).NotEmpty().NotNull();
            RuleFor(p => p.FirstName.am).NotEmpty().NotNull();
            // RuleFor(p => p.MiddleName.or).Must(f => f == father.FirstName.or).WithMessage("The child's father name and his father first name does not match.").NotEmpty().NotNull();
            // RuleFor(p => p.MiddleName.am).Must(f => f == father.FirstName.am).WithMessage("The child's father's name and his father's first name do not match.").NotEmpty().NotNull();
            // RuleFor(p => p.LastName.or).Must(f => f == father.MiddleName.or).WithMessage("The child's grandfather's name and his father's father's name do not match.").NotEmpty().NotNull();
            // RuleFor(p => p.LastName.am).Must(f => f == father.MiddleName.am).WithMessage("The child's grandfather's name and his father's father's name do not match.").NotEmpty().NotNull();
            RuleFor(p => p.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "SexLookupId");
            RuleFor(p => p.BirthDate).NotEmpty().NotNull();
            RuleFor(p => p.NationalityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "NationalityLookupId");
            RuleFor(p => p.BirthAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "BirthAddressId");
        }
    }
    // public class DeadValidator : AbstractValidator<DeadInfoDTO>
    // {
    //     private readonly (ILookupRepository Lookup, IAddressLookupRepository Address) _repo;
    //     public DeadValidator((ILookupRepository Lookup, IAddressLookupRepository Address) repo, FatherInfoDTO father)
    //     {
    //         _repo = repo;
    //         RuleFor(p => p.FirstName.or).NotEmpty().NotNull();
    //         RuleFor(p => p.FirstName.am).NotEmpty().NotNull();
    //         // RuleFor(p => p.MiddleName.or).Must(f => f == father.FirstName.or).WithMessage("The child's father name and his father first name does not match.").NotEmpty().NotNull();
    //         // RuleFor(p => p.MiddleName.am).Must(f => f == father.FirstName.am).WithMessage("The child's father's name and his father's first name do not match.").NotEmpty().NotNull();
    //         // RuleFor(p => p.LastName.or).Must(f => f == father.MiddleName.or).WithMessage("The child's grandfather's name and his father's father's name do not match.").NotEmpty().NotNull();
    //         // RuleFor(p => p.LastName.am).Must(f => f == father.MiddleName.am).WithMessage("The child's grandfather's name and his father's father's name do not match.").NotEmpty().NotNull();
    //         RuleFor(p => p.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "SexLookupId");
    //         RuleFor(p => p.BirthDate).NotEmpty().NotNull();
    //         RuleFor(p => p.NationalityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "NationalityLookupId");
    //         RuleFor(p => p.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "ResidentAddressId");
    //     }
    // }




}