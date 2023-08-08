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
    }
    public class updateCorrectionRequestCommandHandler : IRequestHandler<updateCorrectionRequestCommand, BaseResponse>
    {
        private readonly ICorrectionRequestRepostory _CorrectionRequestRepostory;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IEventRepository _eventRepository;
        private readonly IContentValidator _contentValidator;

        public updateCorrectionRequestCommandHandler(ICorrectionRequestRepostory CorrectionRequestRepostory,
                                                    IEventDocumentService eventDocumentService,
                                                    IEventRepository eventRepository,
                                                    IContentValidator contentValidator)
        {
            _CorrectionRequestRepostory = CorrectionRequestRepostory;
            _eventDocumentService = eventDocumentService;
            _eventRepository = eventRepository;
            this._contentValidator = contentValidator;
        }
        public async Task<BaseResponse> Handle(updateCorrectionRequestCommand request, CancellationToken cancellationToken)
        {
            var correctionRequestData = _CorrectionRequestRepostory.GetAll()
            .Include(x => x.Request).Where(x => x.Id == request.Id).FirstOrDefault();
            if (correctionRequestData.Request.currentStep != 0)
            {
                throw new NotFoundException("you can not edit this request it is Approved");
            }
            correctionRequestData.Description = request.Description;
            correctionRequestData.Content = request.Content;
            var response = new BaseResponse();
            try
            {
                var events = await _eventRepository.GetAsync(correctionRequestData.EventId);
                var validationResponse = await _contentValidator.ValidateAsync(events.EventType, correctionRequestData.Content);
                if (validationResponse.Status != 200)
                {
                    return validationResponse;
                }
                var supportingDocuments = GetSupportingDocuments(request.Content, "eventSupportingDocuments", out JObject newContent);
                var examptionDocuments = GetSupportingDocuments(newContent, "paymentExamption", out JObject finalContent);
                _eventDocumentService.SaveCorrectionRequestSupportingDocuments(supportingDocuments, examptionDocuments, events.EventType);
                correctionRequestData.Content = finalContent;

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
        private ICollection<AddSupportingDocumentRequest> GetSupportingDocuments(JObject content, string type, out JObject modifiedContent)
        {
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
                        if (file.Id != null)
                        {
                            if (type == "eventSupportingDocuments")
                                content?.Value<JObject>("event")?.Value<JArray>("eventSupportingDocuments")?[i].Remove();
                            else
                                content?.Value<JObject>("event")?.Value<JObject>("paymentExamption")?.Value<JArray>("supportingDocuments")?[i].Remove();
                            continue;
                        }
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
