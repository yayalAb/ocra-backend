
using System.Text;
using System.Text.Json;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;

namespace AppDiv.CRVS.Application.Features.Fingerprint.commands.Create
{

    public class CreateFingerprint : IRequest<FingerPrintResponseDto>
    {
        public string clientKey { get; set; }
        public string registrationID
        {
            get; set;
        }
        public BiometricImages? images { get; set; }



        // Customer delete command handler with string response as output
        public class CreateFingerprintCommmandHandler : IRequestHandler<CreateFingerprint, FingerPrintResponseDto>
        {
            private readonly IRequestApiService _apiRequestService;
            public CreateFingerprintCommmandHandler(IRequestApiService apiRequestService)
            {
                _apiRequestService = apiRequestService;
            }

            public async Task<FingerPrintResponseDto> Handle(CreateFingerprint request, CancellationToken cancellationToken)
            {
                FingerPrintResponseDto ApiResponse;
                try
                {
                    var Create = new FingerPrintCreateRequest
                    {

                    };
                    var responseBody = await _apiRequestService.post("Register", request);
                    ApiResponse = JsonSerializer.Deserialize<FingerPrintResponseDto>(responseBody);
                    return ApiResponse;

                }
                catch (Exception exp)
                {
                    throw (new ApplicationException(exp.Message));
                }
            }
        }
    }
}
