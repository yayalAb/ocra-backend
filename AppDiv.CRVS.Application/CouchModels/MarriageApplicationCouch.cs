
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Application.Contracts.DTOs;
using CouchDB.Driver.Types;

namespace AppDiv.CRVS.Application.CouchModels
{
    public class MarriageApplicationCouch : CouchDocument
    {
        public Guid Id { get; set; }
        public string ApplicationDateEt { get; set; }
        public Guid? ApplicationAddressId { get; set; }
        public BrideInfoDTO BrideInfo { get; set; }
        public GroomInfoDTO GroomInfo { get; set; }
        public Guid CivilRegOfficerId { get; set; }
    [NotMapped]
        public bool Synced { get; set; }
        [NotMapped]

        public bool Certified { get; set; }
        [NotMapped]

        public DateTime CreatedDate { get; set; }
        [NotMapped]

        public bool? Updated { get; set; }

        

    }
}