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
            RuleFor(p => p.workflow.workflowName)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
        }

    }
}