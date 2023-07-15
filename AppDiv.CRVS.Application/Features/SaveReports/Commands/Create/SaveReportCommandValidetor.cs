using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;

namespace AppDiv.CRVS.Application.Features.SaveReports.Commands
{
    public class SaveReportCommandValidetor : AbstractValidator<SaveReportCommand>
    {
        private readonly IMyReportRepository _repo;
        public SaveReportCommandValidetor(IMyReportRepository repo)
        {

            _repo = repo;

        }



    }
}