using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;
using Newtonsoft.Json.Linq;


namespace AppDiv.CRVS.Application.Features.Certificates.Command.Verify
{

    public class VerifyCertificateCommand : IRequest<BaseResponse>
    {
        public Guid EventId { get; set; }
        public bool IsApprove { get; set; }
        public string? Comment { get; set; }
        public JArray? RejectionReasons { get; set; }
        public Guid? ReasonLookupId { get; set; }
    }

    public class VerifyCertificateCommandHandler : IRequestHandler<VerifyCertificateCommand, BaseResponse>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IVerficationRequestRepository _VerficationRequestRepository;
        private readonly ICertificateRepository _CertificateRepository;
        private readonly IWorkflowService _WorkflowService;
        public VerifyCertificateCommandHandler(IEventRepository eventRepository,
        IWorkflowService WorkflowService,
        ICertificateRepository CertificateRepository,
        IVerficationRequestRepository VerficationRequestRepository)
        {
            _eventRepository = eventRepository;
            _WorkflowService = WorkflowService;
            _CertificateRepository = CertificateRepository;
            _VerficationRequestRepository = VerficationRequestRepository;

        }
        public async Task<BaseResponse> Handle(VerifyCertificateCommand request, CancellationToken cancellationToken)
        {
            var eventObj = await _eventRepository.GetByIdAsync(request.EventId);

            if (eventObj == null)
            {
                throw new NotFoundException($"event with id = {request.EventId} is not found");
            }
            var verficationRequest = _VerficationRequestRepository.GetAll().Where(x => x.EventId == request.EventId)
            .OrderByDescending(x=>x.CreatedAt).FirstOrDefault();

            if (verficationRequest == null)
            {
                throw new NotFoundException($"verification request for  event with id = {request.EventId} is not found");
            }
            var response = await _WorkflowService.ApproveService(verficationRequest.RequestId, "verification", request.IsApprove, request.Comment, request.RejectionReasons, request.ReasonLookupId, false, cancellationToken);
            if (response.Item1)
            {
                try
                {
                    eventObj.IsVerified = true;
                    await _eventRepository.UpdateAsync(eventObj, e => e.Id);
                    await _eventRepository.SaveChangesAsync(cancellationToken);
                }
                catch (Exception exp)
                {
                    throw new NotFoundException(exp.Message);
                }
            }
            return new BaseResponse
            {
                Message = request.IsApprove ? "verified Successfully" : "Rejected Successfully"
            };
        }
    }
}