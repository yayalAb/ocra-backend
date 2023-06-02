using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;

namespace AppDiv.CRVS.Application.Features.CorrectionRequests.Commands
{
    public class CreateCorrectionRequestValidetor : AbstractValidator<CreateCorrectionRequest>
    {
        private readonly ICorrectionRequestRepostory _repo;
        public CreateCorrectionRequestValidetor(ICorrectionRequestRepostory repo)
        {
            _repo = repo;
        }
    }
}