using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Create
{

    public class CreateBirthEventCommandHandler : IRequestHandler<CreateBirthEventCommand, CreateBirthEventCommandResponse>
    {
        private readonly IBirthEventRepository _birthEventRepository;
        private readonly IEventDocumentService _eventDocumentService;
        public CreateBirthEventCommandHandler(IBirthEventRepository birthEventRepository, IEventDocumentService eventDocumentService)
        {
            this._eventDocumentService = eventDocumentService;
            this._birthEventRepository = birthEventRepository;
        }
        public async Task<CreateBirthEventCommandResponse> Handle(CreateBirthEventCommand request, CancellationToken cancellationToken)
        {

            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var createPaymentCommandResponse = new CreateBirthEventCommandResponse();

            var validator = new CreateBirthEventCommandValidator(_birthEventRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                createPaymentCommandResponse.Success = false;
                createPaymentCommandResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    createPaymentCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                createPaymentCommandResponse.Message = createPaymentCommandResponse.ValidationErrors[0];
            }
            if (createPaymentCommandResponse.Success)
            {
                // var docs = await _groupRepository.GetMultipleUserGroups(request.UserGroups);

                var birthEvent = CustomMapper.Mapper.Map<BirthEvent>(request.BirthEvent);
                birthEvent.Event.EventType = "BirthEvent";

                await _birthEventRepository.InsertOrUpdateAsync(birthEvent, cancellationToken);
                var result = await _birthEventRepository.SaveChangesAsync(cancellationToken);

                var supportingDocuments = birthEvent.Event.EventSupportingDocuments;
                var examptionDocuments = birthEvent.Event.PaymentExamption?.SupportingDocuments;

                _eventDocumentService.saveSupportingDocuments(supportingDocuments, examptionDocuments, "BirthEvents");

            }
            return createPaymentCommandResponse;
        }
    }
}
