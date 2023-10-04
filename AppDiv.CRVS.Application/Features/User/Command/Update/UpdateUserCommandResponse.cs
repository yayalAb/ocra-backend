
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Application.Features.User.Command.Update

{
    public class UpdateUserCommandResponse : BaseResponse
    {
        public UserResponseDTO? UpdatedUser {get;set;}
        public UpdateUserCommandResponse() : base()
        {

        }


    }
}