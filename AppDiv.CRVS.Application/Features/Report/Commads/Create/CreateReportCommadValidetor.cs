using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;

namespace AppDiv.CRVS.Application.Features.Report.Commads
{
    public class CreateReportCommadValidetor : AbstractValidator<CreateReportCommad>
    {
        private readonly IReportRepostory _repo;
        public CreateReportCommadValidetor(IReportRepostory repo)
        {

            _repo = repo;

        }


    }
}