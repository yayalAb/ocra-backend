using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Create
{
    public class CreateAdderssCommandValidator : AbstractValidator<CreateAdderssCommand>
    {
        private readonly IAddressLookupRepository _repo;
        public CreateAdderssCommandValidator(IAddressLookupRepository repo)
        {
            _repo = repo;
            RuleFor(p => p.Address.AddressName)
                .NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(p => p.Address.StatisticCode)
                .MustAsync(ValidateStatisticCode)
                .WithMessage("{PropertyName} is must be unique.");
        }


        private async Task<bool> ValidateForignkeyLookups(string code, CancellationToken token)
        {
            var address = _repo.GetAll().Where(x => x.StatisticCode == code);
            if (address == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private async Task<bool> ValidateStatisticCode(string code, CancellationToken token)
        {
            var address = _repo.GetAll().Where(x => x.StatisticCode == code);
            if (address == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}