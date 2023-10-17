using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using MediatR;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Features.ShareReportApi.commands.create
{
    public class CreateReportApiCommands : IRequest<object>
    {
        public string ReportName { get; set; }
        public string?  ReportTitle  { get; set; }   
        public string?  Agrgate  { get; set; }    
        public string?  Filter  { get; set; }    
        public string?  Colums  { get; set; }    
        public string?  Other  { get; set; }    
        public string  Username  { get; set; }  
        public string?  UserRole  { get; set; }    
        public string  Email  { get; set; }


    }
}
