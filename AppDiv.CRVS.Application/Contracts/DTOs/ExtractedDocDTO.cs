

using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Models;
using CouchDB.Driver.Types;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class ExtractedDocDTO
    {
        public Dictionary<string, string> UserPhoto {get; set; }
        public Dictionary<string, List<BiometricImagesAtt>?> FingerPrints {get;set;}
        public Dictionary<string, string> Signatures {get;set;}
        public List<(string? personId , SupportingDocument doc)> OtherDocs {get; set; }
    }
}