using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.User.Command.Create
{
    public record CreateUserCommand: IRequest<CreateUserCommandResponse>
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? UserImage { get; set; }
        public List<Guid> UserGroups { get; init; }
        // public string Password { get; set; }
        public AddPersonalInfoRequest PersonalInfo { get; set; }

    }
}
