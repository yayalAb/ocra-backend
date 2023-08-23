using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AppDiv.CRVS.Application.Contracts.Request.AdoptionPersonalINformationRequest;

namespace AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Update;
// Customer create command with CustomerResponse
public class UpdateAdoptionCommand : IRequest<UpdateAdoptionCommandResponse>
{
    public Guid? Id { get; set; }
    public Guid? BeforeAdoptionAddressId { get; set; }
    public string? AdoptionCertificateId { get; set; }
    public LanguageModel ApprovedName { get; set; }
    public LanguageModel? Reason { get; set; }
    public virtual AddAdoptionPersonalInfoRequest AdoptiveMother { get; set; }
    public AddAdoptionPersonalInfoRequest AdoptiveFather { get; set; }
    public virtual AddCourtCaseRequest CourtCase { get; set; }
    public virtual AddAdoptionEventRequest Event { get; set; }
    public bool IsFromCommand { get; set; } = false;
    public bool ValidateFirst { get; set; } = false;

}


public class UpdateAdoptionCommandHandler : IRequestHandler<UpdateAdoptionCommand, UpdateAdoptionCommandResponse>
{
    private readonly IAdoptionEventRepository _adoptionEventRepository;
    private readonly IEventPaymentRequestService _paymentRequestService;
    private readonly IPersonalInfoRepository _personalInfoRepository;
    private readonly IEventDocumentService _eventDocumentService;
    private readonly IFileService _fileService;
    private readonly IEventRepository _eventRepository;
    private readonly ILookupRepository _LookupsRepo;


    public UpdateAdoptionCommandHandler(ILookupRepository LookupsRepo, IEventDocumentService eventDocumentService, IAdoptionEventRepository adoptionEventRepository, IEventPaymentRequestService paymentRequestService, IPersonalInfoRepository personalInfoRepository, IFileService fileService, IEventRepository eventRepository)
    {
        _adoptionEventRepository = adoptionEventRepository;
        _paymentRequestService = paymentRequestService;
        _personalInfoRepository = personalInfoRepository;
        _fileService = fileService;
        _eventRepository = eventRepository;
        _LookupsRepo = LookupsRepo;
        _eventDocumentService = eventDocumentService;
    }
    public async Task<UpdateAdoptionCommandResponse> Handle(UpdateAdoptionCommand request, CancellationToken cancellationToken)
    {
        var UpdateAdoptionCommandResponse = new UpdateAdoptionCommandResponse();
        request.Event.EventOwener.MiddleName = request?.AdoptiveFather?.FirstName;
        request.Event.EventOwener.LastName = request?.AdoptiveFather?.MiddleName;
        var validator = new CreateAdoptionCommandValidetor(_adoptionEventRepository, _eventRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (validationResult.Errors.Count > 0)
        {
            UpdateAdoptionCommandResponse.Success = false;
            UpdateAdoptionCommandResponse.ValidationErrors = new List<string>();
            foreach (var error in validationResult.Errors)
                UpdateAdoptionCommandResponse.ValidationErrors.Add(error.ErrorMessage);
            UpdateAdoptionCommandResponse.Message = UpdateAdoptionCommandResponse.ValidationErrors[0];
        }
        else if (UpdateAdoptionCommandResponse.Success)
        {
            var SelectedEvent= _eventRepository.GetAll()
                    .AsNoTracking()
                   .Where(x=>x.Id==request.Event.Id).FirstOrDefault();
            if (request.ValidateFirst == true)
            {
                UpdateAdoptionCommandResponse.Created(entity: "Adoption", message: "Valid Input.");
                return UpdateAdoptionCommandResponse;
            }
            try
            {
                request.Event.EventType = "Adoption";
                request.AdoptiveFather.SexLookupId = _LookupsRepo.GetAll().Where(l => l.Key == "sex")
                                                    .Where(l => EF.Functions.Like(l.ValueStr, "%ወንድ%")
                                                        || EF.Functions.Like(l.ValueStr, "%Dhiira%")
                                                        || EF.Functions.Like(l.ValueStr, "%Male%"))
                                                    .Select(l => l.Id).FirstOrDefault();

                request.AdoptiveMother.SexLookupId = _LookupsRepo.GetAll().Where(l => l.Key == "sex")
                                                        .Where(l => EF.Functions.Like(l.ValueStr, "%ሴት%")
                                                            || EF.Functions.Like(l.ValueStr, "%Dubara%")
                                                            || EF.Functions.Like(l.ValueStr, "%Female%"))
                                                        .Select(l => l.Id).FirstOrDefault();
                request.Event.EventDateEt = request.CourtCase.ConfirmedDateEt;
                var supportingDocs = request.Event.EventSupportingDocuments?.Where(doc => doc.Id == null).ToList();
                var examptionsupportingDocs = request.Event.PaymentExamption?.SupportingDocuments?.Where(doc => doc.Id == null).ToList();
                var adoptionEvent = CustomMapper.Mapper.Map<AdoptionEvent>(request);
                adoptionEvent.Event.EventAddressId = adoptionEvent.CourtCase.Court.AddressId;
                adoptionEvent.Event.IsPaid=SelectedEvent.IsPaid;
                adoptionEvent.Event.IsVerified=SelectedEvent.IsVerified;
                adoptionEvent.Event.IsCertified=false;
                adoptionEvent.Event.EventRegisteredAddressId=SelectedEvent.EventRegisteredAddressId;
                adoptionEvent.Event.HasPendingDocumentApproval=SelectedEvent.HasPendingDocumentApproval;
                adoptionEvent.Event.IsOfflineReg=SelectedEvent.IsOfflineReg;
                var personIds = new PersonIdObj();
                await _adoptionEventRepository.EFUpdate(adoptionEvent, _paymentRequestService, cancellationToken);

                if (!request.IsFromCommand)
                {
                    adoptionEvent.Event.EventSupportingDocuments = null;
                    if (adoptionEvent.Event.PaymentExamption != null)
                    {
                        adoptionEvent.Event.PaymentExamption.SupportingDocuments = null;
                    }
                    
                    personIds = new PersonIdObj
                    {
                        MotherId = adoptionEvent.AdoptiveMother != null ? adoptionEvent.AdoptiveMother.Id : adoptionEvent.AdoptiveMotherId,
                        FatherId = adoptionEvent.AdoptiveFather != null ? adoptionEvent.AdoptiveFather.Id : adoptionEvent.AdoptiveFatherId,
                        ChildId = adoptionEvent.Event.EventOwener != null ? adoptionEvent.Event.EventOwener.Id : adoptionEvent.Event.EventOwenerId
                    };
                    var docs = await _eventDocumentService.createSupportingDocumentsAsync(supportingDocs, examptionsupportingDocs, adoptionEvent.EventId, adoptionEvent.Event.PaymentExamption?.Id, cancellationToken);
                    var separatedDocs = _eventDocumentService.extractSupportingDocs(personIds, docs.supportingDocs);
                    _eventDocumentService.savePhotos(separatedDocs.userPhotos);
                    _eventDocumentService.saveSupportingDocuments((ICollection<SupportingDocument>)separatedDocs.otherDocs, (ICollection<SupportingDocument>)docs.examptionDocs, "Adoption");

                }
                else
                {
                    personIds = new PersonIdObj
                    {
                        MotherId = adoptionEvent.AdoptiveMother != null ? adoptionEvent.AdoptiveMother.Id : adoptionEvent.AdoptiveMotherId,
                        FatherId = adoptionEvent.AdoptiveFather != null ? adoptionEvent.AdoptiveFather.Id : adoptionEvent.AdoptiveFatherId,
                        ChildId = adoptionEvent.Event.EventOwener != null ? adoptionEvent.Event.EventOwener.Id : adoptionEvent.Event.EventOwenerId
                    };
                    var separatedDocs = _eventDocumentService.ExtractOldSupportingDocs(personIds, adoptionEvent.Event.EventSupportingDocuments);
                    if (separatedDocs.userPhotos != null && (separatedDocs.userPhotos.Count != 0))
                    {
                        _eventDocumentService.MovePhotos(separatedDocs.userPhotos, "Adoption");
                    }
                    if (!adoptionEvent.Event.IsExampted)
                    {
                        _eventDocumentService.MoveSupportingDocuments((ICollection<SupportingDocument>)separatedDocs.otherDocs, adoptionEvent?.Event?.PaymentExamption?.SupportingDocuments, "Adoption");
                    }
                }
               await _adoptionEventRepository.SaveChangesAsync(cancellationToken);
                // _eventDocumentService.saveSupportingDocuments(adoptionEvent.Event.EventSupportingDocuments, adoptionEvent.Event.PaymentExamption.SupportingDocuments, "Adoption");
                UpdateAdoptionCommandResponse = new UpdateAdoptionCommandResponse { Message = "Adoption Event Updated Successfully" };
            }
            catch (Exception ex)
            {
                UpdateAdoptionCommandResponse = new UpdateAdoptionCommandResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        // }
        return UpdateAdoptionCommandResponse;
    }
}
