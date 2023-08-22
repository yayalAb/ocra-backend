

using System.Data;
using System.Data.Common;
using System.Reflection;
using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain.Configuration;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Infrastructure.Service;
using AppDiv.CRVS.Infrastructure.Service.FireAndForgetJobs;
// using AppDiv.CRVS.Infrastructure.Service.FireAndForgetJobs;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Nest;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Infrastructure.Services
{
    public class HelperService

    {
        private readonly CRVSDbContext _context;
        public static bool HasCamera = false;
        public static bool HasVideo = false;

        public HelperService(CRVSDbContext context)
        {
            _context = context;
        }



        public JObject getPasswordPolicy()
        {

            var policy = _context.Settings.Where(s => s.Key.ToLower() == "passwordpolicy")
                    .Select(s => s.Value).FirstOrDefault();
            if (policy == null)
            {
                throw new NotFoundException("password policy setting not found");
            }
            return policy;
        }

        public static T UpdateObjectFeilds<T>(T personalInfo, Dictionary<string, object> fieldValues)
        {
            foreach (var fieldValue in fieldValues)
            {

                string propertyName = fieldValue.Key;
                object newValue = fieldValue.Value;

                // Use reflection to get the PropertyInfo for the property
                PropertyInfo propertyInfo = typeof(T).GetProperty(propertyName);

                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    // Update the property value dynamically
                    propertyInfo.SetValue(personalInfo, newValue);

                }
                else
                {
                    throw new Exception($"invalid feildname '{propertyName}'");
                }
            }
            return personalInfo;
        }
        public static int CalculateAge(DateTime birthDate)
        {
            int age = DateTime.Now.Year - birthDate.Year;

            // Check if the current date is before the birth date in the current year
            if (DateTime.Now.Month < birthDate.Month || (DateTime.Now.Month == birthDate.Month && DateTime.Now.Day < birthDate.Day))
            {
                age--;
            }
            return age;
        }
        public static string getCurrentLanguage()
        {
            var lang = "or";
            var httpContext = new HttpContextAccessor().HttpContext;
            if (httpContext != null && httpContext.Request.Headers.ContainsKey("lang"))
            {
                httpContext.Request.Headers.TryGetValue("lang", out StringValues headerValue);
                lang = headerValue.FirstOrDefault();
            }

            return lang;
        }
        public static bool IsBase64String(string input)
        {
            try
            {
                var base64String = input.Substring(input.IndexOf(',') + 1);
                // Attempt to convert the input string to a byte array
                byte[] buffer = Convert.FromBase64String(base64String);
                return true;
            }
            catch (FormatException)
            {
                // If the conversion fails, catch the FormatException and return false
                return false;
            }
        }

        public static async Task<PaymentExamption?> UpdatePaymentExamption(Event updatedEvent, CRVSDbContext dbContext, IEventPaymentRequestService _paymentRequestService, CancellationToken cancellationToken)
        {

            if (updatedEvent.PaymentExamption != null && (updatedEvent.PaymentExamption.Id == null || updatedEvent.PaymentExamption.Id == Guid.Empty))
            {
                var examptionExists = await dbContext.PaymentExamptions.Where(p => p.EventId == updatedEvent.Id).AnyAsync();
                if (!examptionExists)//-- examption status changed form false to true
                {
                    var paymentExamption = updatedEvent.PaymentExamption;
                    paymentExamption.EventId = updatedEvent.Id;
                    await dbContext.PaymentExamptions.AddAsync(paymentExamption);
                    //remove payment request
                    var paymentRequest = await dbContext.PaymentRequests
                        .Where(pr => pr.EventId == updatedEvent.Id
                        && EF.Functions.Like(pr.PaymentRate.PaymentTypeLookup.ValueStr.ToLower(), $"%certificategeneration%"))
                        .FirstOrDefaultAsync();
                    if (paymentRequest != null)
                    {
                        dbContext.PaymentRequests.Remove(paymentRequest);
                    }

                }
                updatedEvent.PaymentExamption = null;
            }
            else
            {
                if (!updatedEvent.IsExampted && updatedEvent.PaymentExamption == null)
                {
                    var oldExamption = await dbContext.PaymentExamptions.Where(pe => pe.EventId == updatedEvent.Id).FirstOrDefaultAsync();
                    if (oldExamption != null)//-- examption status changed from true to false on update
                    {
                        var examptionDocIds = await dbContext.SupportingDocuments
                                .Where(sd => sd.PaymentExamptionId == oldExamption.Id)
                                .Select(sd => sd.Id).ToListAsync();
                        //TODO: remove supporting doc files from server
                        dbContext.PaymentExamptions.Remove(oldExamption);

                        // create payment request
                        var response = await _paymentRequestService.CreatePaymentRequest(updatedEvent.EventType, updatedEvent, "CertificateGeneration", null, HasCamera, HasVideo, cancellationToken);

                    }
                }
            }
            return updatedEvent.PaymentExamption;
        }

        public static async Task<(DbDataReader, DbConnection)> ConnectDatabase(string sql, CRVSDbContext _DbContext)
        {
            var connectionString = _DbContext.Database.GetDbConnection();
            connectionString.Open();
            using var command = connectionString.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            return (await command.ExecuteReaderAsync(), connectionString);

        }

    }
}
