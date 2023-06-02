using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;

namespace AppDiv.CRVS.Application.Features.CorrectionRequests.Commands
{

    public record CreateCorrectionRequest(AddCorrectionRequest CorrectionRequest) : IRequest<CreateCorrectionRequestResponse>
    {

    }
}