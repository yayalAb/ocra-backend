using System.Text;
using System.Text.Json;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Models;
using MediatR;

namespace AppDiv.CRVS.Application.Features.Fingerprint.commands
{

    public class VerifayCommands : IRequest<IdentifayFingerDto>
    {
        public string registrationID
        {
            get; set;
        }
        public BiometricImages? images { get; set; }



        // Customer delete command handler with string response as output
        public class VerifayCommandsCommmandHandler : IRequestHandler<VerifayCommands, IdentifayFingerDto>
        {
            private readonly IRequestApiService _apiRequestService;
            public VerifayCommandsCommmandHandler(IRequestApiService apiRequestService)
            {
                _apiRequestService = apiRequestService;
            }

            public async Task<IdentifayFingerDto> Handle(VerifayCommands request, CancellationToken cancellationToken)
            {
                try
                {
                    IdentifayFingerDto ApiResponse;
                    var Create = new FingerPrintApiRequestDto
                    {
                        registrationID = request.registrationID,
                        images = request.images

                    };
                    var responseBody = await _apiRequestService.post("Verify", Create);
                    ApiResponse = JsonSerializer.Deserialize<IdentifayFingerDto>(responseBody);
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
