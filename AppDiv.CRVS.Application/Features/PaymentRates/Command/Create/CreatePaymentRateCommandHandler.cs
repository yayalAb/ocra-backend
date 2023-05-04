using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;

namespace AppDiv.CRVS.Application.Features.PaymentRates.Command.Create
{

    public class CreatePaymentRateCommandHandler : IRequestHandler<CreatePaymentRateCommand, CreatePaymentRateCommandResponse>
    {
        private readonly IPaymentRateRepository _paymentRateRepository;
        public CreatePaymentRateCommandHandler(IPaymentRateRepository paymentRateRepository)
        {
            _paymentRateRepository = paymentRateRepository;
        }
        public async Task<CreatePaymentRateCommandResponse> Handle(CreatePaymentRateCommand request, CancellationToken cancellationToken)
        {

            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var createPaymentCommandResponse = new CreatePaymentRateCommandResponse();

            var validator = new CreatePaymentRateCommandValidator(_paymentRateRepository);
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

                var paymentRate = CustomMapper.Mapper.Map<PaymentRate>(request.PaymentRate);

                await _paymentRateRepository.InsertAsync(paymentRate, cancellationToken);
                var result = await _paymentRateRepository.SaveChangesAsync(cancellationToken);

                //var customerResponse = CustomerMapper.Mapper.Map<CustomerResponseDTO>(customer);
                // createCustomerCommandResponse.Customer = customerResponse;          
            }
            return createPaymentCommandResponse;
        }
    }
}
