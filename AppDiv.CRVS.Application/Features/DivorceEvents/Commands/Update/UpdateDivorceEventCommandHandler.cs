using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.DivorceEvents.Command.Update
{

    public class UpdateDivorceEventCommandHandler : IRequestHandler<UpdateDivorceEventCommand, UpdateDivorceEventCommandResponse>
    {
        private readonly IDivorceEventRepository _DivorceEventRepository;
        private readonly IPersonalInfoRepository _personalInfoRepository;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly ILookupRepository _lookupRepository;
        private readonly IAddressLookupRepository _addressLookupRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ICourtRepository _courtRepository;

        public UpdateDivorceEventCommandHandler(IDivorceEventRepository DivorceEventRepository,
                                                IPersonalInfoRepository personalInfoRepository,
                                                IEventDocumentService eventDocumentService,
                                                ILookupRepository lookupRepository,
                                                IAddressLookupRepository addressLookupRepository,
                                                IEventRepository eventRepository,
                                                ICourtRepository courtRepository)
        {
            _DivorceEventRepository = DivorceEventRepository;
            _personalInfoRepository = personalInfoRepository;
            _eventDocumentService = eventDocumentService;
            _lookupRepository = lookupRepository;
            _addressLookupRepository = addressLookupRepository;
            _eventRepository = eventRepository;
            _courtRepository = courtRepository;
        }
        public async Task<UpdateDivorceEventCommandResponse> Handle(UpdateDivorceEventCommand request, CancellationToken cancellationToken)
        {
            var executionStrategy = _DivorceEventRepository.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {

                using (var transaction = _DivorceEventRepository.Database.BeginTransaction())
                {
                    try
                    {
                        var updateDivorceEventCommandResponse = new UpdateDivorceEventCommandResponse();

                        var validator = new UpdateDivorceEventCommandValidator(_personalInfoRepository, _lookupRepository, _addressLookupRepository, _eventRepository, _courtRepository);
                        var validationResult = await validator.ValidateAsync(request, cancellationToken);

                        //Check and log validation errors
                        if (validationResult.Errors.Count > 0)
                        {
                            updateDivorceEventCommandResponse.Success = false;
                            updateDivorceEventCommandResponse.ValidationErrors = new List<string>();
                            foreach (var error in validationResult.Errors)
                                updateDivorceEventCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                            updateDivorceEventCommandResponse.Message = updateDivorceEventCommandResponse.ValidationErrors[0];
                            updateDivorceEventCommandResponse.Status = 400;
                        }
                        if (updateDivorceEventCommandResponse.Success)
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

                            var divorceEvent = CustomMapper.Mapper.Map<DivorceEvent>(request);
                            divorceEvent.Event.EventType = "Divorce";
                            //   await _DivorceEventRepository.InsertOrUpdateAsync(divorceEvent,true,cancellationToken);
                            _DivorceEventRepository.EFUpdate(divorceEvent);
                            await _DivorceEventRepository.SaveChangesAsync(cancellationToken);

                            var docs = await _eventDocumentService.createSupportingDocumentsAsync(supportingDocs, examptionsupportingDocs, divorceEvent.EventId, divorceEvent.Event.PaymentExamption?.Id, cancellationToken);
                            var personIds = new PersonIdObj
                            {
                                WifeId = divorceEvent.DivorcedWife.Id,
                                HusbandId = divorceEvent.Event.EventOwener.Id
                            };
                            var separatedDocs = _eventDocumentService.extractSupportingDocs(personIds, docs.supportingDocs);
                            _eventDocumentService.savePhotos(separatedDocs.userPhotos);

                            _eventDocumentService.saveSupportingDocuments((ICollection<SupportingDocument>)separatedDocs.otherDocs, divorceEvent.Event.PaymentExamption?.SupportingDocuments, "Divorce");
                            updateDivorceEventCommandResponse.Message = "Divorce event Updated successfully";

                        }
                        await transaction.CommitAsync();
                        return updateDivorceEventCommandResponse;
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
