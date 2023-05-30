

using System.Reflection;
using AppDiv.CRVS.Application.Exceptions;
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

    

        public  JObject getPasswordPolicy()
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

    }
}
