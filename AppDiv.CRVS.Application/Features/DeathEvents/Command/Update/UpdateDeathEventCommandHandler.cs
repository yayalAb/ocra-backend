using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;

namespace AppDiv.CRVS.Application.Features.DeathEvents.Command.Update
{
    public class UpdateDeathEventCommandHandler : IRequestHandler<UpdateDeathEventCommand, DeathEventDTO>
    {
        private readonly IDeathEventRepository _deathEventRepository;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IEventRepository _eventRepository;

        public UpdateDeathEventCommandHandler(IDeathEventRepository deathEventRepository,
                                              IEventDocumentService eventDocumentService,
                                              IEventRepository eventRepository)
        {
            this._eventRepository = eventRepository;
            this._deathEventRepository = deathEventRepository;
            this._eventDocumentService = eventDocumentService;
        }
        public async Task<DeathEventDTO> Handle(UpdateDeathEventCommand request, CancellationToken cancellationToken)
        {

            var updateDeathEventCommandResponse = new UpdateDeathEventCommandResponse();

            var validator = new UpdateDeathEventCommandValidator(_eventRepository, request);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                updateDeathEventCommandResponse.Success = false;
                updateDeathEventCommandResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    updateDeathEventCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                updateDeathEventCommandResponse.Message = updateDeathEventCommandResponse.ValidationErrors[0];
            }
            if (updateDeathEventCommandResponse.Success)
            {
                var deathEvent = CustomMapper.Mapper.Map<DeathEvent>(request);
                deathEvent.Event.EventType = "DeathEvent";
                _deathEventRepository.Update(deathEvent);
                var result = await _deathEventRepository.SaveChangesAsync(cancellationToken);

                var supportingDocuments = deathEvent.Event.EventSupportingDocuments;
                var examptionDocuments = deathEvent.Event.PaymentExamption?.SupportingDocuments;

                _eventDocumentService.saveSupportingDocuments(supportingDocuments, examptionDocuments, "DeathEvents");
            }

            var modifiedDeathEvent = await _deathEventRepository.GetIncludedAsync(request.Id);
            var paymentRateResponse = CustomMapper.Mapper.Map<DeathEventDTO>(modifiedDeathEvent);

            return paymentRateResponse;
        }
    }
}