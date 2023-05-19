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
    public class UpdateCertificateCommand : IRequest<CertificateDTO>
    {

        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public JObject Content { get; set; }
        public bool Status { get; set; }
        public bool AuthenticationStatus { get; set; }
        public int PrintCount { get; set; }
        public string CertificateSerialNumber { get; set; }
    }

    public class UpdateCertificateCommandHandler : IRequestHandler<UpdateCertificateCommand, CertificateDTO>
    {
        private readonly ICertificateRepository _certificateRepository;
        public UpdateCertificateCommandHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }
        public async Task<CertificateDTO> Handle(UpdateCertificateCommand request, CancellationToken cancellationToken)
        {
            var certificate = CustomMapper.Mapper.Map<Certificate>(request);

            try
            {
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