using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive
{
    public static class Fill
    {
        public static D Filler<D, S>(D destination, S source)
        {
            Type type = destination.GetType();
            Type dest = source.GetType();
            PropertyInfo[] properties = type.GetProperties();
            PropertyInfo[] destProps = dest.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                foreach (var d in destProps)
                {
                    // d.GetValue(destination).Value<string>("am");
                    if (property.Name + "Am" == d.Name)
                    {
                        var value = new object();
                        if (property.Name.Contains("Lookup"))
                            value = (d.GetValue(source) as Lookup)?.Value?.Value<string>("am");
                        else
                            value = (d.GetValue(source) as JObject)?.Value<string>("am");

                        property.SetValue(destination, value);
                    }
                    if (property.Name + "Or" == d.Name)
                    {
                        var value = new object();
                        if (property.Name.Contains("Lookup"))
                            value = (d.GetValue(source) as Lookup)?.Value?.Value<string>("or");
                        else
                            value = (d.GetValue(source) as JObject)?.Value<string>("or");

                        property.SetValue(destination, value);
                    }
                    if (property.Name == d.Name)
                    {
                        property.SetValue(destination, d.GetValue(source));
                    }
                }
                // Console.WriteLine(property.Name);
            }
            return destination;
        }
    }
    public class Person
    {
        public string? FirstNameAm { get; set; }
        public string? FirstNameOr { get; set; }
        public string? MiddleNameAm { get; set; }
        public string? MiddleNameOr { get; set; }
        public string? LastNameAm { get; set; }
        public string? LastNameOr { get; set; }

        public string? BirthMonthOr { get; set; }
        public string? BirthMonthAm { get; set; }
        public string? BirthDay { get; set; }
        public string? BirthYear { get; set; }

        public virtual string? GenderAm { get; set; }
        public virtual string? GenderOr { get; set; }

        public string? BirthAddressOr { get; set; }
        public string? BirthAddressAm { get; set; }

        public string? ResidentAddressOr { get; set; }
        public string? ResidentAddressAm { get; set; }

        public string? NationalId { get; set; }

        public string? NationalityOr { get; set; }
        public string? NationalityAm { get; set; }

        public string? MarriageStatusOr { get; set; }
        public string? MarriageStatusAm { get; set; }

        public string? ReligionOr { get; set; }
        public string? ReligionAm { get; set; }

        public string? NationOr { get; set; }
        public string? NationAm { get; set; }

        public string? EducationalStatusOr { get; set; }
        public string? EducationalStatusAm { get; set; }

        public string? TypeOfWorkOr { get; set; }
        public string? TypeOfWorkAm { get; set; }

    }
}