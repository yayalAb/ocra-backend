using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Update;
// Customer create command with CustomerResponse
public class UpdateAdoptionCommand : IRequest<UpdateAdoptionCommandResponse>
{
    public Guid Id { get; set; }
    public Guid BeforeAdoptionAddressId { get; set; }
    public JObject ApprovedName { get; set; }
    public JObject Reason { get; set; }
    public Guid AdoptiveMotherId { get; set; }
    public Guid AdoptiveFatherId { get; set; }
    public Guid CourtCaseId { get; set; }
    public Guid EventId { get; set; }
    public virtual UpdatePersonalInfoRequest AdoptiveMother { get; set; }
    public UpdatePersonalInfoRequest AdoptiveFather { get; set; }
    public virtual AddCourtCaseRequest CourtCase { get; set; }
    public virtual AddEventRequest Event { get; set; }
}


public class UpdateAdoptionCommandHandler : IRequestHandler<UpdateAdoptionCommand, UpdateAdoptionCommandResponse>
{
    private readonly IAdoptionEventRepository _adoptionEventRepository;
    private readonly IPersonalInfoRepository _personalInfoRepository;
    private readonly IFileService _fileService;

    public UpdateAdoptionCommandHandler(IAdoptionEventRepository adoptionEventRepository, IPersonalInfoRepository personalInfoRepository, IFileService fileService)
    {
        _adoptionEventRepository = adoptionEventRepository;
        _personalInfoRepository = personalInfoRepository;
        _fileService = fileService;
    }
    public async Task<UpdateAdoptionCommandResponse> Handle(UpdateAdoptionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var adoptionEvent = CustomMapper.Mapper.Map<AdoptionEvent>(request);
            await _adoptionEventRepository.UpdateAsync(adoptionEvent, x => x.Id);
            await _adoptionEventRepository.SaveChangesAsync(cancellationToken);
            var eventSupportingDocuments = adoptionEvent.Event.EventSupportingDocuments;
            var examptionSupportingDocuments = adoptionEvent.Event.PaymentExamption.SupportingDocuments;
            var supportingDocFolder = Path.Combine("Resources", "SupportingDocuments", "Adoption");
            var examptiondocFolder = Path.Combine("Resources", "ExamptionDocuments", "Adoption");
            var fullPathSupporting = Path.Combine(Directory.GetCurrentDirectory(), supportingDocFolder);
            var fullPathExamption = Path.Combine(Directory.GetCurrentDirectory(), supportingDocFolder);
            eventSupportingDocuments.ToList().ForEach(doc =>
            {
                _fileService.UploadBase64FileAsync(doc.base64String, doc.Id.ToString(), fullPathSupporting, FileMode.Create);
            });
            examptionSupportingDocuments.ToList().ForEach(doc =>
            {
                _fileService.UploadBase64FileAsync(doc.base64String, doc.Id.ToString(), fullPathExamption, FileMode.Create);
            });
            return new UpdateAdoptionCommandResponse { Message = "Adoption Event Updated Successfully" };

        }
        catch (Exception ex)
        {
            return new UpdateAdoptionCommandResponse { Message = ex.Message };
        }
    }
}
