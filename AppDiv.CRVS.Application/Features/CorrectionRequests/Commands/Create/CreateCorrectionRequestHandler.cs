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

            var CreateAddressCommadResponse = new CreateCorrectionRequestResponse();
            request.CorrectionRequest.Request.RequestType = "CorrectionRequest";
            request.CorrectionRequest.Request.currentStep = _WorkflowService.GetLastWorkflow("change");
            var CorrectionRequest = CustomMapper.Mapper.Map<CorrectionRequest>(request.CorrectionRequest);
            await _CorrectionRepository.InsertAsync(CorrectionRequest, cancellationToken);
            var result = await _CorrectionRepository.SaveChangesAsync(cancellationToken);
            return CreateAddressCommadResponse;
        }
    }
}
