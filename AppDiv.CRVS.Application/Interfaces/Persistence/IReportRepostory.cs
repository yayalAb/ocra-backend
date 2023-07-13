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
        public string GetOperator(SqlOperator? sqlOper, object value, object value2);
        public Task<BaseResponse> CreateReportAsync(string reportName, string query);
        public Task<JObject> GetReportData(string reportName, List<string>? columns = null, List<Filter>? filters = null, List<Aggregate>? aggregates = null);
        public Task<JObject> GetReports();
    }
}