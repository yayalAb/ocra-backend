

using System.Reflection;
using AppDiv.CRVS.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Infrastructure.Services
{
    public class HelperService

    {
        private readonly CRVSDbContext _context;

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

    }
}
