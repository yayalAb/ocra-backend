using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.DeathEvents.Command.Create
{

    public class CreateDeathEventCommandHandler : IRequestHandler<CreateDeathEventCommand, CreateDeathEventCommandResponse>
    {
        private readonly IDeathEventRepository _deathEventRepository;
        private readonly ILookupRepository _lookupRepository;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IAddressLookupRepository _addressRepository;
        private readonly IPersonalInfoRepository _person;
        private readonly IPaymentExamptionRequestRepository _paymentExamption;
        private readonly IEventPaymentRequestService _paymentRequestService;
        public CreateDeathEventCommandHandler(IDeathEventRepository deathEventRepository,
                                              IEventDocumentService eventDocumentService,
                                              ILookupRepository lookupRepository,
                                              IAddressLookupRepository addressRepository,
                                              IPersonalInfoRepository person,
                                              IPaymentExamptionRequestRepository paymentExamption,
                                              IEventPaymentRequestService paymentRequestService)

        {
            this._deathEventRepository = deathEventRepository;
            this._eventDocumentService = eventDocumentService;
            this._addressRepository = addressRepository;
            this._lookupRepository = lookupRepository;
            this._person = person;
            this._paymentExamption = paymentExamption;
            this._paymentRequestService = paymentRequestService;
        }
        public async Task<CreateDeathEventCommandResponse> Handle(CreateDeathEventCommand request, CancellationToken cancellationToken)
        {
            var executionStrategy = _deathEventRepository.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {

                using (var transaction = _deathEventRepository.Database.BeginTransaction())
                {
                    try
                    {
                        var createDeathCommandResponse = new CreateDeathEventCommandResponse();

                        var validator = new CreateDeathEventCommandValidator((_lookupRepository, _addressRepository, _person, _paymentExamption), request);
                        var validationResult = await validator.ValidateAsync(request, cancellationToken);

                        //Check and log validation errors
                        if (validationResult.Errors.Count > 0)
                        {
                            createDeathCommandResponse.Success = false;
                            createDeathCommandResponse.ValidationErrors = new List<string>();
                            foreach (var error in validationResult.Errors)
                                createDeathCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                            createDeathCommandResponse.Message = createDeathCommandResponse.ValidationErrors[0];
                        }
                        if (createDeathCommandResponse.Success)
                        {
                            try
                            {
                                request.DeathEvent.DuringDeath = request.DeathEvent.DuringDeath == "" ? "Null" : request.DeathEvent.DuringDeath;
                                var deathEvent = CustomMapper.Mapper.Map<DeathEvent>(request.DeathEvent);
                                deathEvent.Event.EventType = "Death";

                                await _deathEventRepository.InsertOrUpdateAsync(deathEvent, cancellationToken);
                                var result = await _deathEventRepository.SaveChangesAsync(cancellationToken);

                                var supportingDocuments = deathEvent.Event.EventSupportingDocuments;
                                var examptionDocuments = deathEvent.Event.PaymentExamption?.SupportingDocuments;

                                _eventDocumentService.saveSupportingDocuments(supportingDocuments, examptionDocuments, "Death");
                                if (!deathEvent.Event.IsExampted)
                                {
                                    await _paymentRequestService.CreatePaymentRequest("Death", deathEvent.Event.Id, cancellationToken);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                createDeathCommandResponse.Success = false;
                                throw;
                            }
                            createDeathCommandResponse.Message = "Death Event created Successfully";
                            createDeathCommandResponse.Status = 200;
                            await transaction.CommitAsync();
                        }
                        return createDeathCommandResponse;
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
