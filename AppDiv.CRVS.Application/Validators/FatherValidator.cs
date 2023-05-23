using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;

namespace AppDiv.CRVS.Application.Validators
{
    public class FatherValidator : AbstractValidator<FatherInfoDTO>
    {
        private readonly (ILookupRepository Lookup, IAddressLookupRepository Address) _repo;
        public FatherValidator((ILookupRepository Lookup, IAddressLookupRepository Address) repo)
        {
            _repo = repo;
            RuleFor(p => p.FirstName.or).NotEmpty().NotNull();
            RuleFor(p => p.FirstName.am).NotEmpty().NotNull();
            RuleFor(p => p.MiddleName.am).NotEmpty().NotNull();
            RuleFor(p => p.MiddleName.or).NotEmpty().NotNull();
            RuleFor(p => p.LastName.am).NotEmpty().NotNull();
            RuleFor(p => p.LastName.or).NotEmpty().NotNull();
            RuleFor(p => p.NationalId).NotGuidEmpty();
            RuleFor(p => p.SexLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Father.SexLookupId");
            RuleFor(p => p.NationalityLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Father.NationalityLookupId");
            RuleFor(p => p.ReligionLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Father.ReligionLookupId");
            RuleFor(p => p.EducationalStatusLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Father.EducationalStatusLookupId");
            RuleFor(p => p.TypeOfWorkLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Father.TypeOfWorkLookupId");
            RuleFor(p => p.MarriageStatusLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Father.MarriageStatusLookupId");
            RuleFor(p => p.ResidentAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "Father.ResidentAddressId");
            RuleFor(p => p.NationLookupId.ToString()).NotGuidEmpty().ForeignKeyWithLookup(_repo.Lookup, "Father.NationLookupId");
            RuleFor(p => p.BirthDateEt).NotEmpty().NotNull()
            .IsValidDate("Father Birth date");
            // .Must(date => date < DateTime.Now && date > new DateTime(1900, 1, 1));
            RuleFor(p => p.BirthAddressId.ToString()).NotGuidEmpty().ForeignKeyWithAddress(_repo.Address, "Father.BirthAddressId");
        }
    }
}