using System.Runtime.CompilerServices;
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
using EthiopianCalendar;

namespace AppDiv.CRVS.Application.Service
{
    public static class ValidationService
    {
        // public static bool IsGuidNullOrEmpty(Guid? id)
        // {
        //     return !id.HasValue && id.Equals(Guid.Empty);
        // }
        // private static IBaseRepository<BaseAuditableEntity> repository;
        public static IRuleBuilderOptions<T, string?> ForeignKeyWithLookup<T>(this IRuleBuilder<T, string?> ruleBuilder, ILookupRepository repo, string propertyName)
        {
            // repository = repo;
            return ruleBuilder.MustAsync(async (lookup, c)
                        =>
            {
                Guid.TryParse(lookup, out Guid guid);
                var look = await repo.GetAsync(guid);
                return look == null ? false : true;
            }).WithMessage($"'{propertyName}' Unable to Get The Lookup");
        }

        public static IRuleBuilderOptions<T, string?> ForeignKeyWithAddress<T>(this IRuleBuilder<T, string?> ruleBuilder, IAddressLookupRepository repo, string propertyName)
        {
            // repository = repo;
            return ruleBuilder.MustAsync(async (ad, c)
                        =>
            {
                Guid.TryParse(ad, out Guid guid);
                var address = await repo.GetAsync(guid);
                return address == null ? false : true;
            }).WithMessage($"'{propertyName}' Unable to Get The Address");
        }

        public static IRuleBuilderOptions<T, string?> ForeignKeyWithPerson<T>(this IRuleBuilder<T, string?> ruleBuilder, IPersonalInfoRepository repo, string propertyName)
        {
            // repository = repo;
            return ruleBuilder.Must((pr)
                            =>
                            {
                                Guid.TryParse(pr, out Guid guid);
                                var person = repo.Exists(guid);
                                return person;
                            }).WithMessage($"'{propertyName}' Unable to Get The Person");
        }
        public static IRuleBuilderOptions<T, string?> ForeignKeyWithPaymentExamptionRequest<T>(this IRuleBuilder<T, string?> ruleBuilder, IPaymentExamptionRequestRepository repo, string propertyName)
        {
            // repository = repo;
            return ruleBuilder.MustAsync(async (r, c)
                            =>
                            {
                                Guid.TryParse(r, out Guid guid);
                                var e = await repo.GetAsync(guid);
                                return e == null ? false : true;
                            }).WithMessage($"'{propertyName}' Unable to Get The PaymentExamption Request");
        }
        public static IRuleBuilderOptions<T, ICollection<AddSupportingDocumentRequest>?> SupportingDocNull<T>(this IRuleBuilder<T, ICollection<AddSupportingDocumentRequest>?> ruleBuilder, string propertyName)
        {
            // repository = repo;
            return ruleBuilder.Must(docs =>
            {
                foreach (var d in docs)
                {
                    try
                    {
                        string myString = d.base64String.Substring(d.base64String.IndexOf(',') + 1);
                        Convert.FromBase64String(myString);
                        return d.Type == null || d.Type == "" ? false :
                            (d.Description == null || d.Description == "") ? false : true;
                    }
                    catch (FormatException)
                    {
                        return false;
                    }
                }
                return true;
            }).WithMessage($"'{propertyName}' Check your supporting documents");
        }

        public static IRuleBuilderOptions<T, string?> NotGuidEmpty<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder.Must(m => !string.IsNullOrEmpty(m) && !m.Equals(Guid.Empty)).WithMessage("{PropertyName} must not be null.").WithMessage("'{PropertyName}' is requered");
        }

        public static IRuleBuilderOptions<T, string> IsAbove18<T>(this IRuleBuilder<T, string> ruleBuilder, string propertyName)
        {

            return ruleBuilder.Must(d =>
                {
                    try
                    {
                        DateTime birthDate = DateTime.Parse(d);
                        EthiopianDate etDate = DateTime.Now.ToEthiopianDate();
                        return etDate.Year - birthDate.Year >= 18;
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }).WithMessage($"'{propertyName}' should be above 18.");
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