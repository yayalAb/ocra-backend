using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Repositories;
using FluentValidation;

namespace AppDiv.CRVS.Application.Features.User.Command.Create
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private readonly IUserRepository _repo;
        public CreateUserCommandValidator(IUserRepository repo)
        {
            this._repo = repo;

        }
    }
}