using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Contracts.DTOs;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Domain.Enums;

namespace AppDiv.CRVS.Application.Features.CorrectionRequests.Commands
{
    public class CreateCorrectionRequestHandler : IRequestHandler<CreateCorrectionRequest, CreateCorrectionRequestResponse>
    {
        private readonly ICorrectionRequestRepostory _CorrectionRepository;
        private readonly IWorkflowService _WorkflowService;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IEventRepository _eventRepository;
        private readonly IWorkflowRepository _WorkflowRepository;
        private readonly ITransactionService _transactionService;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;

        public CreateCorrectionRequestHandler(ICorrectionRequestRepostory CorrectionRepository,
                                              IWorkflowService WorkflowService,
                                              IEventDocumentService eventDocumentService,
                                              IEventRepository eventRepository,
                                              IWorkflowRepository WorkflowRepository,
                                              ITransactionService transactionService,
                                              IUserRepository userRepository,
                                              INotificationService notificationService)
        {
            _eventDocumentService = eventDocumentService;
            _eventRepository = eventRepository;
            _CorrectionRepository = CorrectionRepository;
            _WorkflowService = WorkflowService;
            _WorkflowRepository = WorkflowRepository;
            _transactionService = transactionService;
            _userRepository = userRepository;
            _notificationService = notificationService;
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
            Guid WorkflowId = _WorkflowRepository.GetAll()
                      .Where(wf => wf.workflowName == "change").Select(x => x.Id).FirstOrDefault();
            if (WorkflowId == null || WorkflowId == Guid.Empty)
            {
                throw new Exception("change Work Flow Does not exist Pleace Create Workflow First");
            }
            var CreateAddressCommadResponse = new CreateCorrectionRequestResponse();
            request.CorrectionRequest.Request.RequestType = "change";
            request.CorrectionRequest.Request.currentStep = 0;
            var CorrectionRequest = CustomMapper.Mapper.Map<CorrectionRequest>(request.CorrectionRequest);
            CorrectionRequest.Request.WorkflowId = WorkflowId;
            CorrectionRequest.Request.NextStep = _WorkflowService.GetNextStep("change", 0, true);
            await _CorrectionRepository.InsertAsync(CorrectionRequest, cancellationToken);
            var result = await _CorrectionRepository.SaveChangesAsync(cancellationToken);
            var events = await _eventRepository.GetAsync(request.CorrectionRequest.EventId);
            var supportingDocuments = GetSupportingDocuments(request.CorrectionRequest.Content, "eventSupportingDocuments");
            var examptionDocuments = GetSupportingDocuments(request.CorrectionRequest.Content, "paymentExamption");
            _eventDocumentService.SaveCorrectionRequestSupportingDocuments(supportingDocuments, examptionDocuments, events.EventType);
            // request.CorrectionRequest.Content?.Value<JObject>("event")?.Value<JArray>("eventSupportingDocuments")
            string? userId = _userRepository.GetAll()
                                .Where(u => u.PersonalInfoId == CorrectionRequest.Request.CivilRegOfficerId)
                                .Select(u => u.Id).FirstOrDefault();
            if (userId == null)
            {
                throw new NotFoundException("user not found");
            }
            var NewTranscation = new TransactionRequestDTO
            {
                CurrentStep = 0,
                ApprovalStatus = true,
                WorkflowId = WorkflowId,
                RequestId = CorrectionRequest.RequestId,
                CivilRegOfficerId = userId,//_UserResolverService.GetUserId().ToString(),
                Remark = "Correction Request"
            };

            await _transactionService.CreateTransaction(NewTranscation);
            await _notificationService.CreateNotification(request.CorrectionRequest.EventId, Enum.GetName<NotificationType>(NotificationType.change)!, "Correction Request",
                               _WorkflowService.GetReceiverGroupId(Enum.GetName<NotificationType>(NotificationType.change)!, (int)CorrectionRequest.Request.NextStep), CorrectionRequest.RequestId,
                             userId);
            return CreateAddressCommadResponse;
        }
        private ICollection<SupportingDocument> GetSupportingDocuments(JObject content, string type)
        {
            // content = content?.Value<JObject>("event");
            var contentList = type switch
            {
                "eventSupportingDocuments" => content?.Value<JObject>("event")?.Value<JArray>("eventSupportingDocuments"),
                "paymentExamption" => content?.Value<JObject>("event")?.Value<JArray>("paymentExamption")?.Value<JArray>("supportingDocuments")
            };
            var supportingDocuments = new List<SupportingDocument>();
            if (contentList != null)
            {
                var baseList = JsonConvert.DeserializeObject<List<SupportingDocument>>(contentList.ToString());
                foreach (JToken sup in contentList)
                {
                    SupportingDocument file = sup?.ToObject<SupportingDocument>();
                    supportingDocuments.Add(file);
                    // sup?.Value<string>("base64String").Remove();
                    // contentList.Value<string>("base64String")(contentList.IndexOf(sup)).base64String = null;

                }
                // if (type == "eventSupportingDocuments")
                // {
                //     content?.Value<JObject>("event")?.Value<JArray>("eventSupportingDocuments") =
                // }

            }
            return supportingDocuments;
        }
    }
}
