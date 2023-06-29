﻿using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;


namespace AppDiv.CRVS.Application.Features.User.Command.Create
{
    public record CreateUserCommand: IRequest<CreateUserCommandResponse>
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? UserImage { get; set; }
        public Guid AddressId { get; set; }
        public List<Guid> UserGroups { get; init; }
        public bool Status {get; set; }
        public string PreferedLanguage {get; set; }
        // public string Password { get; set; }
        public AddPersonalInfoRequest PersonalInfo { get; set; }

    }
}
