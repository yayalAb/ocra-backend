using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace AppDiv.CRVS.Application.Features.Report.Commads
{
    public class CreateReportCommad : IRequest<CreateReportCommadResponse>
    {
        public string ReportName { get; set; } = "";
        public string Query { get; set; } = "";
    }
}

