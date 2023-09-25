using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Ranges.Command.Delete
{
    // Customer create command with string response
    public class DeleteRangeCommand : IRequest<BaseResponse>
    {
        public Guid[] Ids { get; set; }


    }

    // Customer delete command handler with string response as output
    public class DeleteRangeCommmandHandler : IRequestHandler<DeleteRangeCommand, BaseResponse>
    {
        private readonly IRangeRepository _rangeRepository;
        public DeleteRangeCommmandHandler(IRangeRepository rangeRepository)
        {
            _rangeRepository = rangeRepository;
        }

        public async Task<BaseResponse> Handle(DeleteRangeCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse();
            try
            {
                if (request.Ids != null && request.Ids.Length > 0)
                {
                    foreach (var item in request.Ids)
                    {
                        await _rangeRepository.DeleteAsync(item);

                    }
                    response.Deleted("range ");

                    await _rangeRepository.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    response.BadRequest("There is no range with the specified id");
                }


            }
            catch (Exception exp)
            {
                response.BadRequest("Unable to delete the range.");
            }
            return response;
        }
    }
}