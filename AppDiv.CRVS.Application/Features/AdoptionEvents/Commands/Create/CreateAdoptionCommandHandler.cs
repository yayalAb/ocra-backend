using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Create
{
    public class CreateAdoptionCommandHandler : IRequestHandler<CreateAdoptionCommand, CreateAdoptionCommandResponse>
    {
        private readonly IAdoptionEventRepository _AdoptionEventRepository;
        private readonly IPersonalInfoRepository _personalInfoRepository;
        private readonly IFileService _fileService;
        private readonly IEventDocumentService _eventDocumentService;
        public CreateAdoptionCommandHandler(IEventDocumentService eventDocumentService, IAdoptionEventRepository AdoptionEventRepository, IPersonalInfoRepository personalInfoRepository, IFileService fileService)
        {
            _AdoptionEventRepository = AdoptionEventRepository;
            _personalInfoRepository = personalInfoRepository;
            _fileService = fileService;
            _eventDocumentService = eventDocumentService;
        }
        public async Task<CreateAdoptionCommandResponse> Handle(CreateAdoptionCommand request, CancellationToken cancellationToken)
        {
            var CreateAdoptionCommandResponse = new CreateAdoptionCommandResponse();

            var validator = new CreatAdoptionCommandValidator(_AdoptionEventRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (validationResult.Errors.Count > 0)
            {
                CreateAdoptionCommandResponse.Success = false;
                CreateAdoptionCommandResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    CreateAdoptionCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                CreateAdoptionCommandResponse.Message = CreateAdoptionCommandResponse.ValidationErrors[0];
            }
            else if (CreateAdoptionCommandResponse.Success)
            {
                try
                {
                    request.Adoption.Event.EventType = "Adoption";
                    var adoptionEvent = CustomMapper.Mapper.Map<AdoptionEvent>(request.Adoption);
                    if (adoptionEvent.AdoptiveFather.Id != null && adoptionEvent.AdoptiveFather.Id != Guid.Empty)
                    {
                        _personalInfoRepository.EFUpdate(CustomMapper.Mapper.Map<PersonalInfo>(adoptionEvent.AdoptiveFather));
                        adoptionEvent.AdoptiveFatherId = adoptionEvent.AdoptiveFather.Id;
                        adoptionEvent.AdoptiveFather = null;
                    }
                    if (adoptionEvent.AdoptiveMother.Id != null && adoptionEvent.AdoptiveMother.Id != Guid.Empty)
                    {
                        _personalInfoRepository.EFUpdate(CustomMapper.Mapper.Map<PersonalInfo>(adoptionEvent.AdoptiveMother));
                        adoptionEvent.AdoptiveMotherId = adoptionEvent.AdoptiveMother.Id;
                        adoptionEvent.AdoptiveMother = null;
                    }
                    if (adoptionEvent.Event.EventOwener.Id != null && adoptionEvent.Event.EventOwener.Id != Guid.Empty)
                    {
                        _personalInfoRepository.EFUpdate(CustomMapper.Mapper.Map<PersonalInfo>(adoptionEvent.Event.EventOwener));
                        adoptionEvent.Event.EventOwenerId = adoptionEvent.Event.EventOwener.Id;
                        adoptionEvent.Event.EventOwener = null;
                    }
                    await _AdoptionEventRepository.InsertAsync(adoptionEvent, cancellationToken);
                    await _AdoptionEventRepository.SaveChangesAsync(cancellationToken);
                    _eventDocumentService.saveSupportingDocuments(adoptionEvent.Event.EventSupportingDocuments, adoptionEvent.Event.PaymentExamption.SupportingDocuments, "Adoption");
                    CreateAdoptionCommandResponse = new CreateAdoptionCommandResponse { Message = "Adoption Event created Successfully" };
                }
                catch (Exception ex)
                {
                    CreateAdoptionCommandResponse = new CreateAdoptionCommandResponse { Message = ex.Message };
                }
            }
            return CreateAdoptionCommandResponse;

        }
    }
}




