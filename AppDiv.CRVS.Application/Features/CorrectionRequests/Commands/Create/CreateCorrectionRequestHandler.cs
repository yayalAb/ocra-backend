using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Application.Contracts.DTOs;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Domain.Enums;
using AppDiv.CRVS.Application.Service.ArchiveService;
using AppDiv.CRVS.Application.Common;

namespace AppDiv.CRVS.Application.Features.CorrectionRequests.Commands
{
    public class CreateCorrectionRequestHandler : IRequestHandler<CreateCorrectionRequest, BaseResponse>
    {
        private readonly ICorrectionRequestRepostory _CorrectionRepository;
        private readonly IWorkflowService _WorkflowService;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IEventRepository _eventRepository;
        private readonly IWorkflowRepository _WorkflowRepository;
        private readonly ITransactionService _transactionService;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IContentValidator _contentValidator;
        private readonly IEventPaymentRequestService _paymentRequestService;


        public CreateCorrectionRequestHandler(ICorrectionRequestRepostory CorrectionRepository,
                                              IWorkflowService WorkflowService,
                                              IEventDocumentService eventDocumentService,
                                              IEventRepository eventRepository,
                                              IWorkflowRepository WorkflowRepository,
                                              ITransactionService transactionService,
                                              IUserRepository userRepository,
                                              INotificationService notificationService,
                                              IContentValidator contentValidator,
                                              IEventPaymentRequestService paymentRequestService
                                              )
        {
            if (contentValidator is null)
            {
                throw new ArgumentNullException(nameof(contentValidator));
            }

            _eventDocumentService = eventDocumentService;
            _eventRepository = eventRepository;
            _CorrectionRepository = CorrectionRepository;
            _WorkflowService = WorkflowService;
            _WorkflowRepository = WorkflowRepository;
            _transactionService = transactionService;
            _userRepository = userRepository;
            _notificationService = notificationService;
            this._contentValidator = contentValidator;
            _paymentRequestService = paymentRequestService;
        }

        public async Task<BaseResponse> Handle(CreateCorrectionRequest request, CancellationToken cancellationToken)
        {
            bool hasWorkflow = false;
            var executionStrategy = _CorrectionRepository.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = _CorrectionRepository.Database.BeginTransaction())
                {
                    try

                    {
                        var response = new BaseResponse();
                        request.CorrectionRequest.Request.RequestType = "change";
                        request.CorrectionRequest.Request.currentStep = 0;
                        var CorrectionRequest = CustomMapper.Mapper.Map<CorrectionRequest>(request.CorrectionRequest);
                        var events = await _eventRepository.GetAsync(request.CorrectionRequest.EventId);
                        var Workflow = _WorkflowRepository.GetAll()
                        .Include(x => x.Steps)
                           .Where(wf => wf.workflowName == "change").FirstOrDefault();
                        if ((Workflow?.Id != null && Workflow?.Id != Guid.Empty) && (Workflow?.Steps?.FirstOrDefault() != null))
                        {
                            hasWorkflow = true;
                            CorrectionRequest.Request.WorkflowId = Workflow.Id;
                            CorrectionRequest.Request.NextStep = _WorkflowService.GetNextStep("change", 0, true);
                        }
                        else
                        {
                            (float amount, string code) payment = await _paymentRequestService.CreatePaymentRequest(events.EventType, events, "change", null, false, false, cancellationToken);
                            if (payment.amount == 0 || payment.amount == 0.0)
                            {
                                hasWorkflow = false;
                            }
                        }
                        var validationResponse = await _contentValidator.ValidateAsync(events.EventType, CorrectionRequest.Content, hasWorkflow);
                        if (validationResponse.Status != 200)
                        {
                            return validationResponse;
                        }
                        if (!hasWorkflow)
                        {
                            await transaction.CommitAsync();

                            return response;
                        }
                        var supportingDocuments = GetSupportingDocumentsMe(CorrectionRequest.Content, "eventSupportingDocuments");
                        var examptionDocuments = GetSupportingDocumentsMe(supportingDocuments, "paymentExamption");
                        // _eventDocumentService.SaveCorrectionRequestSupportingDocuments(supportingDocuments, examptionDocuments, events?.EventType);
                        CorrectionRequest.Content = examptionDocuments;

                        await _CorrectionRepository.InsertAsync(CorrectionRequest, cancellationToken);
                        var result = await _CorrectionRepository.SaveChangesAsync(cancellationToken);
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
                            WorkflowId = Workflow.Id,
                            RequestId = CorrectionRequest.RequestId,
                            CivilRegOfficerId = userId,//_UserResolverService.GetUserId().ToString(),
                            Remark = "Correction Request"
                        };

                        await _transactionService.CreateTransaction(NewTranscation);
                        await _notificationService.CreateNotification(CorrectionRequest.Id, Enum.GetName<NotificationType>(NotificationType.change)!, "Correction Request",
                                           _WorkflowService.GetReceiverGroupId(Enum.GetName<NotificationType>(NotificationType.change)!, (int)CorrectionRequest.Request.NextStep), CorrectionRequest.RequestId,
                                         userId);
                        await transaction.CommitAsync();

                        return response;
                    }
                    catch (Exception)
                    {

                        await transaction.RollbackAsync();
                        throw;
                    }
                }

            });
        }
       

        private JObject GetSupportingDocumentsMe(JObject content, string type)
        {

            var supportingDocuments = new List<AddSupportingDocumentRequest>();
            try
            {
                var contentList = type switch 
                {
                    "eventSupportingDocuments" => content?.Value<JObject>("event")?.Value<JArray>("eventSupportingDocuments"),
                    "paymentExamption" => content?.Value<JObject>("event")?.Value<JObject>("paymentExamption")?.Value<JArray>("supportingDocuments")
                };
                if(contentList!=null){
                foreach (JObject item in contentList.ToList())
                {
                    if (item["id"] == null)
                    {
                        item["id"] = Guid.NewGuid().ToString();
                        item["base64String"]="";
                    }else{
                      contentList.Remove(item);  
                    }
                }
                }
            } 
            catch (System.Exception)
            {
                throw;
            }
            return content;
        }




    }
}
