using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.DeathEvents.Command.Update
{
    public class UpdateDeathEventCommandHandler : IRequestHandler<UpdateDeathEventCommand, UpdateDeathEventCommandResponse>
    {
        private readonly IDeathEventRepository _deathEventRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEventDocumentService _eventDocumentService;
        public UpdateDeathEventCommandHandler(IDeathEventRepository deathEventRepository,
                                              IEventRepository eventRepository,
                                              IEventDocumentService eventDocumentService)
        {
            this._deathEventRepository = deathEventRepository;
            this._eventRepository = eventRepository;
            this._eventDocumentService = eventDocumentService;
        }
        public async Task<UpdateDeathEventCommandResponse> Handle(UpdateDeathEventCommand request, CancellationToken cancellationToken)
        {
            var executionStrategy = _deathEventRepository.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = _deathEventRepository.Database.BeginTransaction())
                {
                    try
                    {

                        var updateDeathEventCommandResponse = new UpdateDeathEventCommandResponse();

                        var validator = new UpdateDeathEventCommandValidator(_eventRepository);
                        var validationResult = await validator.ValidateAsync(request, cancellationToken);

                        //Check and log validation errors
                        if (validationResult.Errors.Count > 0)
                        {
                            updateDeathEventCommandResponse.Success = false;
                            updateDeathEventCommandResponse.Status = 400;
                            updateDeathEventCommandResponse.ValidationErrors = new List<string>();
                            foreach (var error in validationResult.Errors)
                                updateDeathEventCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                            updateDeathEventCommandResponse.Message = updateDeathEventCommandResponse.ValidationErrors[0];
                        }
                        if (updateDeathEventCommandResponse.Success)
                        {
                            //supporting docs cant be updated only new (one without id) are created
                            var supportingDocs = request.Event.EventSupportingDocuments?.Where(doc => doc.Id == null).ToList();
                            var examptionsupportingDocs = request.Event.PaymentExamption?.SupportingDocuments?.Where(doc => doc.Id == null).ToList();
                            request.Event.EventSupportingDocuments = null;
                            if (request.Event.PaymentExamption != null)
                            {
                                request.Event.PaymentExamption.SupportingDocuments = null;
                            }

                            var deathEvent = CustomMapper.Mapper.Map<DeathEvent>(request);
                            deathEvent.Event.EventType = "Death";
                            _deathEventRepository.Update(deathEvent);
                            var result = await _deathEventRepository.SaveChangesAsync(cancellationToken);

                            var supportingDocuments = deathEvent.Event.EventSupportingDocuments;
                            var examptionDocuments = deathEvent.Event.PaymentExamption?.SupportingDocuments;
                            var docs = await _eventDocumentService.createSupportingDocumentsAsync(supportingDocs, examptionsupportingDocs, deathEvent.EventId, deathEvent.Event.PaymentExamption?.Id, cancellationToken);

                            var personIds = new PersonIdObj
                            {
                                DeceasedId = deathEvent.Event.EventOwener.Id,
                                RegistrarId = deathEvent.Event.EventRegistrar?.RegistrarInfo.Id
                            };
                            var separatedDocs = _eventDocumentService.extractSupportingDocs(personIds, docs.supportingDocs);
                            _eventDocumentService.savePhotos(separatedDocs.userPhotos);
                            _eventDocumentService.saveSupportingDocuments(supportingDocuments, examptionDocuments, "DeathEvents");
                            await transaction.CommitAsync();
                            updateDeathEventCommandResponse.Success = true;
                            updateDeathEventCommandResponse.Status = 200;
                            updateDeathEventCommandResponse.Message = "Death Event Updated Succesfully.";
                        }
                        return updateDeathEventCommandResponse;

                        // var modifiedDeathEvent = await _deathEventRepository.GetIncludedAsync(request.Id);
                        // var paymentRateResponse = CustomMapper.Mapper.Map<DeathEventDTO>(modifiedDeathEvent);

                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }

            });
        }
    }
}