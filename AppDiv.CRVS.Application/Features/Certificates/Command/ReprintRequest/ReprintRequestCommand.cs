using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Certificates.Command.ReprintRequest
{
    // Customer create command with CustomerResponse
    public class ReprintRequestCommand : IRequest<CertificateDTO>
    {

        public Guid Id { get; set; }
    }

    public class ReprintRequestCommandHandler : IRequestHandler<ReprintRequestCommand, CertificateDTO>
    {
        private readonly ICertificateRepository _certificateRepository;
        public ReprintRequestCommandHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }
        public async Task<CertificateDTO> Handle(ReprintRequestCommand request, CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetByIdAsync(request.Id);
            if (certificate == null)
            {
                throw new Exception("Certificate not Found!");
            }

            try
            {
                certificate.OnReprintPaymentRequest = true;

                await _certificateRepository.UpdateAsync(certificate, x => x.Id);
                var result = await _certificateRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            var modifiedCertificate = await _certificateRepository.GetAsync(request.Id);
            var CertificateResponse = CustomMapper.Mapper.Map<CertificateDTO>(modifiedCertificate);

            return CertificateResponse;
        }
    }
}