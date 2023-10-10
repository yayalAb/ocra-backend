

using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Models;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class ExtractedDocDTO
    {
        public Dictionary<string, string> UserPhoto {get; set; }
        public Dictionary<string, List<BiometricImagesAtt>?> FingerPrints {get;set;}
        public Dictionary<string, string> Signatures {get;set;}
        public IEnumerable<SupportingDocument> OtherDocs {get; set; }
        public Dictionary<string, List<(string name, string value)>> UserDocs {get;set;}
    }
}