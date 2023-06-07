using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;

namespace AppDiv.CRVS.Application.Features.PaymentExamptionRequests.Command.Create
{

    public class CreatePaymentExamptionRequestCommandHandler : IRequestHandler<CreatePaymentExamptionRequestCommand, CreatePaymentExamptionRequestCommandResponse>
    {
        private readonly IPaymentExamptionRequestRepository _paymentExamptionRequestRepository;
        public CreatePaymentExamptionRequestCommandHandler(IPaymentExamptionRequestRepository paymentExamptionRequestRepository)
        {
            _paymentExamptionRequestRepository = paymentExamptionRequestRepository;
        }
        public async Task<CreatePaymentExamptionRequestCommandResponse> Handle(CreatePaymentExamptionRequestCommand request, CancellationToken cancellationToken)
        {

            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var createPaymentExamptionRequestCommandResponse = new CreatePaymentExamptionRequestCommandResponse();

            var validator = new CreatePaymentExamptionRequestCommandValidator(_paymentExamptionRequestRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                createPaymentExamptionRequestCommandResponse.Success = false;
                createPaymentExamptionRequestCommandResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    createPaymentExamptionRequestCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                createPaymentExamptionRequestCommandResponse.Message = createPaymentExamptionRequestCommandResponse.ValidationErrors[0];
            }
            if (createPaymentExamptionRequestCommandResponse.Success)
            {

                var PaymentExamptionRequest = CustomMapper.Mapper.Map<PaymentExamptionRequest>(request);

                PaymentExamptionRequest.ExamptedById = "4d940006-b21f-4841-b8dd-02957c4d7487";
                PaymentExamptionRequest.Request.RequestType = "payment exemption";
                await _paymentExamptionRequestRepository.InsertAsync(PaymentExamptionRequest, cancellationToken);
                var result = await _paymentExamptionRequestRepository.SaveChangesAsync(cancellationToken);

                //var customerResponse = CustomerMapper.Mapper.Map<CustomerResponseDTO>(customer);
                // createCustomerCommandResponse.Customer = customerResponse;          
            }
            return createPaymentExamptionRequestCommandResponse;
        }
    }
}
