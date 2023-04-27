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
    public class DeleteGroupCommands : IRequest<String>
    {
        public Guid Id { get; set; }

    }

    // Customer delete command handler with UserGroup response as output
    public class DeleteGroupCommandsHandler : IRequestHandler<DeleteGroupCommands, String>
    {
        private readonly IGroupRepository _groupRepository;
        public DeleteGroupCommandsHandler(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<String> Handle(DeleteGroupCommands request, CancellationToken cancellationToken)
        {
            try
            {
                var groupEntity = await _groupRepository.GetByIdAsync(request.Id);

                await _groupRepository.DeleteAsync(request.Id);
                await _groupRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Lookup information has been deleted!";
        }
    }
}
