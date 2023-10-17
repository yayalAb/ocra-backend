using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using FluentValidation;

namespace AppDiv.CRVS.Application.Features.ShareReportApi.commands.create
{
    public class shareReportCommandValidetor : AbstractValidator<CreateReportApiCommands>
    {
        private readonly ISharedReportRepository _repo;
        public shareReportCommandValidetor(ISharedReportRepository repo)
        {

            _repo = repo;

        }



    }
}