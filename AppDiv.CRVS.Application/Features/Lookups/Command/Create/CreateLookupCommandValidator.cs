using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Lookups.Command.Create
{
    public class CreateLookupCommandValidator : AbstractValidator<CreateLookupCommand>
    {
        private readonly ILookupRepository _repo;
        public CreateLookupCommandValidator(ILookupRepository repo)
        {
            _repo = repo;
            RuleFor(p => p.lookup.Key)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
        }


    }
}