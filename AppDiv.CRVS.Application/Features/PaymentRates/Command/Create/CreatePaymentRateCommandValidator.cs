using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.PaymentRates.Command.Create
{
    public class CreatePaymentRateCommandValidator : AbstractValidator<CreatePaymentRateCommand>
    {
        private readonly IPaymentRateRepository _repo;
        // private readonly IMediator _mediator;
        public CreatePaymentRateCommandValidator(IPaymentRateRepository repo)
        {

            _repo = repo;
            // _mediator = mediator;
            RuleFor(p => p.PaymentRate.PaymentTypeLookupId)
                .Must(x => x != Guid.Empty).WithMessage("Payment Type must not be empty.");
            // .NotEmpty().WithMessage("{PropertyName} is required.")
            // .NotNull().
            // .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
            RuleFor(p => p.PaymentRate.EventLookupId)
                .Must(x => x != Guid.Empty).WithMessage("Event must not be empty.");
            RuleFor(pr => pr.PaymentRate.Amount)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull();
            RuleFor(pr => pr.PaymentRate.Backlog)
                    .NotEmpty().WithMessage("{PropertyName} is required.")
                    .NotNull();
            RuleFor(e => e)
              .MustAsync(PaymentRateDuplicationCheck)
              .WithMessage("The specified Payment Rate  already exists.");
        }

        private async Task<bool> PaymentRateDuplicationCheck(CreatePaymentRateCommand request, CancellationToken token)
        {
            var member = _repo.GetAll()
            .Where(x => ((x.PaymentTypeLookup.Id == request.PaymentRate.PaymentTypeLookupId) &&
            (x.EventLookupId == request.PaymentRate.EventLookupId)) && (x.IsForeign == request.PaymentRate.IsForeign)).FirstOrDefault();
            if (member == null)
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