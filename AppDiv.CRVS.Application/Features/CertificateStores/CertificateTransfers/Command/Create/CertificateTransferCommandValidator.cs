using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.CertificateStores.CertificateTransfers.Command.Create
{
    public class CreateCertificateTransferCommandValidator : AbstractValidator<CreateCertificateTransferCommand>
    {
        private readonly ICertificateTransferRepository _repo;
        // private readonly IMediator _mediator;
        public CreateCertificateTransferCommandValidator(ICertificateTransferRepository repo)
        {

            _repo = repo;
            // _mediator = mediator;
            // RuleFor(p => p.CertificateTransfer.PaymentTypeLookupId)
            //     .Must(x => x != Guid.Empty).WithMessage("Payment Type must not be empty.");
            // // .NotEmpty().WithMessage("{PropertyName} is required.")
            // // .NotNull().
            // // .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
            // RuleFor(p => p.CertificateTransfer.EventLookupId)
            //     .Must(x => x != Guid.Empty).WithMessage("Event must not be empty.");
            // RuleFor(pr => pr.CertificateTransfer.Amount)
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