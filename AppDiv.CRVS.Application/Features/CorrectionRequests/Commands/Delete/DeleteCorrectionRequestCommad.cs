using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.CorrectionRequests.Commands.Delete
{
    // Customer create command with BaseResponse response
    public class DeleteCorrectionRequestCommad : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }

    }

    // Customer delete command handler with BaseResponse response as output
    public class DeleteCorrectionRequestCommadHandler : IRequestHandler<DeleteCorrectionRequestCommad, BaseResponse>
    {
        private readonly ICorrectionRequestRepostory _correctionRequestRepository;
        public DeleteCorrectionRequestCommadHandler(ICorrectionRequestRepostory correctionRequestRepository)
        {
            _correctionRequestRepository = correctionRequestRepository;
        }

        public async Task<BaseResponse> Handle(DeleteCorrectionRequestCommad request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse();
            try
            {
                await _correctionRequestRepository.DeleteAsync(request.Id);
                await _correctionRequestRepository.SaveChangesAsync(cancellationToken);

                response.Deleted("Correctoon Request");

            }
            catch (Exception exp)
            {
                response.BadRequest("Unable to delete the specified correction Request.");
            }
            return response;
        }
    }
}
