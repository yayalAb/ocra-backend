using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
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
    public AddAdoptionRequest Adoption { get; set; }
}


public class UpdateAdoptionCommandHandler : IRequestHandler<UpdateAdoptionCommand, UpdateAdoptionCommandResponse>
{
    private readonly IAdoptionEventRepository _adoptionEventRepository;
    private readonly IPersonalInfoRepository _personalInfoRepository;
    private readonly IEventDocumentService _eventDocumentService;
    private readonly IFileService _fileService;


    public UpdateAdoptionCommandHandler(IEventDocumentService eventDocumentService, IAdoptionEventRepository adoptionEventRepository, IPersonalInfoRepository personalInfoRepository, IFileService fileService)
    {
        _adoptionEventRepository = adoptionEventRepository;
        _personalInfoRepository = personalInfoRepository;
        _fileService = fileService;

        _eventDocumentService = eventDocumentService;
    }
    public async Task<UpdateAdoptionCommandResponse> Handle(UpdateAdoptionCommand request, CancellationToken cancellationToken)
    {
        var UpdateAdoptionCommandResponse = new UpdateAdoptionCommandResponse();

        var validator = new CreateAdoptionCommandValidetor(_adoptionEventRepository);
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
            try
            {
                request.Adoption.Event.EventType = "Adoption";
                var adoptionEvent = CustomMapper.Mapper.Map<AdoptionEvent>(request.Adoption);
                _adoptionEventRepository.EFUpdate(adoptionEvent);
                await _adoptionEventRepository.SaveChangesAsync(cancellationToken);
                _eventDocumentService.saveSupportingDocuments(adoptionEvent.Event.EventSupportingDocuments, adoptionEvent.Event.PaymentExamption.SupportingDocuments, "Adoption");
                UpdateAdoptionCommandResponse = new UpdateAdoptionCommandResponse { Message = "Adoption Event Updated Successfully" };
            }
            catch (Exception ex)
            {
                UpdateAdoptionCommandResponse = new UpdateAdoptionCommandResponse { Message = ex.Message };
            }
        }
        return UpdateAdoptionCommandResponse;
    }
}
