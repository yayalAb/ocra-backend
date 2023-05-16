using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Update
{
    public class UpdateBirthEventCommandHandler : IRequestHandler<UpdateBirthEventCommand, BirthEventDTO>
    {
        private readonly IBirthEventRepository _birthEventRepository;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IEventRepository _eventRepository;

        public UpdateBirthEventCommandHandler(IBirthEventRepository birthEventRepository,
                                              IEventDocumentService eventDocumentService,
                                              IEventRepository eventRepository)
        {
            this._eventDocumentService = eventDocumentService;
            this._eventRepository = eventRepository;
            _birthEventRepository = birthEventRepository;
        }
        public async Task<BirthEventDTO> Handle(UpdateBirthEventCommand request, CancellationToken cancellationToken)
        {
            var updateBirthEventCommandResponse = new UpdateBirthEventCommandResponse();

            var validator = new UpdateBirthEventCommandValidator(_eventRepository, request);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                updateBirthEventCommandResponse.Success = false;
                updateBirthEventCommandResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    updateBirthEventCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                updateBirthEventCommandResponse.Message = updateBirthEventCommandResponse.ValidationErrors[0];
            }
            if (updateBirthEventCommandResponse.Success)
            {
                var birthEvent = CustomMapper.Mapper.Map<BirthEvent>(request);
                birthEvent.Event.EventType = "BirthEvent";
                _birthEventRepository.Update(birthEvent);
                var result = await _birthEventRepository.SaveChangesAsync(cancellationToken);

                var supportingDocuments = birthEvent.Event.EventSupportingDocuments;
                var examptionDocuments = birthEvent.Event.PaymentExamption?.SupportingDocuments;

                _eventDocumentService.saveSupportingDocuments(supportingDocuments, examptionDocuments, "BirthEvents");
            }

            var modifiedBirthEvent = await _birthEventRepository.GetWithIncludedAsync(request.Id);
            var paymentRateResponse = CustomMapper.Mapper.Map<BirthEventDTO>(modifiedBirthEvent);

            return paymentRateResponse;
        }
    }
}