using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;


namespace AppDiv.CRVS.Application.Features.MarriageApplications.Command.Update
{
    public class UpdateMarriageApplicationCommandValidator : AbstractValidator<UpdateMarriageApplicationCommand>
    {
        private readonly IMarriageApplicationRepository _repo;
        private readonly IPersonalInfoRepository _personalInfoRepo;

        public UpdateMarriageApplicationCommandValidator(IMarriageApplicationRepository repo , IPersonalInfoRepository personalInfoRepo)
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