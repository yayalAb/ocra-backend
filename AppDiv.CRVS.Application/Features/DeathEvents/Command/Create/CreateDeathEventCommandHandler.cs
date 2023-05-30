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
using AppDiv.CRVS.Utility.Services;

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
        private readonly ISmsService _smsService;
        private readonly IEventPaymentRequestService _paymentRequestService;
        public CreateDeathEventCommandHandler(IDeathEventRepository deathEventRepository,
                                              IEventDocumentService eventDocumentService,
                                              ILookupRepository lookupRepository,
                                              IAddressLookupRepository addressRepository,
                                              IPersonalInfoRepository person,
                                              IPaymentExamptionRequestRepository paymentExamption,
                                              ISmsService smsService,
                                              IEventPaymentRequestService paymentRequestService)

        {
            _deathEventRepository = deathEventRepository;
            _eventDocumentService = eventDocumentService;
            _addressRepository = addressRepository;
            _lookupRepository = lookupRepository;
            _person = person;
            _paymentExamption = paymentExamption;
            _smsService = smsService;
            _paymentRequestService = paymentRequestService;
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
                            createDeathCommandResponse.Status = 400;
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


                                await _deathEventRepository.InsertOrUpdateAsync(deathEvent, cancellationToken);
                                var result = await _deathEventRepository.SaveChangesAsync(cancellationToken);

                                var supportingDocuments = deathEvent.Event.EventSupportingDocuments;
                                var examptionDocuments = deathEvent.Event.PaymentExamption?.SupportingDocuments;

                                _eventDocumentService.saveSupportingDocuments(supportingDocuments, examptionDocuments, "Death");
                                if (!deathEvent.Event.IsExampted)
                                {
                                    (float amount , string code) response = await _paymentRequestService.CreatePaymentRequest("Death", deathEvent.Event, cancellationToken);
                                    string message = $"Dear Customer,\nThis is to inform you that your request for Death certificate from OCRA is currently being processed. To proceed with the issuance, kindly make a payment of {response.amount} ETB to finance office using code {response.code}.\n OCRA";
                                    if (deathEvent.Event.EventRegistrar?.RegistrarInfo.PhoneNumber != null)
                                    {
                                        await _smsService.SendSMS(deathEvent.Event.EventRegistrar.RegistrarInfo.PhoneNumber, message);
                                    }
                                    //
                                }
                            }
                            catch (System.Exception ex)
                            {
                                createDeathCommandResponse.Success = false;
                                createDeathCommandResponse.Status = 400;
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
