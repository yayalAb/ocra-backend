using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using MediatR;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Features.Report.Commads
{
    public class CreateReportCommad : IRequest<CreateReportCommadResponse>
    {
        public string ReportName { get; set; } = "";
        public string ReportTitle { get; set; } = "";
        public string Query { get; set; } = "";
        public string? Description { get; set; }
        public string[]? DefualtColumns { get; set; }
        public ReportColumsLngDto[] ColumnsLang { get; set; }
        public  JArray? UserGroups { get; set; }
        public  bool? isAddressBased { get; set; }=false;

    }
}

