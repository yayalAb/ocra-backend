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
            RuleFor(u => u.UserName)
            .NotNull()
            .NotEmpty()
            .MustAsync(BeUniqueUsername).WithMessage("userName already exists ")
            .Matches("^[a-zA-Z0-9-._@+]+$").WithMessage("invalid user name:\n user name cannot have spaces or special characters except -._@+");
            RuleFor(u => u.Email)
            .NotEmpty()
            .NotNull()
            .EmailAddress().WithMessage("invalid email address");
            // RuleFor(u => u.PersonalInfo.ContactInfo.Phone)
            //         .MustAsync(async (p,c) => await repo.CheckPhoneNumber(p))
            //         .WithMessage("Phone number already exists")
            //         .When(p => p.PersonalInfo.ContactInfo.Phone != null);
        }

        private async Task<bool> BeUniqueUsername(string username, CancellationToken cancellationToken)
        {
            var user = await _repo.GetUserByName(username);

            return user == null;
        }


    }
}