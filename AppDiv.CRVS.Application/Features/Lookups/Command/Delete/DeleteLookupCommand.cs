using AppDiv.CRVS.Application.Common;
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
        public Guid Id { get; set; }

        // public DeleteLookupCommand(Guid Id)
        // {
        //     this.Id = Id;
        // }
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
            try
            {
                // _Ilog.lo
                var LookupEntity = await _lookupRepository.GetByIdAsync(request.Id);

                await _lookupRepository.DeleteAsync(request.Id);
                await _lookupRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            var res = new BaseResponse
            {
                Success = true,
                Message = "Lookup information has been deleted!"
            };


            return res;
        }
    }
}