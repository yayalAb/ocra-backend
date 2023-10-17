using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class SharedReportDetailResponseDTO
    {
         public Guid Id { get; set; }
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