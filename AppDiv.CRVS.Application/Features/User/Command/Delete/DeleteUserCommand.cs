using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces;
using MediatR;

namespace AppDiv.CRVS.Application.Features.User.Command.Delete
{

    public class DeleteUserCommand : IRequest<String>
    {
        public string Id { get; private set; }

        public DeleteUserCommand(string Id)
        {
            this.Id = Id;
        }
    }

    // Customer delete command handler with string response as output
    public class DeleteLookupCommandHandler : IRequestHandler<DeleteUserCommand, String>
    {
        private readonly IIdentityService _identityService;
        public DeleteLookupCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<string> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _identityService.DeleteUser(request.Id);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "User information has been deleted!";
        }
    }
}