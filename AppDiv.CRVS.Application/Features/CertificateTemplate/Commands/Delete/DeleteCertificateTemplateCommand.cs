using AppDiv.CRVS.Application.Features.AddressLookup.Commands.Delete;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.CertificateTemplateLookup.Commands.Delete
{
    // Customer create command with string response
    public class DeleteCertificateTemplateCommand : IRequest<DeleteCertificateTemplateResponse>
    {
        public Guid Id { get; set; }

    }

    // Customer delete command handler with string response as output
    public class DeleteCertificateTemplateCommandHandler : IRequestHandler<DeleteCertificateTemplateCommand, DeleteCertificateTemplateResponse>
    {
        private readonly ICertificateTemplateRepository _CertificateTemplateRepository;
        public DeleteCertificateTemplateCommandHandler(ICertificateTemplateRepository CertificateTemplateRepository)
        {
            _CertificateTemplateRepository = CertificateTemplateRepository;
        }

        public async Task<DeleteCertificateTemplateResponse> Handle(DeleteCertificateTemplateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var CertificateTemplateEntity = await _CertificateTemplateRepository.GetAsync(request.Id);

                await _CertificateTemplateRepository.DeleteAsync(CertificateTemplateEntity);
                await _CertificateTemplateRepository.SaveChangesAsync(cancellationToken);
                //TODO: Delete the file
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return new DeleteCertificateTemplateResponse{
                Success = true,
                Message =$"Successfuly deleted Certificate template with id {request.Id}",
            };
        }
    }
}
