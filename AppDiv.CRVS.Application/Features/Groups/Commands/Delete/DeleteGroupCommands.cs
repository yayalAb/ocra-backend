using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Groups.Commands.Delete
{
    // Customer create command with UserGroup response
    public class DeleteGroupCommands : IRequest<BaseResponse>
    {
        public Guid[] Id { get; set; }

    }

    // Customer delete command handler with UserGroup response as output
    public class DeleteGroupCommandsHandler : IRequestHandler<DeleteGroupCommands, BaseResponse>
    {
        private readonly IGroupRepository _groupRepository;
        public DeleteGroupCommandsHandler(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<BaseResponse> Handle(DeleteGroupCommands request, CancellationToken cancellationToken)
        {
            var res = new BaseResponse();
            try
            {
                var groupEntity = await _groupRepository.GetAsync(request.Id);
                foreach (var item in request.Id)
                {
                    await _groupRepository.DeleteAsync(request.Id);
                }
                
                await _groupRepository.SaveChangesAsync(cancellationToken);
                res.Deleted("Group");
            }
            catch (Exception exp)
            {
                res.BadRequest("Unable to delete the specified group.");
                throw (new ApplicationException(exp.Message));
            }
            return res;
        }
    }
}
