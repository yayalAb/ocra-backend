using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class BirthCertificateDTO
    {
        public string? CertifcateId { get; set; }
        public string? RegBookNo { get; set; }
        public string? BirthCertifcateId { get; set; }

        public string? ChildFirstNameOr { get; set; }
        public string? ChildFirstNameAm { get; set; }
        public string? ChildMiddleNameOr { get; set; }
        public string? ChildMiddleNameAm { get; set; }
        public string? ChildLastNameOr { get; set; }
        public string? ChildLastNameAm { get; set; }

        public string? GenderAm { get; set; }
        public string? GenderOr { get; set; }

        // public DateTime? BirthDate { get; set; }
        public string? BirthMonth { get; set; }
        public string? BirthDay { get; set; }
        public string? BirthYear { get; set; }

        // public AddressDTO? BirthPlace { get; set; }
        public string? BirthAddressOr { get; set; }
        public string? BirthAddressAm { get; set; }

        // public LookupDTO? ChildNationality { get; set; }
        public string? NationalityOr { get; set; }
        public string? NationalityAm { get; set; }

        // public JObject? MotherFirstName { get; set; }
        // public JObject? MotherMiddleName { get; set; }
        // public JObject? MotherLastName { get; set; }
        public string? MotherFullNameOr { get; set; }
        public string? MotherFullNameAm { get; set; }

        // public LookupDTO? MotherNationality { get; set; }
        public string? MotherNationalityOr { get; set; }
        public string? MotherNationalityAm { get; set; }

        // public JObject? FatherFirstName { get; set; }
        // public JObject? FatherMiddleName { get; set; }
        // public JObject? FatherLastName { get; set; }
        public string? FatherFullNameOr { get; set; }
        public string? FatherFullNameAm { get; set; }

        // public LookupDTO? FatherNationality { get; set; }
        public string? FatherNationalityOr { get; set; }
        public string? FatherNationalityAm { get; set; }

        // public DateTime? EventRegDate { get; set; }
        public string? EventRegisteredMonth { get; set; }
        public string? EventRegisteredDay { get; set; }
        public string? EventRegisteredYear { get; set; }

        // public DateTime? CertificateGenerationDate { get; set; } = DateTime.Now;
        public string? GeneratedMonth { get; set; }
        public string? GeneratedDay { get; set; }
        public string? GeneratedYear { get; set; }


        // public JObject? CivilRegOfficerFirstName { get; set; }
        // public JObject? CivilRegOfficerMiddleName { get; set; }
        // public JObject? CivilRegOfficerLastName { get; set; }
        public string? CivileRegOfficerFullNameOr { get; set; }
        public string? CivileRegOfficerFullNameAm { get; set; }


    }
}