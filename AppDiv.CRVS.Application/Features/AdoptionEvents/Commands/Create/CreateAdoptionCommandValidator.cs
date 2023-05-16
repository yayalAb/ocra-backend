using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;

namespace AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Create
{
    public class CreatAdoptionCommandValidator : AbstractValidator<CreateAdoptionCommand>
    {
        private readonly IAdoptionEventRepository _repo;
        private readonly IAddressLookupRepository _address;
        public CreatAdoptionCommandValidator(IAdoptionEventRepository repo, IAddressLookupRepository address)
        {
            _repo = repo;
            _address = address;
            RuleFor(p => p.Adoption.ApprovedName.am)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.");
            RuleFor(p => p.Adoption.ApprovedName.or)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.");
            RuleFor(p => p.Adoption.AdoptiveFather.FirstName.am)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.");
            RuleFor(p => p.Adoption.AdoptiveFather.FirstName.or)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.");
            RuleFor(p => p.Adoption.Event.EventSupportingDocuments)
                .NotNull()
                .WithMessage("{PropertyName} is required.")
                .NotEmpty()
                .WithMessage("{PropertyName} must not be empty.");
            RuleFor(e => e)
              .MustAsync(ValidateForignkeyAddress)
              .WithMessage("A address does not  exists.");

        }
        private async Task<bool> ValidateForignkeyAddress(CreateAdoptionCommand request, CancellationToken token)
        {
            var address = await _address.GetByIdAsync(request.Adoption.BeforeAdoptionAddressId);
            if (address == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}




