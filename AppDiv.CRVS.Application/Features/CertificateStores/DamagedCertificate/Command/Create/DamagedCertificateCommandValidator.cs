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
    public class CreateDamagedCertificatesCommandValidator : AbstractValidator<CreateDamagedCertificatesCommand>
    {
        private readonly ICertificateRangeRepository _repo;
        // private readonly IMediator _mediator;
        public CreateDamagedCertificatesCommandValidator(ICertificateRangeRepository repo)
        {

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