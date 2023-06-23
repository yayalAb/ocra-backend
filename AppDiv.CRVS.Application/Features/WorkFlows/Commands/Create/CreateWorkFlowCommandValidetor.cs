using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

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
            RuleFor(p => p.workflow.workflowName)
                .MustAsync(ValidateWorkflowname)
                .WithMessage("{PropertyName}  must be unique");
        }

        private async Task<bool> ValidateWorkflowname(string name, CancellationToken token)
        {
            var address = _repo.GetAll()
            .Include(x => x.Steps)
            .Where(x => x.workflowName == name).FirstOrDefault();
            if (address == null || address.Steps.FirstOrDefault() == null)
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