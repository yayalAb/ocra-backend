using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;

namespace AppDiv.CRVS.Application.Features.Courts.Commmands.Create
{//CourtRepository
    public class CreateCourtCommandValidetor : AbstractValidator<CreateCourtCommand>
    {
        private readonly ICourtRepository _repo;
        public CreateCourtCommandValidetor(ICourtRepository repo)
        {
            _repo = repo;
            // RuleFor(p => p.Address.AddressNameStr)
            //     .NotEmpty().WithMessage("{PropertyName} is required.")
            //     .NotNull()
            //     .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
            //RuleFor(e => e)
            //   .MustAsync(phoneNumberUnique)
            //   .WithMessage("A Customer phoneNumber already exists.");
        }
    }
}