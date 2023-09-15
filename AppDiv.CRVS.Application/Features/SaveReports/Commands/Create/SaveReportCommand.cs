using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using MediatR;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Features.SaveReports.Commands
{
    public class SaveReportCommand : IRequest<SaveReportCommandResponse>
    {
        public string? ReportName { get; set; }
        public string? Description { get; set; }
        public string? ReportTitle { get; set; }
        public List<Aggregate>? Agrgate { get; set; }
        public string? Filter { get; set; }
        public string[]? Colums { get; set; }
        public JObject? Other { get; set; }


    }
}