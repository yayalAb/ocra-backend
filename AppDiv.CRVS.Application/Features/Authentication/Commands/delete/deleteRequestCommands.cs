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

namespace AppDiv.CRVS.Application.Features.Authentication.Commands.delete
{
    // Customer create command with BaseResponse response
    public class deleteRequestCommands : IRequest<BaseResponse>
    {
        public Guid[] Ids { get; set; }

    }

    // Customer delete command handler with BaseResponse response as output
    public class deleteRequestCommandsHandler : IRequestHandler<deleteRequestCommands, BaseResponse>
    {
        private readonly IRequestRepostory _requestRepostory;
        public deleteRequestCommandsHandler(IRequestRepostory requestRepostory)
        {
            _requestRepostory = requestRepostory;
        }

        public async Task<BaseResponse> Handle(deleteRequestCommands request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse();
            try
            {  
                foreach(Guid id in request.Ids){
                var SelectedRequest=await _requestRepostory.GetAsync(id);
                SelectedRequest.isDeleted=true;
                await _requestRepostory.UpdateAsync(SelectedRequest, x=>x.Id);
            }  
                await _requestRepostory.SaveChangesAsync(cancellationToken);
                response.Deleted("Request");

            }
            catch (Exception exp)
            {
                response.BadRequest("Unable to delete the specified  Request.");
            }
            return response;
        }
    }
}


