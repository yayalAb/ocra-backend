

using System.Reflection;

namespace AppDiv.CRVS.Infrastructure.Services
{
    public static class HelperService

    {

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
