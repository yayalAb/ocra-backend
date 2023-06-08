using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Update
{
    public class UpdateBirthEventCommandHandler : IRequestHandler<UpdateBirthEventCommand, UpdateBirthEventCommandResponse>
    {
        private readonly IBirthEventRepository _birthEventRepository;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IEventPaymentRequestService _paymentRequestService;
        private readonly IEventRepository _eventRepository;

        public UpdateBirthEventCommandHandler(IBirthEventRepository birthEventRepository,
                                              IEventRepository eventRepository,
                                              IEventDocumentService eventDocumentService,
                                              IEventPaymentRequestService paymentRequestService)
        {
            this._eventDocumentService = eventDocumentService;
            this._eventRepository = eventRepository;
            this._birthEventRepository = birthEventRepository;
            this._paymentRequestService = paymentRequestService;
        }
        public async Task<UpdateBirthEventCommandResponse> Handle(UpdateBirthEventCommand request, CancellationToken cancellationToken)
        {
            var executionStrategy = _birthEventRepository.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = _birthEventRepository.Database.BeginTransaction())
                {
                    try
                    {
                        var updateBirthEventCommandResponse = new UpdateBirthEventCommandResponse();
                        // var birth = await _birthEventRepository.GetWithIncludedAsync(request.Id);
                        var validator = new UpdateBirthEventCommandValidator(_eventRepository);
                        var validationResult = await validator.ValidateAsync(request, cancellationToken);
                        //Check and log validation errors
                        if (validationResult.Errors.Count > 0)
                        {
                            updateBirthEventCommandResponse.Success = false;
                            updateBirthEventCommandResponse.Status = 400;
                            updateBirthEventCommandResponse.ValidationErrors = new List<string>();
                            foreach (var error in validationResult.Errors)
                                updateBirthEventCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                            updateBirthEventCommandResponse.Message = updateBirthEventCommandResponse.ValidationErrors[0];
                        }
                        if (updateBirthEventCommandResponse.Success)
                        {
                            try
                            {
                                //supporting docs cant be updated only new (one without id) are created
                                var supportingDocs = request.Event.EventSupportingDocuments?.Where(doc => doc.Id == null).ToList();
                                var examptionsupportingDocs = request.Event.PaymentExamption?.SupportingDocuments?.Where(doc => doc.Id == null).ToList();
                                request.Event.EventSupportingDocuments = null;
                                if (request.Event.PaymentExamption != null)
                                {
                                    request.Event.PaymentExamption.SupportingDocuments = null;
                                }
                                //////

                                var birthEvent = CustomMapper.Mapper.Map<BirthEvent>(request);
                                birthEvent.Event.EventType = "Birth";

                                _birthEventRepository.UpdateAll(birthEvent);
                                var result = await _birthEventRepository.SaveChangesAsync(cancellationToken);

                                // var supportingDocuments = birthEvent.Event.EventSupportingDocuments;
                                var examptionDocuments = birthEvent.Event.PaymentExamption?.SupportingDocuments;
                                var docs = await _eventDocumentService.createSupportingDocumentsAsync(supportingDocs, examptionsupportingDocs, birthEvent.EventId, birthEvent.Event.PaymentExamption?.Id, cancellationToken);
                                var personIds = new PersonIdObj
                                {
                                    MotherId = birthEvent.Mother.Id,
                                    FatherId = birthEvent.Father.Id,
                                    ChildId = birthEvent.Event.EventOwener.Id,
                                    RegistrarId = birthEvent.Event.EventRegistrar?.RegistrarInfo.Id
                                };
                                var separatedDocs = _eventDocumentService.extractSupportingDocs(personIds, docs.supportingDocs);
                                _eventDocumentService.savePhotos(separatedDocs.userPhotos);

                                _eventDocumentService.saveSupportingDocuments((ICollection<SupportingDocument>)separatedDocs.otherDocs, examptionDocuments, "Birth");
                            }
                            catch (System.Exception)
                            {
                                updateBirthEventCommandResponse.Message = "Something went wrong.";
                                updateBirthEventCommandResponse.Status = 400;
                                throw;
                            }
                            updateBirthEventCommandResponse.Message = "Birth Event Updated Successfully";
                            updateBirthEventCommandResponse.Status = 200;
                        }

                        return updateBirthEventCommandResponse;
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