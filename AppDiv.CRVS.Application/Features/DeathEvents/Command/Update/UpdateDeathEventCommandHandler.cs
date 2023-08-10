using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.DeathEvents.Command.Update
{
    // update death event command handler
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
                using var transaction = request.IsFromCommand ? null : _deathEventRepository.Database.BeginTransaction();
                try
                {

                    var response = new UpdateDeathEventCommandResponse();
                    // validate the request inputs.
                    var validator = new UpdateDeathEventCommandValidator(_eventRepository);
                    var validationResult = await validator.ValidateAsync(request, cancellationToken);

                    //Check and log validation errors
                    if (validationResult.Errors.Count > 0)
                    {
                        response.Success = false;
                        response.Status = 400;
                        response.ValidationErrors = new List<string>();
                        foreach (var error in validationResult.Errors)
                            response.ValidationErrors.Add(error.ErrorMessage);
                        response.Message = response.ValidationErrors[0];
                    }
                    if (response.Success)
                    {
                        // Validate the inputs first for the correction request approval.
                        if (request.ValidateFirst == true)
                        {
                            response.Updated(entity: "Death", message: "Valid Input.");
                            return response;
                        }
                        // Identify new and old supporting documents.
                        var supportingDocs = request.Event.EventSupportingDocuments?.Where(doc => doc.Id == null).ToList();
                        var examptionsupportingDocs = request.Event.PaymentExamption?.SupportingDocuments?.Where(doc => doc.Id == null).ToList();
                        var correctionSupportingDocs = request.Event.EventSupportingDocuments?.Where(doc => doc.Id != null).ToList();
                        var correctionExamptionsupportingDocs = request.Event.PaymentExamption?.SupportingDocuments?.Where(doc => doc.Id != null).ToList();
                        // Map the reques to model eintity.
                        var deathEvent = CustomMapper.Mapper.Map<DeathEvent>(request);
                        deathEvent.Event.EventType = "Death";
                        // Set the supporting documents to null.
                        deathEvent.Event.EventSupportingDocuments = null!;
                        if (deathEvent.Event.PaymentExamption != null)
                        {
                            deathEvent.Event.PaymentExamption.SupportingDocuments = null!;
                        }
                        // Set the daeth status of the person to true.
                        deathEvent.Event.EventOwener.DeathStatus = true;
                        // Update the Death Event.
                        await _deathEventRepository.UpdateWithNested(deathEvent, cancellationToken);
                        // persons id.
                        var personIds = new PersonIdObj
                        {
                            DeceasedId = deathEvent.Event.EventOwener != null ? deathEvent.Event.EventOwener.Id : deathEvent.Event.EventOwenerId,
                            RegistrarId = deathEvent.Event.EventRegistrar?.RegistrarInfo != null ? deathEvent.Event.EventRegistrar.RegistrarInfo.Id : deathEvent.Event.EventRegistrar?.RegistrarInfoId
                        };
                        // if the update not from the corection request.
                        if (!request.IsFromCommand)
                        {
                            // Save the supporting documents.
                            var docs = await _eventDocumentService.createSupportingDocumentsAsync(supportingDocs!, examptionsupportingDocs!, deathEvent.EventId, deathEvent.Event.PaymentExamption?.Id, cancellationToken);
                            var (userPhotos, fingerprints, otherDocs) = _eventDocumentService.extractSupportingDocs(personIds, docs.supportingDocs);
                            _eventDocumentService.savePhotos(userPhotos);
                            _eventDocumentService.saveSupportingDocuments((ICollection<SupportingDocument>)otherDocs, (ICollection<SupportingDocument>)docs.examptionDocs, "Death");
                            _eventDocumentService.saveFingerPrints(fingerprints);

                        }
                        else
                        {
                            deathEvent.Event.IsCertified = false;
                            var docs = await _eventDocumentService.createSupportingDocumentsAsync(correctionSupportingDocs!, correctionExamptionsupportingDocs!, deathEvent.EventId, deathEvent.Event.PaymentExamption?.Id, cancellationToken);
                            var (userPhotos, otherDocs) = _eventDocumentService.ExtractOldSupportingDocs(personIds, docs.supportingDocs);
                            if (userPhotos != null && (userPhotos.Count != 0))
                            {
                                _eventDocumentService.MovePhotos(userPhotos, "Death");
                            }
                            // _eventDocumentService.MoveSupportingDocuments((ICollection<SupportingDocument>)otherDocs, (ICollection<SupportingDocument>)docs.examptionDocs, "Death");

                        }
                        // Set the response to Updated.
                        response.Updated("Death Event");
                        // Commit the transaction.
                        if (!request.IsFromCommand)
                        {
                            await transaction?.CommitAsync()!;
                        }


                    }
                    return response;
                }
                catch (Exception)
                {
                    // Rollback the transaction on exception.
                    if (transaction != null)
                    {
                        await transaction?.RollbackAsync()!;
                    }
                    throw;
                }

            });
        }
    }
}
