using System.Data.Common;
using System.Data;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Enums;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using System.Text.Json;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain.Entities;
using System.Text.RegularExpressions;

namespace AppDiv.CRVS.Infrastructure.Persistence
{

    public class ReportRepostory : IReportRepostory
    {
        private readonly CRVSDbContext _DbContext;
        private readonly IReportStoreRepostory _reportStor;

        public ReportRepostory(CRVSDbContext dbContext, IReportStoreRepostory reportStor)
        {
            _DbContext = dbContext;
            _reportStor = reportStor;
        }
        public async Task<BaseResponse> CreateReportAsync(string reportName, string query, string Description, string[]? Colums, string? ReportTitle, CancellationToken cancellationToken)
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
                reportName = this.SanitizeString(reportName);
                query = RemoveSpecialChar(query);
                sql = $" CREATE VIEW `{reportName}` AS {query}";
                try
                {
                    string defualt = (Colums != null && Colums.Count() != 0) ? string.Join(",", Colums) : "";
                    var Data = await _DbContext.Database.ExecuteSqlRawAsync(sql);
                    var Report = new ReportStore
                    {
                        Id = Guid.NewGuid(),
                        ReportName = reportName,
                        ReportTitle = ReportTitle,
                        Description = Description,
                        DefualtColumns = defualt,
                        CreatedAt = DateTime.Now,
                        Query = query
                    };
                    await this.CreateReportTable(Report, cancellationToken);
                    response.Message = $"Created a report {reportName} successfully.";
                }
                catch (Exception ex)
                {
                    throw new BadRequestException(ex.Message);
                }
            }

            return response;
        }
        public async Task<BaseResponse> CreateReportTable(ReportStore report, CancellationToken cancellationToken)
        {
            await _reportStor.InsertAsync(report, cancellationToken);
            await _reportStor.SaveChangesAsync(cancellationToken);
            return new BaseResponse();
        }


        public async Task<BaseResponse> UpdateReportQuery(string Viewname, string query)
        {

            var sql = $"ALTER VIEW {Viewname} As {query}";
            var reader = await ConnectDatabase(sql);
            reader.Item2.Close();

            return new BaseResponse();
        }

        public async Task<BaseResponse> DeleteReport(string Viewname)
        {

            var sql = $"DROP VIEW {Viewname}";
            var reader = await ConnectDatabase(sql);
            reader.Item2.Close();

            return new BaseResponse();
        }



        public async Task<IEnumerable<string>> GetReportColums(string viewName)
        {
            var properties = new List<string>();
            var sql = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{viewName}'";
            var reader = await ConnectDatabase(sql); ;
            while (reader.Item1.Read())
            {
                var jObject = new JObject();
                string columnName = reader.Item1.GetString(0);
                jObject["PropertyName"] = columnName;
                properties.Add(columnName);
            }
            return properties;
        }


        public async Task<List<object>> GetReportData(string reportName, List<string>? columns = null, String filters = "", List<Aggregate>? aggregates = null)
        {
            var ReportInfo = _reportStor.GetAll().Where(x => x.ReportName == reportName).FirstOrDefault();
            if (ReportInfo == null)
            {
                throw new NotFoundException($"Report With Name {reportName} Does not Found");
            }
            var reportData = new JObject();
            string groupBySql = "";
            string aggregateSql = "";
            string SelectedColumns = columns?.Count() > 0 ? string.Join(",", columns) : string.IsNullOrEmpty(ReportInfo.DefualtColumns) ? "*" : ReportInfo.DefualtColumns;
            if (!string.IsNullOrEmpty(filters))
            {
                filters = $"WHERE {filters}";
            }
            if (aggregates?.Count() > 0)
            {
                (string Group, string Aggregate) response = ReturnAgrgateString(aggregates);
                groupBySql = response.Group;
                aggregateSql = response.Aggregate;
            }
            var sql = "";
            Console.WriteLine("Sql statment1 {0} {1}", aggregateSql, columns);
            if (!string.IsNullOrEmpty(aggregateSql) && aggregateSql.Length > 0)
            {
                Console.WriteLine("Sql statment2 {0} ", sql);
                reportName = this.SanitizeString(reportName);
                sql = $"SELECT {aggregateSql} FROM `{reportName}` {filters} {groupBySql}";
            }
            else
            {
                sql = $"SELECT {SelectedColumns} FROM `{reportName}` {filters} {groupBySql}";
            }
            Console.WriteLine("Sql statment {0} ", sql);

            var reader = await ConnectDatabase(sql);
            List<Dictionary<string, object>> resultList = new List<Dictionary<string, object>>();
            while (await reader.Item1.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.Item1.FieldCount; i++)
                {
                    row[reader.Item1.GetName(i)] = reader.Item1.GetValue(i);
                }
                resultList.Add(row);
            }
            reader.Item2.Close();

            var reportItems = new JArray();
            // var propertyNames = await GetReportColums(reportName);

            foreach (var dictionary in resultList)
            {
                var jsonDictionary = new Dictionary<string, JToken>();
                foreach (var kvp in dictionary)
                {
                    jsonDictionary[kvp.Key] = JToken.FromObject(kvp.Value);
                }
                reportItems.Add(JObject.FromObject(jsonDictionary));
            }

            List<object> list = reportItems.ToObject<List<object>>();
            reportData["reportData"] = reportItems;
            return list;
        }




        // The method to return available reports (we considered sql view as report)
        public async Task<JObject> GetReports()
        {

            var reportObjects = new JObject();
            var sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS";
            var viewReader = await ConnectDatabase(sql);//
            var objectNames = new JArray();
            while (await viewReader.Item1.ReadAsync())
            {
                var reportObject = new JObject();
                string reportName = viewReader.Item1.GetString(0);
                reportObject["ReportName"] = reportName;
                objectNames.Add(reportObject);
            }
            viewReader.Item1.Close();
            viewReader.Item2.Close();
            reportObjects["items"] = objectNames;

            return reportObjects;
        }

        public async Task<(DbDataReader, DbConnection)> ConnectDatabase(string sql)
        {
            var connectionString = _DbContext.Database.GetDbConnection();
            connectionString.Open();
            using var command = connectionString.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            return (await command.ExecuteReaderAsync(), connectionString);

        }
        public (string, string) ReturnAgrgateString(List<Aggregate>? aggregates)
        {
            string groupBySql = "";
            string aggregateSql = "";
            int x = 0;
            if (aggregates != null && aggregates.Count > 0)
            {
                Console.WriteLine("Agrgate test : {0}", aggregateSql);

                foreach (var aggregate in aggregates)
                {

                    switch (aggregate.AggregateMethod)
                    {
                        case SqlAggregate.GroupBy:
                            groupBySql += $" GROUP BY {aggregate.PropertyName}";
                            break;
                        case SqlAggregate.OrderBy:
                            groupBySql += $" ORDER BY {aggregate.PropertyName} ASC";
                            break;
                        case SqlAggregate.OrderByDesc:
                            groupBySql += $" ORDER BY {aggregate.PropertyName} DESC";
                            break;
                        case SqlAggregate.Count:
                            aggregateSql += $" COUNT({aggregate.PropertyName}) AS Count_{aggregate.PropertyName}";
                            break;
                        case SqlAggregate.Max:
                            aggregateSql += $" MAX({aggregate.PropertyName}) AS Max_{aggregate.PropertyName}";
                            break;
                        case SqlAggregate.Min:
                            aggregateSql += $" MIN({aggregate.PropertyName}) AS Min_{aggregate.PropertyName}";
                            break;
                        case SqlAggregate.Average:
                            aggregateSql += $" AVG({aggregate.PropertyName}) AS Average_{aggregate.PropertyName}";
                            break;
                        case SqlAggregate.Sum:
                            aggregateSql += $" SUM({aggregate.PropertyName}) AS Sum_Of_{aggregate.PropertyName}";
                            break;
                        default:
                            aggregateSql += $" COUNT({aggregate.PropertyName}) AS Count_{aggregate.PropertyName}";
                            break;
                    }
                    x++;
                    if ((aggregates.Count() > x) && !string.IsNullOrEmpty(aggregateSql))
                    {
                        aggregateSql += ",";
                    }
                }
            }

            return (groupBySql, aggregateSql);

        }

        public string SanitizeString(string StringToSanitize)
        {
            string output = "";
            if (string.IsNullOrEmpty(StringToSanitize))
            {
                output = Regex.Replace(StringToSanitize, "[^a-zA-Z0-9_]", "");

            }


            return output;
        }
        public string RemoveSpecialChar(string StringToSanitize)
        {
            string output = "";
            if (string.IsNullOrEmpty(StringToSanitize))
            {
                output = Regex.Replace(StringToSanitize, "[;--]", "");

            }


            return output;
        }

   public async Task<object> ReturnPerson(string Id)
        {
            var sql = $"CALL Person_procedure ('{Id}')";
            Console.WriteLine("Person_procedure : {0}",sql);
            var result = new List<object>();
            var viewReader = await ConnectDatabase(sql);
            while (viewReader.Item1.Read())
            {
                result.Add(new
                {
                    FirstNameAm = viewReader.Item1["FirstNameAm"],
                    FirstNameOr=viewReader.Item1["FirstNameOr"],
                    MiddleNameAm =viewReader.Item1["MiddleNameAm"],
                    MiddleNameOr=viewReader.Item1["MiddleNameOr"],
                    LastNameAm=viewReader.Item1["LastNameAm"],
                    LastNameOr=viewReader.Item1["LastNameOr"],
                    BirthMonthOr=viewReader.Item1["BirthMonthOr"],
                    BirthMonthAm=viewReader.Item1["BirthMonthAm"],
                    BirthDay=viewReader.Item1["BirthDay"],
                    BirthYear=viewReader.Item1["BirthYear"],
                    GenderAm=viewReader.Item1["GenderAm"],
                    GenderOr=viewReader.Item1["GenderOr"],
                    NationalId=viewReader.Item1["NationalId"],
                    NationalityOr=viewReader.Item1["NationalityOr"],
                    NationalityAm=viewReader.Item1["NationalityAm"],
                    MarriageStatusOr=viewReader.Item1["MarriageStatusOr"],
                    MarriageStatusAm=viewReader.Item1["MarriageStatusAm"],
                    ReligionOr=viewReader.Item1["ReligionOr"],
                    ReligionAm=viewReader.Item1["ReligionAm"],
                    NationOr=viewReader.Item1["NationOr"],
                    NationAm=viewReader.Item1["NationAm"],
                    EducationalStatusOr=viewReader.Item1["EducationalStatusOr"],
                    EducationalStatusAm=viewReader.Item1["EducationalStatusAm"],
                    TypeOfWorkOr=viewReader.Item1["TypeOfWorkOr"],
                    TypeOfWorkAm=viewReader.Item1["TypeOfWorkAm"]
                });
            }
            await viewReader.Item2.CloseAsync();


            return result;
        }

public async Task<object> ReturnAddress(string Id)
        {
            var sql = $"CALL Address_procedure ('{Id}')";
            var result = new List<object>();
            var viewReader = await ConnectDatabase(sql);
            while (viewReader.Item1.Read())
            {
                result.Add(new
                {
                        CountryOr= viewReader.Item1["CountryOr"],
                        CountryAm= viewReader.Item1["CountryAm"],
                        RegionOr= viewReader.Item1["RegionOr"],
                        RegionAm= viewReader.Item1["RegionAm"],
                        ZoneOr= viewReader.Item1["ZoneOr"],
                        ZoneAm= viewReader.Item1["ZoneAm"],
                        WoredaOr= viewReader.Item1["WoredaOr"],
                        WoredaAm = viewReader.Item1["WoredaAm"],
                        KebeleOr= viewReader.Item1["KebeleOr"],
                        KebeleAm= viewReader.Item1["KebeleAm"]
                });
            }
            await viewReader.Item2.CloseAsync();


            return result;
        }

        



    }
}
