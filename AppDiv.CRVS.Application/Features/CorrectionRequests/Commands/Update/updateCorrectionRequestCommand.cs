using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Exceptions;

namespace AppDiv.CRVS.Application.Features.CorrectionRequests.Commands.Update
{
    // Customer create command with CustomerResponse
    public class updateCorrectionRequestCommand : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }
        public JArray? Description { get; set; }
        public JObject Content { get; set; }
        public bool HasPayment { get; set; }=false;
    }
    public class updateCorrectionRequestCommandHandler : IRequestHandler<updateCorrectionRequestCommand, BaseResponse>
    {
        private readonly ICorrectionRequestRepostory _CorrectionRequestRepostory;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IEventRepository _eventRepository;
        private readonly IContentValidator _contentValidator;
        private readonly IWorkflowService _WorkflowService;


        public updateCorrectionRequestCommandHandler(ICorrectionRequestRepostory CorrectionRequestRepostory,
                                                    IEventDocumentService eventDocumentService,
                                                    IEventRepository eventRepository,
                                                    IContentValidator contentValidator,
                                                    IWorkflowService WorkflowService)
        {
            _CorrectionRequestRepostory = CorrectionRequestRepostory;
            _eventDocumentService = eventDocumentService;
            _eventRepository = eventRepository;
            this._contentValidator = contentValidator;
            _WorkflowService=WorkflowService;
        }
        public async Task<BaseResponse> Handle(updateCorrectionRequestCommand request, CancellationToken cancellationToken)
        {
             var response = new BaseResponse();
            var correctionRequestData = _CorrectionRequestRepostory.GetAll()
            .Include(x => x.Request)
            .Where(x => x.Id == request.Id).FirstOrDefault();
            if(correctionRequestData.Request==null||(correctionRequestData.Request.currentStep==correctionRequestData.Request.NextStep)){
               throw new NotFoundException("you can not edit this request b/c it is approved"); 
            }
            correctionRequestData.Description = request.Description;
            correctionRequestData.Content = request.Content;
            correctionRequestData.Request.IsRejected = false;
            correctionRequestData.HasPayment=request.HasPayment;
            correctionRequestData.Request.currentStep=0;
            correctionRequestData.Request.NextStep=_WorkflowService.GetNextStep("change", 0, true);
            try
            {
                var events = await _eventRepository.GetAsync(correctionRequestData.EventId);
                var validationResponse = await _contentValidator.ValidateAsync(events.EventType, correctionRequestData.Content);
                if (validationResponse.Status != 200)
                {
                    return validationResponse;
                }
                var supportingDocuments = GetSupportingDocumentsMe(correctionRequestData.Content, "eventSupportingDocuments");
                var examptionDocuments = GetSupportingDocumentsMe(supportingDocuments.Item1, "paymentExamption");
                _eventDocumentService.SaveCorrectionRequestSupportingDocuments(supportingDocuments.Item2, examptionDocuments.Item2, events?.EventType);
                correctionRequestData.Content = examptionDocuments.Item1;
                await _CorrectionRequestRepostory.UpdateAsync(correctionRequestData, x => x.Id);
                await _CorrectionRequestRepostory.SaveChangesAsync(cancellationToken);
                // var events = await _eventRepository.GetAsync(correctionRequestData.EventId);

            }
            catch (Exception exp)
            {
                response.BadRequest(exp.Message);
            }
            return response;
        }
                private (JObject, List<AddSupportingDocumentRequest>) GetSupportingDocumentsMe(JObject content, string type)
        {

            var supportingDocuments = new List<AddSupportingDocumentRequest>();
            try
            {
                var contentList = type switch
                {
                    "eventSupportingDocuments" => content?.Value<JObject>("event")?.Value<JArray>("eventSupportingDocuments"),
                    "paymentExamption" => content?.Value<JObject>("event")?.Value<JObject>("paymentExamption")?.Value<JArray>("supportingDocuments")
                };
                if (contentList != null)
                {
                    foreach (JObject item in contentList.ToList())
                    {
                        if
                        (item["id"] == null)
                        {
                            item["id"] = Guid.NewGuid().ToString();
                            supportingDocuments.Add(item.ToObject<AddSupportingDocumentRequest>());
                            item["base64String"] = "";
                        }
                        else
                        {
                            contentList.Remove(item);
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            return (content, supportingDocuments);
        }
    }
}
