using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Domain.Enums;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IReportRepostory
    {
        public Task<BaseResponse> CreateReportAsync(string reportName, string query, string Description, string[]? Colums, string? ReportTitle, CancellationToken cancellationToken);
        public Task<List<object>> GetReportData(string reportName, List<string>? columns = null, String? filters = "", List<Aggregate>? aggregates = null);
        public Task<JObject> GetReports();
        public Task<BaseResponse> UpdateReportQuery(string Viewname, string query);
        public Task<BaseResponse> DeleteReport(string Viewname);
        public string SanitizeString(string StringToSanitize);
         public  Task<IEnumerable<string>> GetReportColums(string viewName);
         public  Task<object> ReturnPerson(string Id);
         public  Task<object> ReturnAddress(string Id);
         public  Task<object> ReturnAddressIds(string Id);
    }
}