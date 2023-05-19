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

namespace AppDiv.CRVS.Application.Features.Certificates.Command.Update
{
    // Customer create command with CustomerResponse
    public class ReprintCertificateCommand : IRequest<CertificateDTO>
    {

        public Guid Id { get; set; }
        public bool IsPrint { get; set; } = false;
        public string? CertificateSerialNumber { get; set; } = "";
    }

    public class ReprintCertificateCommandHandler : IRequestHandler<ReprintCertificateCommand, CertificateDTO>
    {
        private readonly ICertificateRepository _certificateRepository;
        public ReprintCertificateCommandHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }
        public async Task<CertificateDTO> Handle(ReprintCertificateCommand request, CancellationToken cancellationToken)
        {

            if (request.IsPrint && !string.IsNullOrEmpty(request.CertificateSerialNumber))
            {
                var certificate = await _certificateRepository.GetAsync(request.Id);
                certificate.PrintCount += 1;
                certificate.CertificateSerialNumber = certificate.CertificateSerialNumber + ", " + request.CertificateSerialNumber;
                try
                {
                    await _certificateRepository.UpdateAsync(certificate, x => x.Id);
                    var result = await _certificateRepository.SaveChangesAsync(cancellationToken);
                }
                catch (Exception exp)
                {
                    throw new ApplicationException(exp.Message);
                }
            }
            var modifiedCertificate = await _certificateRepository.GetAsync(request.Id);
            var CertificateResponse = CustomMapper.Mapper.Map<CertificateDTO>(modifiedCertificate);

            return CertificateResponse;
        }
    }
}