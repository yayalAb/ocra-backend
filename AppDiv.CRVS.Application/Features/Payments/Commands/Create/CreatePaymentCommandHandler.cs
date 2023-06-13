using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.Payments.Command.Create
{

    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, CreatePaymentCommandResponse>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentRequestRepository _PaymentRequestRepository;
        private readonly IUpdateEventPaymetnService _UpdateEventPaymetnService;

        public CreatePaymentCommandHandler(IUpdateEventPaymetnService UpdateEventPaymetnService, IPaymentRepository paymentRepository, IPaymentRequestRepository PaymentRequestRepository)
        {
            _paymentRepository = paymentRepository;
            _PaymentRequestRepository = PaymentRequestRepository;
            _UpdateEventPaymetnService = UpdateEventPaymetnService;
        }
        public async Task<CreatePaymentCommandResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {

            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var createPaymentCommandResponse = new CreatePaymentCommandResponse();

            var validator = new CreatePaymentCommandValidator(_paymentRepository);
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
                var payment = CustomMapper.Mapper.Map<Payment>(request);
                await _paymentRepository.InsertAsync(payment, cancellationToken);
                await _UpdateEventPaymetnService.UpdatePaymetnStatus(request.PaymentRequestId, cancellationToken);
                await _paymentRepository.SaveChangesAsync(cancellationToken);
                createPaymentCommandResponse.Message = "payment created successfully";

            }
            return createPaymentCommandResponse;
        }
    }
}
