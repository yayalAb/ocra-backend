using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Plans.Command.Create
{
    public class CreatePlanCommandValidator : AbstractValidator<CreatePlanCommand>
    {
        private readonly IPlanRepository _repo;
        // private readonly IMediator _mediator;
        public CreatePlanCommandValidator(IPlanRepository repo)
        {

            _repo = repo;
        }


    }
}