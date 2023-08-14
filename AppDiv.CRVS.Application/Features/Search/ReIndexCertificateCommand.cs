using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;

namespace AppDiv.CRVS.Application.Features.Search
{
    public class ReIndexCertificateCommand : IRequest<BaseResponse>
    {
    }


    public class ReIndexCertificateCommandHandler : IRequestHandler<ReIndexCertificateCommand, BaseResponse>
    {
        private readonly ICertificateRepository _CertificateRepository;
        public ReIndexCertificateCommandHandler(ICertificateRepository CertificateRepository)
        {
            _CertificateRepository = CertificateRepository;
        }

        public async Task<BaseResponse> Handle(ReIndexCertificateCommand request, CancellationToken cancellationToken)
        { 
            await _CertificateRepository.InitializeCertificateIndex(true);
            return new BaseResponse("certificate re-indexed successfully!");
        }

    }
}