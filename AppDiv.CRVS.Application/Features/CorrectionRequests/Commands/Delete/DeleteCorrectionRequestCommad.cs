using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        public Guid[] Ids { get; set; }

    }

    // Customer delete command handler with BaseResponse response as output
    public class DeleteCorrectionRequestCommadHandler : IRequestHandler<DeleteCorrectionRequestCommad, BaseResponse>
    {
        private readonly ICorrectionRequestRepostory _correctionRequestRepository;
        private readonly IRequestRepostory _requestRepostory;
        public DeleteCorrectionRequestCommadHandler(ICorrectionRequestRepostory correctionRequestRepository, IRequestRepostory requestRepostory)
        {
            _correctionRequestRepository = correctionRequestRepository;
            _requestRepostory = requestRepostory;
        }

        public async Task<BaseResponse> Handle(DeleteCorrectionRequestCommad request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse();
            try
            {   foreach(Guid id in request.Ids){
                var correctionRequest= _correctionRequestRepository.GetAll()
                .Include(x=>x.Request).Where(x=>x.Id==id).FirstOrDefault();
                if(correctionRequest.Request.currentStep!=0){   
                    throw  new NotFoundException("You Can not delete this Request It Is Approved");
                }
                await _requestRepostory.DeleteAsync(correctionRequest.RequestId);
                await _correctionRequestRepository.DeleteAsync(id);
            }  
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
