using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.User.Command.Create
{
    public record CreateUserCommand(AddUserRequest User) : IRequest<CreateUserCommandResponse>
    {

    }
}
