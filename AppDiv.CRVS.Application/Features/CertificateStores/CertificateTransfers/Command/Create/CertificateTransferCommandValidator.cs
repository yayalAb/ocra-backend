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
            RuleFor(p => p.CertificateTransfer.RecieverId).NotNull().NotEmpty();
            RuleFor(p => p.CertificateTransfer.SenderId).NotNull().NotEmpty()
                            .When(p => string.IsNullOrEmpty(p.CertificateTransfer.ReceivedFrom));
            RuleFor(p => p.CertificateTransfer.From).NotNull().NotEmpty()
                            .Must(p => false).WithMessage("'From' must be less than 'To'")
                            .When(p => p.CertificateTransfer.To < p.CertificateTransfer.From);
            RuleFor(p => p.CertificateTransfer.To).NotNull().NotEmpty();

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