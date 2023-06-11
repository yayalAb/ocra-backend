using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Contracts.DTOs;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Features.CorrectionRequests.Commands
{
    public class CreateCorrectionRequestHandler : IRequestHandler<CreateCorrectionRequest, CreateCorrectionRequestResponse>
    {
        private readonly ICorrectionRequestRepostory _CorrectionRepository;
        private readonly IWorkflowService _WorkflowService;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IEventRepository _eventRepository;
        public CreateCorrectionRequestHandler(ICorrectionRequestRepostory CorrectionRepository,
                                                IWorkflowService WorkflowService,
                                                IEventDocumentService eventDocumentService,
                                                IEventRepository eventRepository)
        {
            _eventDocumentService = eventDocumentService;
            _eventRepository = eventRepository;
            _CorrectionRepository = CorrectionRepository;
            _WorkflowService = WorkflowService;
        }

        public async Task<CreateCorrectionRequestResponse> Handle(CreateCorrectionRequest request, CancellationToken cancellationToken)
        {
            // var NewTranscation = new TransactionRequestDTO
            // {
            //     CurrentStep = request.currentStep,
            //     ApprovalStatus = IsApprove,
            //     WorkflowId = RequestId,
            //     RequestId = RequestId,
            //     CivilRegOfficerId = _UserResolverService.GetUserId().ToString(),
            //     Remark = Remark
            // };
            // await _TransactionService.CreateTransaction(NewTranscation);
            // await _NotificationService.CreateNotification(ReturnId, workflowType, workflowType,
            //                    this.GetReceiverGroupId(workflowType, request.currentStep), RequestId,
            //                   _UserResolverService.GetUserId().ToString());

            var CreateAddressCommadResponse = new CreateCorrectionRequestResponse();
            request.CorrectionRequest.Request.RequestType = "change";
            request.CorrectionRequest.Request.currentStep = 0;
            var CorrectionRequest = CustomMapper.Mapper.Map<CorrectionRequest>(request.CorrectionRequest);
            await _CorrectionRepository.InsertAsync(CorrectionRequest, cancellationToken);
            var result = await _CorrectionRepository.SaveChangesAsync(cancellationToken);
            var events = await _eventRepository.GetAsync(request.CorrectionRequest.EventId);
            var supportingDocuments = GetSupportingDocuments(request.CorrectionRequest.Content, "eventSupportingDocuments");
            var examptionDocuments = GetSupportingDocuments(request.CorrectionRequest.Content, "paymentExamption");
            _eventDocumentService.SaveCorrectionRequestSupportingDocuments(supportingDocuments, examptionDocuments, "Birth");
            return CreateAddressCommadResponse;
        }
        private ICollection<SupportingDocument> GetSupportingDocuments(JObject content, string type)
        {
            content = content?.Value<JObject>("event");
            var contentList = type switch
            {
                "eventSupportingDocuments" => content?.Value<JArray>("eventSupportingDocuments"),
                "paymentExamption" => content?.Value<JArray>("paymentExamption")?.Value<JArray>("supportingDocuments")
            };
            var supportingDocuments = new List<SupportingDocument>();
            foreach (JToken sup in contentList)
            {
                SupportingDocument file = sup.ToObject<SupportingDocument>();
                supportingDocuments.Add(file);
            }
            return supportingDocuments;
        }
    }
}
