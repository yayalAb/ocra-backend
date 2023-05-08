using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.MarriageApplications.Command.Create
{
    public class CreateMarriageApplicationCommandValidator : AbstractValidator<CreateMarriageApplicationCommand>
    {
        private readonly IMarriageApplicationRepository _repo;
        private readonly IPersonalInfoRepository _personalInfoRepo;

        public CreateMarriageApplicationCommandValidator(IMarriageApplicationRepository repo , IPersonalInfoRepository personalInfoRepo)
        {
            _repo = repo;
            _personalInfoRepo = personalInfoRepo;
            RuleFor(e => e.ApplicationAddressId)
                .NotEmpty()
                .NotNull()
                .MustAsync(BefoundInAddressDb);
            RuleFor(e => e.CivilRegOfficerId)
                .NotEmpty()
                .NotNull()
                .MustAsync(BefoundInPersonDb);
        }

        private async Task<bool> BefoundInPersonDb(Guid guid, CancellationToken token)
        {
            return (await _personalInfoRepo.GetAsync(guid)!= null);
        }

        private async Task<bool> BefoundInAddressDb(Guid guid, CancellationToken token)
        {
            return (await _repo.GetAsync(guid)) != null;

        }

        
    }
}