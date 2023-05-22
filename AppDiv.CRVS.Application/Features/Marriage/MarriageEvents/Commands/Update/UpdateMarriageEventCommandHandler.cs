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

namespace AppDiv.CRVS.Application.Features.MarriageEvents.Command.Update
{

    public class UpdateMarriageEventCommandHandler : IRequestHandler<UpdateMarriageEventCommand, UpdateMarriageEventCommandResponse>
    {
        private readonly IMarriageEventRepository _marriageEventRepository;
        private readonly IPersonalInfoRepository _personalInfoRepository;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IMarriageApplicationRepository _marriageApplicationRepository;
        private readonly ILookupRepository _lookupRepository;
        private readonly IDivorceEventRepository _divorceEventRepository;
        private readonly IEventPaymentRequestService _paymentRequestService;
        private readonly IAddressLookupRepository _addressRepository;
        private readonly IPaymentExamptionRequestRepository _paymentExamptionRequestRepository;

        public UpdateMarriageEventCommandHandler(IMarriageEventRepository marriageEventRepository,
                                                 IPersonalInfoRepository personalInfoRepository,
                                                 IEventDocumentService eventDocumentService,
                                                 IMarriageApplicationRepository marriageApplicationRepository,
                                                 ILookupRepository lookupRepository,
                                                 IDivorceEventRepository divorceEventRepository,
                                                 IEventPaymentRequestService paymentRequestService,
                                                 IAddressLookupRepository addressRepository,
                                                 IPaymentExamptionRequestRepository paymentExamptionRequestRepository
                                                 )
        {
            _marriageEventRepository = marriageEventRepository;
            _personalInfoRepository = personalInfoRepository;
            _eventDocumentService = eventDocumentService;
            _marriageApplicationRepository = marriageApplicationRepository;
            _lookupRepository = lookupRepository;
            _divorceEventRepository = divorceEventRepository;
            _paymentRequestService = paymentRequestService;
            _addressRepository = addressRepository;
            _paymentExamptionRequestRepository = paymentExamptionRequestRepository;
        }

        public async Task<UpdateMarriageEventCommandResponse> Handle(UpdateMarriageEventCommand request, CancellationToken cancellationToken)
        {
            var executionStrategy = _marriageEventRepository.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {

                using (var transaction = _marriageEventRepository.Database.BeginTransaction())
                {

                    try

                    {
                        var updateMarriageEventCommandResponse = new UpdateMarriageEventCommandResponse();

                        var validator = new UpdateMarriageEventCommandValidator(_lookupRepository, _marriageApplicationRepository, _personalInfoRepository, _divorceEventRepository, _marriageEventRepository, _paymentExamptionRequestRepository, _addressRepository);
                        var validationResult = await validator.ValidateAsync(request, cancellationToken);

                        //Check and log validation errors
                        if (validationResult.Errors.Count > 0)
                        {
                            updateMarriageEventCommandResponse.Success = false;
                            updateMarriageEventCommandResponse.ValidationErrors = new List<string>();
                            foreach (var error in validationResult.Errors)
                                updateMarriageEventCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                            updateMarriageEventCommandResponse.Message = updateMarriageEventCommandResponse.ValidationErrors[0];
                        }
                        if (updateMarriageEventCommandResponse.Success)
                        {

                            var marriageEvent = CustomMapper.Mapper.Map<MarriageEvent>(request);
                            marriageEvent.Event.EventType = "Marriage";
                           await _marriageEventRepository.EFUpdateAsync(marriageEvent);
                            // await _marriageEventRepository.InsertOrUpdateAsync(marriageEvent, true, cancellationToken);
                            await _marriageEventRepository.SaveChangesAsync(cancellationToken);

                            var eventSupportingDocuments = marriageEvent.Event.EventSupportingDocuments;
                            var examptionSupportingDocuments = marriageEvent.Event?.PaymentExamption?.SupportingDocuments;
                            _eventDocumentService.saveSupportingDocuments(eventSupportingDocuments, examptionSupportingDocuments, "Marriage");
                            updateMarriageEventCommandResponse.Message = "Marriage Event Updated Successfully";
                            await transaction.CommitAsync();
                        }
                        return updateMarriageEventCommandResponse;

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
