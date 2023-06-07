using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.CorrectionRequests.Commands
{
    public class CreateCorrectionRequestHandler : IRequestHandler<CreateCorrectionRequest, CreateCorrectionRequestResponse>
    {
        private readonly ICorrectionRequestRepostory _CorrectionRepository;
        private readonly IWorkflowService _WorkflowService;
        public CreateCorrectionRequestHandler(ICorrectionRequestRepostory CorrectionRepository, IWorkflowService WorkflowService)
        {
            _CorrectionRepository = CorrectionRepository;
            _WorkflowService = WorkflowService;
        }

        public async Task<CreateCorrectionRequestResponse> Handle(CreateCorrectionRequest request, CancellationToken cancellationToken)
        {
            // var NewTranscation = new TransactionRequestDTO
            // {
            //     CurrentStep = request.currentStep,
            //     ApprovalStatus = IsApprove,
            //     WorkflowId = RequestId,
            //     RequestId = RequestId,
            //     CivilRegOfficerId = _UserResolverService.GetUserId().ToString(),
            //     Remark = Remark
            // };
            // await _TransactionService.CreateTransaction(NewTranscation);
            // await _NotificationService.CreateNotification(ReturnId, workflowType, workflowType,
            //                    this.GetReceiverGroupId(workflowType, request.currentStep), RequestId,
            //                   _UserResolverService.GetUserId().ToString());

            var CreateAddressCommadResponse = new CreateCorrectionRequestResponse();
            request.CorrectionRequest.Request.RequestType = "change";
            request.CorrectionRequest.Request.currentStep = 0;
            var CorrectionRequest = CustomMapper.Mapper.Map<CorrectionRequest>(request.CorrectionRequest);
            await _CorrectionRepository.InsertAsync(CorrectionRequest, cancellationToken);
            var result = await _CorrectionRepository.SaveChangesAsync(cancellationToken);
            return CreateAddressCommadResponse;
        }
    }
}
