using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Groups.Commands.Create
{
    public class CreateGroupComadValidetor : AbstractValidator<CreateGroupCommand>
    {
        private readonly IGroupRepository _repo;
        public CreateGroupComadValidetor(IGroupRepository repo)
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

        //private async Task<bool> phoneNumberUnique(CreateCustomerCommand request, CancellationToken token)
        //{
        //    var member = await _repo.GetByIdAsync(request.FirstName);
        //    if (member == null)
        //        return true;
        //    else return false;
        //}

    }
}