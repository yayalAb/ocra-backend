using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;

namespace AppDiv.CRVS.Application.Features.WorkFlows.Commands.Create
{
    public class CreateWorkFlowCommandValidetor : AbstractValidator<CreateWorkFlowCommand>
    {
        private readonly IWorkflowRepository _repo;
        public CreateWorkFlowCommandValidetor(IWorkflowRepository repo)
        {
            _repo = repo;
            // RuleFor(p => p.customer.FirstName)
            //     .NotEmpty().WithMessage("{PropertyName} is required.")
            //     .NotNull()
            //     .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
            //RuleFor(e => e)
            //   .MustAsync(phoneNumberUnique)
            //   .WithMessage("A Customer phoneNumber already exists.");
        }

    }
}