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
        private readonly ILogger<CreateAdoptionCommandHandler> _logger;



        public CreateAdoptionCommandHandler(ILogger<CreateAdoptionCommandHandler> logger, IAdoptionEventRepository AdoptionEventRepository, IPersonalInfoRepository personalInfoRepository, IFileService fileService)
        {
            _AdoptionEventRepository = AdoptionEventRepository;
            _personalInfoRepository = personalInfoRepository;
            _fileService = fileService;
            _logger = logger;

        }
        public async Task<CreateAdoptionCommandResponse> Handle(CreateAdoptionCommand request, CancellationToken cancellationToken)
        {
            var adoptionEvent = CustomMapper.Mapper.Map<AdoptionEvent>(request.Adoption);


            // if (adoptionEvent.AdoptiveFather.Id != null && adoptionEvent.AdoptiveFather.Id != Guid.Empty)
            // {
            //     _personalInfoRepository.EFUpdate(CustomMapper.Mapper.Map<PersonalInfo>(adoptionEvent.AdoptiveFather));
            //     adoptionEvent.AdoptiveFatherId = adoptionEvent.AdoptiveFather.Id;
            //     adoptionEvent.AdoptiveFather = null;
            // }
            // if (adoptionEvent.Event.EventOwener.Id != null && adoptionEvent.Event.EventOwener.Id != Guid.Empty)
            // {
            //     _personalInfoRepository.EFUpdate(CustomMapper.Mapper.Map<PersonalInfo>(adoptionEvent.Event.EventOwener));
            //     adoptionEvent.Event.EventOwenerId = adoptionEvent.Event.EventOwener.Id;
            //     adoptionEvent.Event.EventOwener = null;
            // }
            // if (adoptionEvent.Event.EventRegistrar.RegistrarInfo.Id != null && adoptionEvent.Event.EventRegistrar.RegistrarInfo.Id != Guid.Empty)
            // {
            //     _personalInfoRepository.EFUpdate(CustomMapper.Mapper.Map<PersonalInfo>(adoptionEvent.Event.EventRegistrar.RegistrarInfo));
            //     adoptionEvent.Event.EventRegistrar.RegistrarInfoId = adoptionEvent.Event.EventRegistrar.RegistrarInfo.Id;
            //     adoptionEvent.Event.EventRegistrar.RegistrarInfo = null;
            // }
            _logger.LogCritical(request.Adoption.BeforeAdoptionAddressId.ToString());
            _logger.LogCritical(adoptionEvent.BeforeAdoptionAddressId.ToString());

            await _AdoptionEventRepository.InsertAsync(adoptionEvent, cancellationToken);
            await _AdoptionEventRepository.SaveChangesAsync(cancellationToken);

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

            return new CreateAdoptionCommandResponse { Message = "Adoption Event created Successfully" };

        }
    }
}


