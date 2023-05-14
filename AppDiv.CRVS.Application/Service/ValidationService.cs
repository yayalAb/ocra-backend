using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation;

namespace AppDiv.CRVS.Application.Service
{
    public static class ValidationService
    {
        // public static bool IsGuidNullOrEmpty(Guid? id)
        // {
        //     return !id.HasValue && id.Equals(Guid.Empty);
        // }
        public static IRuleBuilderOptions<T, string> NotStartWithWhiteSpaces<T>(this IRuleBuilder<T, string> ruleBuilder, string type)
        {

            return ruleBuilder.Must(m => m != null && !m.StartsWith(" ")).WithMessage("'{PropertyName}' should not start with whitespace");
        }
        public static IRuleBuilderOptions<T, object> IsGuidNullOrEmpty<T>(this IRuleBuilder<T, object> ruleBuilder)
        {

            return ruleBuilder.Must(m => !m.Equals(Guid.Empty)).WithMessage("'{PropertyName}' is requered");
        }

        public static IRuleBuilderOptions<T, Guid> NotGuidEmpty<T>(this IRuleBuilder<T, Guid> ruleBuilder)
        {
            return ruleBuilder.Must(m => !m.Equals(Guid.Empty)).WithMessage("'{PropertyName}' is requered");
        }
        public static Expression<Func<T, object>> GetNestedProperty<T>(string propertyPath)
        {
            var param = Expression.Parameter(typeof(T), "x");
            Expression body = param;
            foreach (var member in propertyPath.Split('.'))
            {
                body = Expression.PropertyOrField(body, member);


            }
            if (Nullable.GetUnderlyingType(body.Type) != null)
            {
                // If the type is a nullable value type, convert it to its underlying type before converting to object
                body = Expression.Convert(body, Nullable.GetUnderlyingType(body.Type));
            }

            // Convert the result to object
            body = Expression.Convert(body, typeof(object));
            return Expression.Lambda<Func<T, object>>(body, param);
        }
    }
}