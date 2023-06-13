using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.PaymentExamptionRequests.Command.Create
{

    public class CreatePaymentExamptionRequestCommandHandler : IRequestHandler<CreatePaymentExamptionRequestCommand, CreatePaymentExamptionRequestCommandResponse>
    {
        private readonly IPaymentExamptionRequestRepository _paymentExamptionRequestRepository;
        private readonly IWorkflowService _WorkflowService;
        private readonly IWorkflowRepository _WorkflowRepository;
        public CreatePaymentExamptionRequestCommandHandler(IWorkflowRepository WorkflowRepository, IWorkflowService WorkflowService, IPaymentExamptionRequestRepository paymentExamptionRequestRepository)
        {
            _paymentExamptionRequestRepository = paymentExamptionRequestRepository;
            _WorkflowService = WorkflowService;
            _WorkflowRepository = WorkflowRepository;
        }
        public async Task<CreatePaymentExamptionRequestCommandResponse> Handle(CreatePaymentExamptionRequestCommand request, CancellationToken cancellationToken)
        {

            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var createPaymentExamptionRequestCommandResponse = new CreatePaymentExamptionRequestCommandResponse();

            var validator = new CreatePaymentExamptionRequestCommandValidator(_paymentExamptionRequestRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            Guid WorkflowId = _WorkflowRepository.GetAll()
          .Where(wf => wf.workflowName == "payment exemption").Select(x => x.Id).FirstOrDefault();
            if (WorkflowId == null || WorkflowId == Guid.Empty)
            {
                throw new Exception("authentication Work Flow Does not exist Pleace Create Workflow First");
            }

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

                PaymentExamptionRequest.Request.RequestType = "payment exemption";
                PaymentExamptionRequest.Request.WorkflowId = WorkflowId;
                PaymentExamptionRequest.Request.NextStep = _WorkflowService.GetNextStep("payment exemption", 0, true);
                await _paymentExamptionRequestRepository.InsertAsync(PaymentExamptionRequest, cancellationToken);
                var result = await _paymentExamptionRequestRepository.SaveChangesAsync(cancellationToken);

                //var customerResponse = CustomerMapper.Mapper.Map<CustomerResponseDTO>(customer);
                // createCustomerCommandResponse.Customer = customerResponse;          
            }
            return createPaymentExamptionRequestCommandResponse;
        }
    }
}
