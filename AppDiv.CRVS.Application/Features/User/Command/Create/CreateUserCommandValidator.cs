using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain.Repositories;
using FluentValidation;

namespace AppDiv.CRVS.Application.Features.User.Command.Create
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private readonly IIdentityService _repo;
        public CreateUserCommandValidator(IIdentityService repo)
        {
            this._repo = repo;

        }
    }
}