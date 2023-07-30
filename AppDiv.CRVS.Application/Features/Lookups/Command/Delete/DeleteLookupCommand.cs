using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Lookups.Command.Delete
{
    // Customer create command with BaseResponse response
    public class DeleteLookupCommand : IRequest<BaseResponse>
    {
        public Guid[] Ids { get; set; }

    }

    // Customer delete command handler with BaseResponse response as output
    public class DeleteLookupCommandHandler : IRequestHandler<DeleteLookupCommand, BaseResponse>
    {
        private readonly ILookupRepository _lookupRepository;
        public DeleteLookupCommandHandler(ILookupRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;
        }

        public async Task<BaseResponse> Handle(DeleteLookupCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse();
            try
            {
                IEnumerable<Guid> ids = request.Ids;
                foreach (Guid x in ids)
                {
                    await _lookupRepository.DeleteAsync(x); ;
                }
                await _lookupRepository.SaveChangesAsync(cancellationToken);

                response.Deleted("Lookup");
            }
            catch (Exception e)
            {
                response.BadRequest("Cannot delete the specified lookup because it is being used.");
            }

            return response;
        }
    }
}