using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Features.Certificates.Command.Create;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Contracts;
using AppDiv.CRVS.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Application.Features.Certificates.Query.Check;
using AppDiv.CRVS.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.Certificates.Query
{
    public class CertificateResponseDTO
    {
        public EventImagesDTO Images { get; set; } = null;
        public JObject Content { get; set; }
        public Guid? TemplateId { get; set; }

    }

    // Customer GenerateCustomerQuery with Customer response
    public class GenerateCertificateQuery : IRequest<object>
    {
        public Guid Id { get; set; }
        // public bool Status { get; set; }
        // public bool AuthenticationStatus { get; set; }
        public string? CertificateSerialNumber { get; set; }
        public bool IsPrint { get; set; } = false;
        // public Guid UserId { get; set; } = new Guid("134b4daa-bfac-445d-bd45-a83048eada3b");
        public bool CheckSerialNumber { get; set; } = true;

        // public GenerateCertificateQuery(Guid Id, string SerialNumber)
        // {
        //     this.Id = Id;
        //     this.CertificateSerialNumber = SerialNumber;
        // }

    }

    public class GenerateCertificateHandler : IRequestHandler<GenerateCertificateQuery, object>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IBirthEventRepository _IBirthEventRepository;
        private readonly IMediator _mediator;
        private readonly ICertificateTemplateRepository _ICertificateTemplateRepository;
        private readonly ICertificateGenerator _CertificateGenerator;
        private readonly ILogger<GenerateCertificateHandler> _ILogger;
        private readonly IFileService _fileService;
        private readonly IUserResolverService _userResolverService;
        private readonly IVerficationRequestRepository _VerficationRequestRepository;
        private readonly IWorkflowRepository _WorkflowRepository;
        private readonly IWorkflowService _WorkflowService;
        private readonly IEventPaymentRequestService _paymentRequestService;

        // private readonly IMediator _mediator;
        private readonly ISupportingDocumentRepository _supportingDocumentRepository;


        public GenerateCertificateHandler(ILogger<GenerateCertificateHandler> ILogger,
                                        ICertificateGenerator CertificateGenerator,
                                        IBirthEventRepository IBirthEventRepository,
                                        ICertificateTemplateRepository ICertificateTemplateRepository,
                                        ICertificateRepository CertificateRepository,
                                        IMediator mediator,
                                        IEventRepository eventRepository,
                                        IFileService fileService,
                                        IUserResolverService userResolverService,
                                        IVerficationRequestRepository VerficationRequestRepository,
                                        IWorkflowRepository WorkflowRepository,
                                        IWorkflowService WorkflowService,
                                        IEventPaymentRequestService paymentRequestService,
                                        // IMediator mediator,
                                        ISupportingDocumentRepository supportingDocumentRepository)
        {
            _certificateRepository = CertificateRepository;
            _eventRepository = eventRepository;
            _userResolverService = userResolverService;
            _ICertificateTemplateRepository = ICertificateTemplateRepository;
            _IBirthEventRepository = IBirthEventRepository;
            _CertificateGenerator = CertificateGenerator;
            _ILogger = ILogger;
            _mediator = mediator;
            _fileService = fileService;
            _supportingDocumentRepository = supportingDocumentRepository;
            _VerficationRequestRepository = VerficationRequestRepository;
            _WorkflowRepository = WorkflowRepository;
            _WorkflowService = WorkflowService;
            _paymentRequestService = paymentRequestService;
        }
        public async Task<object> Handle(GenerateCertificateQuery request, CancellationToken cancellationToken)
        {
            var errorResponse = new BaseResponse();
            var response = new CertificateResponseDTO();
            if (request.CheckSerialNumber)
            {
                errorResponse = await _mediator.Send(new CheckSerialNoValidation { CertificateSerialNumber = request.CertificateSerialNumber, UserId = _userResolverService.GetUserId() });
            }
            if (errorResponse.Status != 200)
            {
                return errorResponse;
            }
            var selectedEvent = await _eventRepository.GetByIdAsync(request.Id);
            // if (selectedEvent.ReprintWaiting)
            // {
            //     throw new NotFoundException("please request it on reprint");
            // }


            var birthCertificateNo = _IBirthEventRepository
                                    .GetAll()
                                    .Where(x => x.Event.EventOwenerId == selectedEvent.EventOwenerId)
                                    .Select(x => x.Event.CertificateId)
                                    .FirstOrDefault();
            var content = await _certificateRepository.GetContent(request.Id);
            var certificate = _CertificateGenerator.GetCertificate(request, content, birthCertificateNo);
            var certificateTemplateId = _ICertificateTemplateRepository.GetAll().Where(c => c.CertificateType == selectedEvent.EventType).Select(c => c.Id).FirstOrDefault();
            if (request.IsPrint && !string.IsNullOrEmpty(request.CertificateSerialNumber))
            {
                var findPerCertificates = _certificateRepository.GetAll().Where(x => x.EventId == request.Id).ToList();
                foreach (var item in findPerCertificates)
                {
                    item.Status = false;
                    await _certificateRepository.UpdateAsync(item, x => x.Id);
                }
                var Workflow = _WorkflowRepository.GetAll()
                .Include(x => x.Steps)
                .Where(wf => wf.workflowName == "verification").FirstOrDefault();
                if (Workflow==null|| (Workflow.Steps.FirstOrDefault() == null))
                {
                    (float amount, string code) payment = await _paymentRequestService.CreatePaymentRequest(selectedEvent.EventType, selectedEvent, "verification", null, false, false, cancellationToken);
                    if (payment.amount == 0 || payment.amount == 0.0)
                    {

                        selectedEvent.IsVerified = true;
                        await _eventRepository.UpdateAsync(selectedEvent, x => x.Id);
                    }
                }
                else
                {
                    var next = _WorkflowService.GetNextStep("verification", 0, true);
                    var verficationRequest = new VerficationRequest
                    {
                        EventId = selectedEvent.Id,
                        Request = new Request
                        {
                            RequestType = "verification",
                            CivilRegOfficerId = selectedEvent.CivilRegOfficerId,
                            currentStep = 0,
                            NextStep = next,
                            WorkflowId = Workflow.Id,
                        }
                    };
                    await _VerficationRequestRepository.InsertAsync(verficationRequest, cancellationToken);

                }

                selectedEvent.IsCertified = true;
                selectedEvent.OnReprintPaymentRequest = false;
                selectedEvent.ReprintWaiting = false;
                _eventRepository.Update(CustomMapper.Mapper.Map<Event>(selectedEvent));
                await _certificateRepository.InsertAsync(certificate, cancellationToken);
                var result = await _certificateRepository.SaveChangesAsync(cancellationToken);
            }
            if (selectedEvent.EventType.ToLower() == "marriage" || content.marriage != null)
            {
                (string Bride, string Groom) image = _supportingDocumentRepository.MarriageImage();
                response.Images = new EventImagesDTO
                {
                    BrideImage = $"File?id={image.Bride}&fileType=SupportingDocuments&eventType=Marriage",
                    GroomImage = $"File?id={image.Groom}&fileType=SupportingDocuments&eventType=Marriage",
                };
            }
            response.Content = certificate.Content;
            response.TemplateId = certificateTemplateId;
            return response;
        }

    }
}