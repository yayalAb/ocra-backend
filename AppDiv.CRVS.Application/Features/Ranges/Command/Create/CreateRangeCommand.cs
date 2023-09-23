﻿using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Ranges.Command.Create
{
    // Customer create command with CustomerResponse
    public record CreateRangeCommand(AddRangeRequest Range) : IRequest<CreateRangeCommandResponse>
    {

    }
}