using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Update
{
    public class UpdateBirthEventCommandHandler : IRequestHandler<UpdateBirthEventCommand, UpdateBirthEventCommandResponse>
    {
        private readonly IBirthEventRepository _birthEventRepository;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IEventRepository _eventRepository;
        private readonly ILookupRepository _lookupRepository;

        public UpdateBirthEventCommandHandler(IBirthEventRepository birthEventRepository,
                                              IEventRepository eventRepository,
                                              ILookupRepository lookupRepository,
                                              IEventDocumentService eventDocumentService)
        {
            this._eventDocumentService = eventDocumentService;
            this._eventRepository = eventRepository;
            this._lookupRepository = lookupRepository;
            this._birthEventRepository = birthEventRepository;
        }
        public async Task<UpdateBirthEventCommandResponse> Handle(UpdateBirthEventCommand request, CancellationToken cancellationToken)
        {
            var executionStrategy = _birthEventRepository.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = request.IsFromCommand ? null : _birthEventRepository.Database.BeginTransaction();
                try
                {
                    // Create response.
                    var response = new UpdateBirthEventCommandResponse();
                    // Validate the inputs.
                    var validator = new UpdateBirthEventCommandValidator(_eventRepository, _lookupRepository);
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
                        if (request.ValidateFirst == true)
                        {
                            response.Updated(entity: "Birth", message: "Valid Input.");
                            return response;
                        }
                        try
                        {
                            // Get newly added supporting documents and examption documents.
                            var supportingDocs = request.Event.EventSupportingDocuments?.Where(doc => doc.Id == null)?.ToList();
                            var examptionsupportingDocs = request.Event.PaymentExamption?.SupportingDocuments?.Where(doc => doc.Id == null)?.ToList();
                            // Get old supporting documents and examption documents.
                            var correctionSupportingDocs = request.Event.EventSupportingDocuments?.Where(doc => doc.Id != null).ToList();
                            var correctionExamptionsupportingDocs = request.Event.PaymentExamption?.SupportingDocuments?.Where(doc => doc.Id != null).ToList();
                            // Map the reques to the model entity.
                            var birthEvent = CustomMapper.Mapper.Map<BirthEvent>(request);
                            birthEvent.Event.EventType = "Birth";
                            if (request.Event.InformantType == "guardian" && ValidationService.HaveGuardianSupportingDoc(request.Event.EventSupportingDocuments, _lookupRepository))
                            {
                                birthEvent.Event.HasPendingDocumentApproval = true;
                            }
                            // Set the supporting documents and exemption documents null
                            birthEvent.Event.EventSupportingDocuments = null!;
                            if (birthEvent.Event.PaymentExamption != null)
                            {
                                birthEvent.Event.PaymentExamption.SupportingDocuments = null!;
                            }
                            // Update the birth event.
                            _birthEventRepository.UpdateAll(birthEvent);
                            // person ids
                            var personIds = new PersonIdObj
                            {
                                MotherId = birthEvent.Mother != null ? birthEvent.Mother.Id : birthEvent.MotherId,
                                FatherId = birthEvent.Father != null ? birthEvent.Father.Id : birthEvent.FatherId,
                                ChildId = birthEvent.Event.EventOwener != null ? birthEvent.Event.EventOwener.Id : birthEvent.Event.EventOwenerId,
                                RegistrarId = birthEvent.Event.EventRegistrar?.RegistrarInfo != null ? birthEvent.Event.EventRegistrar?.RegistrarInfo.Id : birthEvent.Event.EventRegistrar?.RegistrarInfoId
                            };
                            // for requests not from correction request
                            if (!request.IsFromCommand)
                            {
                                // Save the newly added supporting documents and exemption documents.
                                var docs = await _eventDocumentService.createSupportingDocumentsAsync(supportingDocs!, examptionsupportingDocs!, (Guid)birthEvent.EventId, birthEvent.Event.PaymentExamption?.Id, cancellationToken);
                                var result = await _birthEventRepository.SaveChangesAsync(cancellationToken);
                                var (userPhotos, fingerprints, otherDocs) = _eventDocumentService.extractSupportingDocs(personIds, docs.supportingDocs);
                                _eventDocumentService.savePhotos(userPhotos);
                                _eventDocumentService.saveSupportingDocuments((ICollection<SupportingDocument>)otherDocs, (ICollection<SupportingDocument>)docs.examptionDocs, "Birth");
                                _eventDocumentService.saveFingerPrints(fingerprints);

                            }
                            else
                            {
                                // Move the supporting documents form temporary to permenant place.
                                var docs = await _eventDocumentService.createSupportingDocumentsAsync(correctionSupportingDocs!, correctionExamptionsupportingDocs!, (Guid)birthEvent.EventId, birthEvent.Event.PaymentExamption?.Id, cancellationToken);
                                var result = await _birthEventRepository.SaveChangesAsync(cancellationToken);
                                var (userPhotos, otherDocs) = _eventDocumentService.ExtractOldSupportingDocs(personIds, docs.supportingDocs);
                                _eventDocumentService.MovePhotos(userPhotos, "Birth");
                                _eventDocumentService.MoveSupportingDocuments((ICollection<SupportingDocument>)otherDocs, (ICollection<SupportingDocument>)docs.examptionDocs, "Birth");
                                //TODO:save fingerprint
                            }
                            // Commit the transaction.
                            await transaction?.CommitAsync()!;
                        }
                        catch (System.Exception ex)
                        {
                            response.BadRequest($"Something went wrong.");
                            throw new ApplicationException(ex.Message);
                        }
                        // Set response to created.
                        response.Updated("Birth Event");
                    }

                    return response;
                }
                catch (Exception)
                {
                    // Rollback the transaction on exception.
                    await transaction?.RollbackAsync()!;
                    throw;
                }

            });
        }
    }
}