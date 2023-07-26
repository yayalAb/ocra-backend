

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

    public class IdentifayCommand : IRequest<IdentifayFingerDto>
    {
        public BiometricImages? images { get; set; }

        // Customer delete command handler with string response as output
        public class IdentifayCommandCommmandHandler : IRequestHandler<IdentifayCommand, IdentifayFingerDto>
        {
            private readonly IRequestApiService _apiRequestService;
            public IdentifayCommandCommmandHandler(IRequestApiService apiRequestService)
            {
                _apiRequestService = apiRequestService;
            }

            public async Task<IdentifayFingerDto> Handle(IdentifayCommand request, CancellationToken cancellationToken)
            {
                IdentifayFingerDto ApiResponse;
                try
                {
                    var Create = new FingerPrintApiRequestDto
                    {
                        registrationID = null,
                        images = request.images

                    };
                    var responseBody = await _apiRequestService.post("Identify", Create);
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
