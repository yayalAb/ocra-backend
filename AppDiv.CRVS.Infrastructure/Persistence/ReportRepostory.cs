using System.Data;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Enums;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Infrastructure.Persistence
{

    public class ReportRepostory : IReportRepostory
    {
        private readonly CRVSDbContext _DbContext;
        public ReportRepostory(CRVSDbContext dbContext)
        {
            _DbContext = dbContext;
        }
        // SqlOperator? sqlOper
        public string GetOperator(SqlOperator? sqlOper, object value, object value2)
        {
            string sqlOperator = "";
            if (sqlOper == null)
            {
                throw new NotFoundException("Operator must not be null");
            }
            switch (sqlOper)
            {
                case SqlOperator.Contain:
                    sqlOperator = $"LIKE '%{value}%'";
                    break;

                case SqlOperator.EqualTo:
                    if (value == null || string.IsNullOrEmpty(value.ToString())) sqlOperator = "IS NULL";
                    else sqlOperator = $"= '{value}'";
                    break;

                case SqlOperator.GreaterThan:
                    sqlOperator = $">{value}";
                    break;
                case SqlOperator.LessThan:
                    sqlOperator = $"<{value}";
                    break;
                case SqlOperator.Between:
                    sqlOperator = $"BETWEEN{value}AND{value2}";
                    break;
                case SqlOperator.NotEqual:
                    if (value == null || string.IsNullOrEmpty(value.ToString())) sqlOperator = "IS NOT NULL";
                    else sqlOperator = $"<>{value}";
                    break;
                case SqlOperator.GreaterThanOrEqual:
                    sqlOperator = $">={value}";
                    break;
                case SqlOperator.LessThanOrEqual:
                    sqlOperator = $"<={value}";
                    break;
                case SqlOperator.NotContain:
                    sqlOperator = $"NOT LIKE '%{value}%'";
                    break;
                case SqlOperator.NotBetween:
                    sqlOperator = $"NOT BETWEEN{value}AND{value2}";
                    break;
                default:
                    break;
            }

            return sqlOperator;
        }

        public async Task<IEnumerable<JObject>> GetObjectProperties(string viewName)
        {
            var properties = new List<JObject>();

            var sql = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{viewName}'";

            var connection = _DbContext.Database.GetDbConnection();
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            using var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                var jObject = new JObject();
                string columnName = reader.GetString(0);
                jObject["PropertyName"] = columnName;
                properties.Add(jObject);
            }
            return properties;
        }
        // The method that returns the report data which is query result.  And also the report columns(column names)
        public async Task<JObject> GetReportData(string reportName, List<string>? columns = null, List<Filter>? filters = null, List<Aggregate>? aggregates = null)
        {
            var reportData = new JObject();
            string filterSql = "";
            string selectColumn = "*";
            string selectColumnList = "";

            if (columns != null && columns.Count > 0)
            {
                foreach (var column in columns)
                {
                    selectColumnList += $"{column}, ";
                }

                selectColumn = selectColumnList.Remove(selectColumnList.Length - 2);
            }

            if (filters != null && filters.Count > 0)
            {
                filters.Sort();
                var lastFilter = filters.Last();
                foreach (var filter in filters)
                {
                    var conditionStr = "";
                    conditionStr = $"{filter.PropertyName} {GetOperator(filter.Operator, filter.Value, filter.Value2)}";
                    if (filter != lastFilter)
                    {
                        conditionStr += " AND ";
                    }
                    filterSql += conditionStr;
                }
                filterSql = " WHERE " + filterSql;
            }

            string groupBySql = "";
            string aggregateSql = "";
            if (aggregates != null && aggregates.Count > 0)
            {
                aggregates.Sort();
                var lastAggregate = aggregates.Last();

                foreach (var aggregate in aggregates)
                {
                    var propertyName = aggregate.PropertyName;

                    var aggregateMethod = aggregate.AggregateMethod ?? SqlAggregate.Count;
                    switch (aggregateMethod)
                    {
                        case SqlAggregate.GroupBy:
                            groupBySql += $"GROUP BY {propertyName}, ";
                            break;
                        case SqlAggregate.OrderBy:
                            groupBySql += $"ORDER BY {propertyName} ASC, ";
                            break;
                        case SqlAggregate.Count:
                            aggregateSql += $", COUNT({propertyName}) AS Count_{propertyName}";
                            break;
                        case SqlAggregate.Max:
                            aggregateSql += $", MAX({propertyName}) AS Max_{propertyName}";
                            break;
                        case SqlAggregate.Min:
                            aggregateSql += $", MIN({propertyName}) AS Min_{propertyName}";
                            break;
                        case SqlAggregate.Average:
                            aggregateSql += $", AVG({propertyName}) AS Average_{propertyName}";
                            break;
                    }
                }

                // Remove the trailing comma and space from the group by clause
                if (!string.IsNullOrEmpty(groupBySql))
                {
                    groupBySql = groupBySql.Remove(groupBySql.Length - 2);
                }
            }


            var connection = _DbContext.Database.GetDbConnection();
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            if (!string.IsNullOrEmpty(reportName))
            {
                var sql = $"SELECT {selectColumn} {aggregateSql} FROM `{reportName}` {filterSql} {groupBySql}";
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
            }

            using var reader = await command.ExecuteReaderAsync();

            List<Dictionary<string, object>> resultList = new List<Dictionary<string, object>>();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.GetValue(i);
                }
                resultList.Add(row);
            }
            connection.Close();

            var reportItems = new JArray();
            var properties = new JArray();
            var propertyNames = await GetObjectProperties(reportName);

            foreach (var dictionary in resultList)
            {
                var jsonDictionary = new Dictionary<string, JToken>();
                foreach (var kvp in dictionary)
                {
                    jsonDictionary[kvp.Key] = JToken.FromObject(kvp.Value);
                }
                reportItems.Add(JObject.FromObject(jsonDictionary));
            }

            if (propertyNames.Count() > 0 || propertyNames != null)
            {
                foreach (var prop in propertyNames)
                {
                    properties.Add(prop);
                }
            }
            reportData["reportData"] = reportItems;
            reportData["reportProperties"] = properties;
            return reportData;
        }
        public async Task<BaseResponse> CreateReportAsync(string reportName, string query)
        {
            var response = new BaseResponse();
            string sql = "";

            if (string.IsNullOrEmpty(reportName))
            {
                response.Message = "The report name should not be empty";
                return response;
            }
            else if (string.IsNullOrEmpty(query))
            {
                response.Message = "You should provide the report query statement.";
                return response;
            }
            else
            {
                sql = $" CREATE VIEW `{reportName}` AS {query}";
                try
                {
                    var Data = await _DbContext.Database.ExecuteSqlRawAsync(sql);
                    response.Message = $"Created a report {reportName} successfully.";
                }
                catch (Exception ex)
                {
                    // response.Errors?.Add(ex.Message);
                    throw new BadRequestException(ex.Message);
                }
            }

            return response;
        }

        // The method to return available reports (we considered sql view as report)
        public async Task<JObject> GetReports()
        {

            var reportObjects = new JObject();

            var sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS";
            var Data = await _DbContext.Database.ExecuteSqlRawAsync(sql);

            var connectionString = _DbContext.Database.GetDbConnection();
            connectionString.Open();
            using var command = connectionString.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            var viewReader = await command.ExecuteReaderAsync();
            var objectNames = new JArray();
            while (await viewReader.ReadAsync())
            {
                var reportObject = new JObject();
                string reportName = viewReader.GetString(0);

                reportObject["ReportName"] = reportName;
                objectNames.Add(reportObject);
            }
            viewReader.Close();

            connectionString.Close();
            reportObjects["items"] = objectNames;

            return reportObjects;
        }
    }
}