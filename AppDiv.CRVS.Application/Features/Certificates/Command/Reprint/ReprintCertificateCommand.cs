using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Features.Certificates.Query.Check;

namespace AppDiv.CRVS.Application.Features.Certificates.Command.Update
{


    // Customer create command with CustomerResponse
    public class ReprintCertificateCommand : IRequest<object>
    {

        public Guid Id { get; set; }
        public bool IsPrint { get; set; } = false;
        public string? CertificateSerialNumber { get; set; } = "";
        public Guid CivilRegOfficerId { get; set; }
        // public Guid UserId { get; set; } = new Guid("134b4daa-bfac-445d-bd45-a83048eada3b");
        public JObject? Reason { get; set; }
        public bool CheckSerialNumber { get; set; } = true;
    }

    public class ReprintCertificateCommandHandler : IRequestHandler<ReprintCertificateCommand, object>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ICertificateHistoryRepository _CertificateHistoryRepository;
        private readonly ICertificateTemplateRepository _ICertificateTemplateRepository;
        private readonly IMediator _mediator;
        private readonly IUserResolverService _userResolverService;
        public ReprintCertificateCommandHandler(ICertificateTemplateRepository ICertificateTemplateRepository,
                                                ICertificateRepository certificateRepository,
                                                ICertificateHistoryRepository CertificateHistoryRepository,
                                                IMediator mediator,
                                                IUserResolverService userResolverService)
        {
            _certificateRepository = certificateRepository;
            _CertificateHistoryRepository = CertificateHistoryRepository;
            _ICertificateTemplateRepository = ICertificateTemplateRepository;
            _userResolverService = userResolverService;
            _mediator = mediator;
        }
        public async Task<object> Handle(ReprintCertificateCommand request, CancellationToken cancellationToken)
        {
            var errorResponse = new BaseResponse();
            if (request.CheckSerialNumber)
            {
                errorResponse = await _mediator.Send(new CheckSerialNoValidation { CertificateSerialNumber = request.CertificateSerialNumber, UserId = _userResolverService.GetUserId() });
            }
            if (errorResponse.Status != 200)
            {
                return errorResponse;
            }
            string certId = "";
            var certificate = _certificateRepository.GetAll()
            .Include(x => x.Event)
            .Where(x => x.Id == request.Id).FirstOrDefault();
            if (certificate == null)
            {
                throw new Exception("Certificate With This Id Not Found");
            }
            var certificateTemplateId = _ICertificateTemplateRepository.GetAll()
            .Where(c => c.CertificateType == certificate.Event.EventType)
            .FirstOrDefault();
            certId = certificateTemplateId.Id.ToString();
            if (request.IsPrint && !string.IsNullOrEmpty(request.CertificateSerialNumber))
            {

                var AddHistory = new AddCertificateHistoryRequest
                {
                    CerteficateId = request.Id,
                    CivilRegOfficerId = request.CivilRegOfficerId,
                    SrialNo = request.CertificateSerialNumber,
                    Reason = request.Reason,
                    PrintType = "Certificate"

                };
                try
                {
                    var CertificateHistory = CustomMapper.Mapper.Map<CertificateHistory>(AddHistory);
                    await _CertificateHistoryRepository.InsertAsync(CertificateHistory, cancellationToken);
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
            return new
            {
                Content = CertificateResponse.Content,
                certificateTemplateId = certId
            };
        }
    }
}