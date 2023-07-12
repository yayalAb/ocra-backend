using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Application.Features.PaymentExamptionRequests.Command.Create
{

    public class CreatePaymentExamptionRequestCommandHandler : IRequestHandler<CreatePaymentExamptionRequestCommand, CreatePaymentExamptionRequestCommandResponse>
    {
        private readonly IPaymentExamptionRequestRepository _paymentExamptionRequestRepository;
        private readonly ITransactionService _transactionService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly IWorkflowService _WorkflowService;
        private readonly IWorkflowRepository _WorkflowRepository;
        public CreatePaymentExamptionRequestCommandHandler(IWorkflowRepository WorkflowRepository,
                                                           IWorkflowService WorkflowService,
                                                           IPaymentExamptionRequestRepository paymentExamptionRequestRepository,
                                                           ITransactionService transactionService,
                                                           INotificationService notificationService,
                                                           IUserRepository userRepository
                                                           )
        {
            _paymentExamptionRequestRepository = paymentExamptionRequestRepository;
            _transactionService = transactionService;
            _notificationService = notificationService;
            _userRepository = userRepository;
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
                throw new NotFoundException("payment Exemption Work Flow Does not exist Pleace Create Workflow First");
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

                string? userId = _userRepository.GetAll()
                               .Where(u => u.PersonalInfoId == request.Request.CivilRegOfficerId)
                               .Select(u => u.Id).FirstOrDefault();
                if (userId == null)
                {
                    throw new NotFoundException("user not found");
                }
                var NewTranscation = new TransactionRequestDTO
                {
                    CurrentStep = 0,
                    ApprovalStatus = true,
                    WorkflowId = WorkflowId,
                    RequestId = PaymentExamptionRequest.Request.Id,
                    CivilRegOfficerId = userId,
                    Remark = "payment Exemption Request" //TODO:remark or reason from user
                };

                await _transactionService.CreateTransaction(NewTranscation);
                await _notificationService.CreateNotification(PaymentExamptionRequest.Request.Id, PaymentExamptionRequest.Request.RequestType, "payment Exemption Request",
                                   _WorkflowService.GetReceiverGroupId("payment exemption", (int)PaymentExamptionRequest.Request.NextStep), PaymentExamptionRequest.Request.Id,
                                 userId);


                //var customerResponse = CustomerMapper.Mapper.Map<CustomerResponseDTO>(customer);
                // createCustomerCommandResponse.Customer = customerResponse;          
            }
            return createPaymentExamptionRequestCommandResponse;
        }
    }
}
