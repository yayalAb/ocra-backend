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
        public CreateCorrectionRequestHandler(ICorrectionRequestRepostory CorrectionRepository,
                                                IWorkflowService WorkflowService,
                                                IEventDocumentService eventDocumentService,
                                                IEventRepository eventRepository,
                                                IWorkflowRepository WorkflowRepository)
        {
            _eventDocumentService = eventDocumentService;
            _eventRepository = eventRepository;
            _CorrectionRepository = CorrectionRepository;
            _WorkflowService = WorkflowService;
            _WorkflowRepository = WorkflowRepository;
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
                        var events = await _eventRepository.GetAsync(request.CorrectionRequest.EventId);
                        var supportingDocuments = GetSupportingDocuments(CorrectionRequest.Content, "eventSupportingDocuments", out JObject newContent);
                        var examptionDocuments = GetSupportingDocuments(newContent, "paymentExamption", out JObject finalContent);
                        _eventDocumentService.SaveCorrectionRequestSupportingDocuments(supportingDocuments, examptionDocuments, events.EventType);
                        CorrectionRequest.Content = finalContent;

                        await _CorrectionRepository.InsertAsync(CorrectionRequest, cancellationToken);
                        var result = await _CorrectionRepository.SaveChangesAsync(cancellationToken);
                        await transaction.CommitAsync();
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
                            base64String = file.base64String
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
