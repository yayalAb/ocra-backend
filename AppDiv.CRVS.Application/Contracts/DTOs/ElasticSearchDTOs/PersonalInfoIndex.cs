
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs
{
    public class PersonalInfoIndex
    {

        public string Id { get; set; }
        public string? FirstNameStr { get; set; }
        public string? FirstNameOr { get; set; }
        public string? FirstNameAm { get; set; }
        public string? MiddleNameStr { get; set; }
        public string? MiddleNameOr { get; set; }
        public string? MiddleNameAm { get; set; }
        public string? LastNameStr { get; set; }
        public string? LastNameOr { get; set; }
        public string? LastNameAm { get; set; }
        public string? NationalId { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? GenderOr { get; set; }
        public string? GenderAm { get; set; }
        public string? GenderEn { get; set; }
        public string? GenderStr { get; set; }
        public string? TypeOfWorkStr { get; set; }
        public string? TitleStr { get; set; }
        public string? MarriageStatusStr { get; set; }
        public string? AddressOr { get; set; }
        public string? AddressAm { get; set; }
        public bool? DeathStatus { get; set; }
        public bool? HasCivilMarriage { get; set; }






    }
}