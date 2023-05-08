using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Certificates.Command.Delete
{
    // Customer create command with string response
    public class DeleteCertificateCommand : IRequest<String>
    {
        public Guid Id { get; private set; }

        public DeleteCertificateCommand(Guid Id)
        {
            this.Id = Id;
        }
    }

    // Customer delete command handler with string response as output
    public class DeleteCertificateCommmandHandler : IRequestHandler<DeleteCertificateCommand, String>
    {
        private readonly ICertificateRepository _CertificateRepository;
        public DeleteCertificateCommmandHandler(ICertificateRepository CertificateRepository)
        {
            _CertificateRepository = CertificateRepository;
        }

        public async Task<string> Handle(DeleteCertificateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var CertificateEntity = await _CertificateRepository.GetAsync(request.Id);
                if (CertificateEntity != null)
                {
                    await _CertificateRepository.DeleteAsync(request.Id);
                    await _CertificateRepository.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    return "There is no Certificate with the specified id";
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Certificate information has been deleted!";
        }
    }
}