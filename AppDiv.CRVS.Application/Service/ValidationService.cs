using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Base;
using AppDiv.CRVS.Domain.Entities;
using FluentValidation;

namespace AppDiv.CRVS.Application.Service
{
    public static class ValidationService
    {
        // public static bool IsGuidNullOrEmpty(Guid? id)
        // {
        //     return !id.HasValue && id.Equals(Guid.Empty);
        // }
        // private static IBaseRepository<BaseAuditableEntity> repository;
        public static IRuleBuilderOptions<T, string?> ForeignKeyWithLookup<T>(this IRuleBuilder<T, string?> ruleBuilder, ILookupRepository repo)
        {
            // repository = repo;
            return ruleBuilder.MustAsync(async (lookup, c)
                        =>
            {
                var look = await repo.GetAsync(lookup);
                return look == null ? false : true;
            }).WithMessage("'{PropertyName}' Unable to Get The Lookup");
        }
        // repo.CheckForeignKey(e => e.InformantTypeLookup.Id.ToString() == lookup, p => p.InformantTypeLookup)

        public static IRuleBuilderOptions<T, string?> ForeignKeyWithAddress<T>(this IRuleBuilder<T, string?> ruleBuilder, IAddressLookupRepository repo)
        {
            // repository = repo;
            return ruleBuilder.MustAsync(async (ad, c)
                        =>
            {
                var address = await repo.GetAsync(ad);
                return address == null ? false : true;
            }).WithMessage("'{PropertyName}' Unable to Get The Address");
        }

        public static IRuleBuilderOptions<T, string?> ForeignKeyWithPerson<T>(this IRuleBuilder<T, string?> ruleBuilder, IPersonalInfoRepository repo)
        {
            // repository = repo;
            return ruleBuilder.MustAsync(async (pr, c)
                            =>
                            {
                                var person = await repo.GetAsync(pr);
                                return person == null ? false : true;
                            }).WithMessage("'{PropertyName}' Unable to Get The Person");
        }

        public static IRuleBuilderOptions<T, string?> NotGuidEmpty<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder.Must(m => !string.IsNullOrEmpty(m) && !m.Equals(Guid.Empty)).WithMessage("'{PropertyName}' is requered");
        }



        public static IRuleBuilderOptions<T, string> NotStartWithWhiteSpaces<T>(this IRuleBuilder<T, string> ruleBuilder, string type)
        {

            return ruleBuilder.Must(m => m != null && !m.StartsWith(" ")).WithMessage("'{PropertyName}' should not start with whitespace");
        }
        public static IRuleBuilderOptions<T, object> IsGuidNullOrEmpty<T>(this IRuleBuilder<T, object> ruleBuilder)
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