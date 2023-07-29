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
using AppDiv.CRVS.Utility.Services;
using AppDiv.CRVS.Application.Exceptions;

namespace AppDiv.CRVS.Application.Service
{
    public static class ValidationService
    {
        // public static bool IsGuidNullOrEmpty(Guid? id)
        // {
        //     return !id.HasValue && id.Equals(Guid.Empty);
        // }
        // private static IBaseRepository<BaseAuditableEntity> repository;
        public static IRuleBuilderOptions<T, string?> ForeignKeyWithLookup<T>(this IRuleBuilder<T, string?> ruleBuilder, IEventRepository repo, string propertyName)
        {
            // repository = repo;
            return ruleBuilder.MustAsync(async (lookup, c)
                        =>
            {
                Guid.TryParse(lookup, out Guid guid);
                return repo.CheckForeignKey<Lookup>(guid);
            }).WithMessage($"'{propertyName}' Unable to Get The Lookup");
        }

        public static IRuleBuilderOptions<T, string?> ForeignKeyWithAddress<T>(this IRuleBuilder<T, string?> ruleBuilder, IEventRepository repo, string propertyName)
        {
            // repository = repo;
            return ruleBuilder.MustAsync(async (ad, c)
                        =>
            {
                Guid.TryParse(ad, out Guid guid);
                return repo.CheckForeignKey<Address>(guid);
            }).WithMessage($"'{propertyName}' Unable to Get The Address");
        }

        public static IRuleBuilderOptions<T, string?> ForeignKeyWithPerson<T>(this IRuleBuilder<T, string?> ruleBuilder, IEventRepository repo, string propertyName)
        {
            // repository = repo;
            return ruleBuilder.Must((pr)
                            =>
                            {
                                Guid.TryParse(pr, out Guid guid);
                                return repo.CheckForeignKey<PersonalInfo>(guid);
                            }).WithMessage($"'{propertyName}' Unable to Get The Person");
        }
        public static IRuleBuilderOptions<T, string?> ForeignKeyWithPaymentExamptionRequest<T>(this IRuleBuilder<T, string?> ruleBuilder, IEventRepository repo, string propertyName)
        {
            // repository = repo;
            return ruleBuilder.MustAsync(async (r, c)
                            =>
                            {
                                Guid.TryParse(r, out Guid guid);
                                return repo.CheckForeignKey<PaymentExamptionRequest>(guid);
                            }).WithMessage($"'{propertyName}' Unable to Get The PaymentExamption Request");
        }
        public static IRuleBuilderOptions<T, ICollection<AddSupportingDocumentRequest>?> SupportingDocNull<T>(this IRuleBuilder<T, ICollection<AddSupportingDocumentRequest>?> ruleBuilder, string propertyName)
        {
            var message = new List<string>();
            // repository = repo;
            return ruleBuilder.Must(docs =>
            {
                foreach (var d in docs)
                {
                    try
                    {
                        if (d.base64String != null)
                        {

                            string myString = d.base64String.Substring(d.base64String.IndexOf(',') + 1);
                            // Convert.FromBase64String(myString);
                            if (d.Label == null || d.Label == "")
                            {
                                message.Add("Label is required.");
                                // return false;
                            }
                            if (d.Type == null || d.Type == Guid.Empty)
                            {
                                message.Add("Type is required.");
                                // return false;
                            }
                            if (d.Description == null || d.Description == "")
                            {
                                message.Add("Description is required.");
                                // return false;
                            }
                            // return d.Label == null || d.Label == "" ? false : d.Type == null || d.Type == "" ? false :
                            //     (d.Description == null || d.Description == "") ? false : true;
                        }
                    }
                    catch (FormatException)
                    {
                        message.Add("Invalid Base64String.");
                        // return false;
                    }
                }

                return message.Count > 0 ? false : true;
            }).WithMessage($"'{propertyName}' Check your supporting documents {string.Join("\n\t", message)}");
        }

        public static IRuleBuilderOptions<T, string?> ValidCertificate<T>(this IRuleBuilder<T, string?> ruleBuilder, IEventRepository repo, string propertyName, string eventType)
        {
            return ruleBuilder.Must(c =>
            {
                var valid = int.TryParse(c.Substring(c.Length - 4), out _);
                if (valid)
                {
                    var certfcate = repo.GetAll().Where(x => x.CertificateId == c && x.EventType == eventType).FirstOrDefault();
                    return certfcate == null;
                }
                else
                {
                    return false;
                }
            }).WithMessage($"The last 4 digit of  {propertyName} must be int., and must be unique.");
        }

        public static IRuleBuilderOptions<T, string?> NotGuidEmpty<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder.Must(m => !string.IsNullOrEmpty(m) && !m.Equals(Guid.Empty)).WithMessage("{PropertyName} must not be null.").WithMessage("'{PropertyName}' is required");
        }

        public static IRuleBuilderOptions<T, string> IsAbove18<T>(this IRuleBuilder<T, string> ruleBuilder, string propertyName)
        {

            return ruleBuilder.Must(d =>
                {
                    try
                    {
                        DateTime converted = new CustomDateConverter(d).gorgorianDate;
                        return DateTime.Now.Year - converted.Year >= 18;
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }).WithMessage($"'{propertyName}' should be above 18.");
        }

        public static IRuleBuilderOptions<T, string> IsValidDate<T>(this IRuleBuilder<T, string> ruleBuilder, string propertyName)
        {

            return ruleBuilder.Must(d =>
                {
                    try
                    {
                        var dateConverter = new CustomDateConverter();
                        DateTime date = dateConverter.EthiopicToGregorian(d);
                        return date < DateTime.Now ? true : false;
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }).WithMessage($"'{propertyName}' Not Valid Ethiopian date.");
        }
        public static IRuleBuilderOptions<T, string> NotValidChildDate<T>(this IRuleBuilder<T, string> ruleBuilder)
        {

            return ruleBuilder.Must(d =>
                {
                    try
                    {
                        var dateConverter = new CustomDateConverter();
                        DateTime date = dateConverter.EthiopicToGregorian(d);
                        return false;
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }).WithMessage($"Child age is not valid");
        }
        public static IRuleBuilderOptions<T, string> IsValidRegistrationDate<T>(this IRuleBuilder<T, string> ruleBuilder, string propertyName)
        {
            return ruleBuilder.Must(d =>
                {
                    try
                    {
                        var dateConverter = new CustomDateConverter();
                        DateTime ethiodate = dateConverter.EthiopicToGregorian(d);

                        return ((ethiodate.Year == DateTime.Now.Year) || (ethiodate.Year == DateTime.Now.Year - 1)) ? true : false;
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }).WithMessage($"'{propertyName}' is must be this year or last year.");
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
         public static bool HaveGuardianSupportingDoc(ICollection<AddSupportingDocumentRequest>? supportingDocs,ILookupRepository _lookupRepo)
        {
            var supportingDocTypeLookupId = _lookupRepo
                                            .GetAll()
                                            .Where(l => l.Key.ToLower() == "supporting-document-type"
                                                && l.ValueStr.ToLower().Contains("guardian certificate"))
                                            .Select(l => l.Id)
                                            .FirstOrDefault();
            if (supportingDocTypeLookupId == null)
            {
                throw new NotFoundException($"guardian certificate supporting document type lookup is not found in database");
            }
            return supportingDocs != null && supportingDocs.Where(s => s.Type == supportingDocTypeLookupId).Any();
        }
    }
}