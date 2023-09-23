using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Ranges.Command.Create
{
    public class CreateRangeCommandValidator : AbstractValidator<CreateRangeCommand>
    {
        private readonly IRangeRepository _repo;
        // private readonly IMediator _mediator;
        public CreateRangeCommandValidator(IRangeRepository repo)
        {

            _repo = repo;
        }


    }
}