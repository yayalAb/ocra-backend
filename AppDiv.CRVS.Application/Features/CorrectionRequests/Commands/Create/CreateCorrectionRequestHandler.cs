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
            var executionStrategy = _CorrectionRepository.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = _CorrectionRepository.Database.BeginTransaction())
                {
                    try

                    {
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
                        var events = await _eventRepository.GetAsync(request.CorrectionRequest.EventId);
                        var supportingDocuments = GetSupportingDocuments(CorrectionRequest.Content, "eventSupportingDocuments", out JObject newContent);
                        var examptionDocuments = GetSupportingDocuments(newContent, "paymentExamption", out JObject finalContent);
                        _eventDocumentService.SaveCorrectionRequestSupportingDocuments(supportingDocuments, examptionDocuments, events?.EventType);
                        CorrectionRequest.Content = finalContent;

                        await _CorrectionRepository.InsertAsync(CorrectionRequest, cancellationToken);
                        var result = await _CorrectionRepository.SaveChangesAsync(cancellationToken);
                        await transaction.CommitAsync();
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
                    catch (Exception)
                    {

                        await transaction.RollbackAsync();
                        throw;
                    }
                }

            });
        }
        private ICollection<AddSupportingDocumentRequest> GetSupportingDocuments(JObject content, string type, out JObject modifiedContent)
        {
            // content = content?.Value<JObject>("event");
            // modifiedContent = content;
            try
            {
                var contentList = type switch
                {
                    "eventSupportingDocuments" => content?.Value<JObject>("event")?.Value<JArray>("eventSupportingDocuments"),
                    "paymentExamption" => content?.Value<JObject>("event")?.Value<JObject>("paymentExamption")?.Value<JArray>("supportingDocuments")
                };
                var supportingDocuments = new List<AddSupportingDocumentRequest>();
                if (contentList != null)
                {
                    for (int i = 0; i < contentList.Count; i++)
                    {
                        JToken sup = contentList[i];
                        AddSupportingDocumentRequest file = sup?.ToObject<AddSupportingDocumentRequest>();
                        file.Id = Guid.NewGuid();
                        var newFile = new AddSupportingDocumentRequest
                        {
                            Id = file.Id,
                            Description = file.Description,
                            Label = file.Label,
                            Type = file.Type,
                            base64String = file?.base64String
                        };
                        supportingDocuments.Add(file);
                        newFile.base64String = "null";
                        JToken supdoc = JToken.FromObject(newFile);
                        if (type == "eventSupportingDocuments")
                            content?.Value<JObject>("event")?.Value<JArray>("eventSupportingDocuments")?[i].Replace(supdoc);
                        else
                            content?.Value<JObject>("event")?.Value<JObject>("paymentExamption")?.Value<JArray>("supportingDocuments")?[i].Replace(supdoc);
                    }
                }
                modifiedContent = content;
                return supportingDocuments;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
