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
        public string? serialNo { get; set; } = "";
        public Guid CivilRegOfficerId { get; set; }
        // public Guid UserId { get; set; } = new Guid("134b4daa-bfac-445d-bd45-a83048eada3b");
        public JObject? Reason { get; set; }
        public Guid? ReasonLookupId { get; set; }

        public bool CheckSerialNumber { get; set; } = true;
    }

    public class ReprintCertificateCommandHandler : IRequestHandler<ReprintCertificateCommand, object>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ICertificateHistoryRepository _CertificateHistoryRepository;
        private readonly ICertificateTemplateRepository _ICertificateTemplateRepository;
        private readonly IMediator _mediator;
        private readonly IUserResolverService _userResolverService;
        private readonly IEventRepository _EventRepository;
        public ReprintCertificateCommandHandler(IEventRepository EventRepository, ICertificateTemplateRepository ICertificateTemplateRepository,
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
            _EventRepository = EventRepository;
        }
        public async Task<object> Handle(ReprintCertificateCommand request, CancellationToken cancellationToken)
        {
            var errorResponse = new BaseResponse();
            if (request.CheckSerialNumber)
            {
                errorResponse = await _mediator.Send(new CheckSerialNoValidation { CertificateSerialNumber = request.serialNo, UserId = _userResolverService.GetUserId() });
            }
            if (errorResponse.Status != 200)
            {
                return errorResponse;
            }
            string certId = "";
            var cert = _EventRepository.GetAll()
            .Include(c => c.EventCertificates.OrderByDescending(c => c.CreatedAt))
            .Where(e => e.Id == request.Id).FirstOrDefault();

            var certificate = _certificateRepository.GetAll()
            .Include(x => x.Event)
            .Where(x => x.Id == cert.EventCertificates.FirstOrDefault().Id).FirstOrDefault();
            if (certificate == null)
            {
                throw new Exception("Certificate With This Id Not Found");
            }
            var certificateTemplateId = _ICertificateTemplateRepository.GetAll()
            .Where(c => c.CertificateType == certificate.Event.EventType)
            .FirstOrDefault();
            certId = certificateTemplateId.Id.ToString();
            if (request.IsPrint && !string.IsNullOrEmpty(request.serialNo))
            {
                var findPerCertificates = _certificateRepository.GetAll()
                .Where(x => x.EventId == request.Id && x.Status).ToList();
                foreach (var item in findPerCertificates)
                {
                    item.Status = false;
                    await _certificateRepository.UpdateAsync(item, x => x.Id);
                }

                var AddHistory = new AddCertificateHistoryRequest
                {
                    CerteficateId = cert.EventCertificates.FirstOrDefault().Id,
                    CivilRegOfficerId = request.CivilRegOfficerId,
                    SrialNo = request.serialNo,
                    ReasonLookupId = request.ReasonLookupId,
                    Reason = request.Reason,
                    PrintType = "Certificate"

                };
                try
                {
                    var CertificateHistory = CustomMapper.Mapper.Map<CertificateHistory>(AddHistory);
                    await _CertificateHistoryRepository.InsertAsync(CertificateHistory, cancellationToken);
                    certificate.PrintCount += 1;
                    certificate.Status = true;
                    await _certificateRepository.UpdateAsync(certificate, x => x.Id);
                    var even = await _EventRepository.GetAsync(certificate.EventId);
                    even.OnReprintPaymentRequest = false;
                    even.ReprintWaiting = false;
                    await _EventRepository.UpdateAsync(even, x => x.Id);
                    var result = await _certificateRepository.SaveChangesAsync(cancellationToken);
                }
                catch (Exception exp)
                {
                    throw new ApplicationException(exp.Message);
                }
            }
            var modifiedCertificate = await _certificateRepository.GetAsync(cert?.EventCertificates?.FirstOrDefault()?.Id);
            var CertificateResponse = CustomMapper.Mapper.Map<CertificateDTO>(modifiedCertificate);
            return new
            {
                Content = CertificateResponse.Content,
                TemplateId = certId
            };
        }
    }
}