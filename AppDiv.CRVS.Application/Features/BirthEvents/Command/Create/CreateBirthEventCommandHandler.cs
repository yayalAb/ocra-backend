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

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Create
{

    public class CreateBirthEventCommandHandler : IRequestHandler<CreateBirthEventCommand, CreateBirthEventCommandResponse>
    {
        private readonly IBirthEventRepository _birthEventRepository;
        private readonly ILookupRepository _lookupRepository;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IAddressLookupRepository _addressRepository;
        private readonly IPersonalInfoRepository _person;
        private readonly IPaymentExamptionRequestRepository _paymentExamption;
        private readonly IEventPaymentRequestService _paymentRequestService;

        public CreateBirthEventCommandHandler(IBirthEventRepository birthEventRepository,
                                              IEventDocumentService eventDocumentService,
                                              ILookupRepository lookupRepository,
                                              IAddressLookupRepository addressRepository,
                                              IPersonalInfoRepository person,
                                              IPaymentExamptionRequestRepository paymentExamption,
                                              IEventPaymentRequestService paymentRequestService)
        {
            this._eventDocumentService = eventDocumentService;
            this._birthEventRepository = birthEventRepository;
            this._addressRepository = addressRepository;
            this._lookupRepository = lookupRepository;
            this._person = person;
            this._paymentExamption = paymentExamption;
            this._paymentRequestService = paymentRequestService;
        }
        public async Task<CreateBirthEventCommandResponse> Handle(CreateBirthEventCommand request, CancellationToken cancellationToken)
        {
            var executionStrategy = _birthEventRepository.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {

                using (var transaction = _birthEventRepository.Database.BeginTransaction())
                {

                    try

                    {
                        // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

                        var createBirthEventCommandResponse = new CreateBirthEventCommandResponse();

                        var validator = new CreateBirthEventCommandValidator((_lookupRepository, _addressRepository, _person, _paymentExamption), request);
                        var validationResult = await validator.ValidateAsync(request, cancellationToken);

                        //Check and log validation errors
                        if (validationResult.Errors.Count > 0)
                        {
                            createBirthEventCommandResponse.Success = false;
                            createBirthEventCommandResponse.Status = 400;
                            createBirthEventCommandResponse.ValidationErrors = new List<string>();
                            foreach (var error in validationResult.Errors)
                                createBirthEventCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                            createBirthEventCommandResponse.Message = createBirthEventCommandResponse.ValidationErrors[0];
                        }
                        if (createBirthEventCommandResponse.Success)
                        {
                            // var docs = await _groupRepository.GetMultipleUserGroups(request.UserGroups);

                            try
                            {
                                var birthEvent = CustomMapper.Mapper.Map<BirthEvent>(request.BirthEvent);
                                birthEvent.Event.EventType = "Birth";

                                await _birthEventRepository.InsertOrUpdateAsync(birthEvent, cancellationToken);
                                var result = await _birthEventRepository.SaveChangesAsync(cancellationToken);

                                var supportingDocuments = birthEvent.Event.EventSupportingDocuments;
                                var examptionDocuments = birthEvent.Event.PaymentExamption?.SupportingDocuments;

                                _eventDocumentService.saveSupportingDocuments(supportingDocuments, examptionDocuments, "Birth");
                                if (!birthEvent.Event.IsExampted)
                                {
                                    await _paymentRequestService.CreatePaymentRequest("Birth", birthEvent.Event.Id, cancellationToken);
                                }

                            }
                            catch (System.Exception ex)
                            {
                                createBirthEventCommandResponse.Success = false;
                                createBirthEventCommandResponse.Status = 400;
                                throw;
                            }
                            createBirthEventCommandResponse.Message = "Birth Event created Successfully";
                            createBirthEventCommandResponse.Status = 200;
                            await transaction.CommitAsync();
                        }
                        return createBirthEventCommandResponse;

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
