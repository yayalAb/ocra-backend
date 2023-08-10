
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using AppDiv.CRVS.Application.Contracts.DTOs;
using CouchDB.Driver.Types;
using Newtonsoft.Json;

namespace AppDiv.CRVS.Application.CouchModels
{
    public class MarriageApplicationCouch : CouchDocument
    {
        //id2 is not used as database id the document id (_id ) is used as the entity id 
        [DataMember]
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public Guid Id2 { get; set; }
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