using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
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
        public Guid CivilRegOfficerId { get; set; }
        public JObject Reason { get; set; }
    }

    public class ReprintCertificateCommandHandler : IRequestHandler<ReprintCertificateCommand, CertificateDTO>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ICertificateHistoryRepository _CertificateHistoryRepository;
        public ReprintCertificateCommandHandler(ICertificateRepository certificateRepository, ICertificateHistoryRepository CertificateHistoryRepository)
        {
            _certificateRepository = certificateRepository;
            _CertificateHistoryRepository = CertificateHistoryRepository;
        }
        public async Task<CertificateDTO> Handle(ReprintCertificateCommand request, CancellationToken cancellationToken)
        {

            if (request.IsPrint && !string.IsNullOrEmpty(request.CertificateSerialNumber))
            {
                var certificate = await _certificateRepository.GetAsync(request.Id);
                if (certificate == null)
                {
                    throw new Exception("Certificate With This Id Not Found");
                }
                var AddHistory = new AddCertificateHistoryRequest
                {
                    CerteficateId = request.Id,
                    CivilRegOfficerId = request.CivilRegOfficerId,
                    SrialNo = request.CertificateSerialNumber,
                    Reason = request.Reason

                };
                try
                {
                    var CertificateHistory = CustomMapper.Mapper.Map<CertificateHistory>(AddHistory);
                    await _CertificateHistoryRepository.InsertAsync(CertificateHistory, cancellationToken);
                    var Historyresult = await _CertificateHistoryRepository.SaveChangesAsync(cancellationToken);
                    certificate.PrintCount += 1;
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