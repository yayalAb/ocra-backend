using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;

namespace AppDiv.CRVS.Application.Features.Search
{
    public class ReIndexPersonalInfoCommand : IRequest<BaseResponse>
    {
    }


    public class ReIndexPersonalInfoCommandHandler : IRequestHandler<ReIndexPersonalInfoCommand, BaseResponse>
    {
        private readonly IPersonalInfoRepository _PersonalInfoRepository;
        public ReIndexPersonalInfoCommandHandler(IPersonalInfoRepository PersonalInfoRepository)
        {
            _PersonalInfoRepository = PersonalInfoRepository;
        }

        public async Task<BaseResponse> Handle(ReIndexPersonalInfoCommand request, CancellationToken cancellationToken)
        {
            await _PersonalInfoRepository.InitializePersonalInfoIndex(true);
            return new BaseResponse("personalInfo re-indexed successfully!");
        }

    }
}