using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Create
{
    public class CreateCertificateTemplateCommandValidator : AbstractValidator<CreateCertificateTemplateCommand>
    {
        private readonly IAddressLookupRepository _repo;
        public CreateCertificateTemplateCommandValidator(IAddressLookupRepository repo)
        {
            _repo = repo;
            RuleFor(p => p.SvgFile)
            .NotEmpty()
            .NotNull()
            .Must(BeSvgFile).WithMessage("the certificate template file must be svg format");
                
        }

        private bool BeSvgFile(IFormFile arg)
        {
            return Path.GetExtension(arg.FileName).ToLower() == ".svg";
        }
    }
}