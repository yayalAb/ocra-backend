using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Settings.Commands.create
{
    public class createSettingCommandValidator : AbstractValidator<createSettingCommand>
    {
        private readonly ISettingRepository _repo;
        public createSettingCommandValidator(ISettingRepository repo)
        {
            _repo = repo;
            // RuleFor(p => p.Setting.Key).MustAsync(async (k,c) => await repo.AnyAsync(s => s.Key == k))
            //         .WithMessage("Setting with the same key already exists.");
        }

    }
}