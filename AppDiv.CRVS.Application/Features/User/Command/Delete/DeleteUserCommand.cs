using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces;
using MediatR;

namespace AppDiv.CRVS.Application.Features.User.Command.Delete
{

    public class DeleteUserCommand : IRequest<BaseResponse>
    {
        public string[] Id { get;  set; }

        // public DeleteUserCommand(string[] Id)
        // {
        //     this.Id = Id;
        // }
    }

    // Customer delete command handler with string response as output
    public class DeleteLookupCommandHandler : IRequestHandler<DeleteUserCommand, BaseResponse>
    {
        private readonly IIdentityService _identityService;
        public DeleteLookupCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<BaseResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var deleteUserCommandResponse = new BaseResponse();
            try
            { foreach (var item in request.Id)
            {
                await _identityService.DeleteUser(item);
            }
                
                deleteUserCommandResponse.Message = "User information has been deleted!";
                deleteUserCommandResponse.Status = 200;
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return deleteUserCommandResponse;
        }
    }
}