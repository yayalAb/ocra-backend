using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Create
{
    public class CreateBirthEventCommandValidator : AbstractValidator<CreateBirthEventCommand>
    {
        private readonly IBirthEventRepository _repo;
        // private readonly IMediator _mediator;
        public CreateBirthEventCommandValidator(IBirthEventRepository repo)
        {

            _repo = repo;
            // _mediator = mediator;
            // RuleFor(p => p.BirthEvent.Event.EventOwener.FirstName)
            //     // .Must(x => x != Guid.Empty).WithMessage("Payment Type must not be empty.");
            // .NotEmpty().WithMessage("{PropertyName} is required.")
            // .NotNull()
            // .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
            // RuleFor(p => p.BirthEvent.Event.EventOwener.FirstName)
            //     // .Must(x => x != Guid.Empty).WithMessage("Payment Type must not be empty.");
            // .NotEmpty().WithMessage("{PropertyName} is required.")
            // .NotNull()
            // .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
            // RuleFor(p => p.BirthEvent.EventLookupId)
            //     .Must(x => x != Guid.Empty).WithMessage("Event must not be empty.");

            // RuleFor(p => p.BirthEvent.AddressId)
            //     .Must(x => x != Guid.Empty).WithMessage("Address must not be empty.");
            // RuleFor(pr => pr.BirthEvent.Amount)
            //     .NotEmpty().WithMessage("{PropertyName} is required.")
            //     .NotNull();

            // RuleFor(e => e)
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