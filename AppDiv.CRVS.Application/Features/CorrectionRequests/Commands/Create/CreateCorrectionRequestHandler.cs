using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Application.Mapper;

namespace AppDiv.CRVS.Application.Features.CorrectionRequests.Commands
{
    public class CreateCorrectionRequestHandler : IRequestHandler<CreateCorrectionRequest, CreateCorrectionRequestResponse>
    {
        private readonly ICorrectionRequestRepostory _CorrectionRepository;
        public CreateCorrectionRequestHandler(ICorrectionRequestRepostory CorrectionRepository)
        {
            _CorrectionRepository = CorrectionRepository;


        }
        public async Task<CreateCorrectionRequestResponse> Handle(CreateCorrectionRequest request, CancellationToken cancellationToken)
        {


            var CreateAddressCommadResponse = new CreateCorrectionRequestResponse();
            request.CorrectionRequest.Request.RequestType = "CorrectionRequest";
            var CorrectionRequest = CustomMapper.Mapper.Map<CorrectionRequest>(request.CorrectionRequest);
            await _CorrectionRepository.InsertAsync(CorrectionRequest, cancellationToken);
            var result = await _CorrectionRepository.SaveChangesAsync(cancellationToken);


            return CreateAddressCommadResponse;
        }
    }
}
