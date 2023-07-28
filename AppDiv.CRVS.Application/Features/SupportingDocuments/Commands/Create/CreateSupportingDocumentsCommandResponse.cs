
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Models;

namespace AppDiv.CRVS.Application.Features.SupportingDocuments.Commands.Create
{
    public class CreateSupportingDocumentsCommandResponse :BaseResponse
    {
        public Dictionary<string, BiometricImages> BiometricData {get;set;}
        public Event SavedEvent {get; set; }
        public CreateSupportingDocumentsCommandResponse(ISupportingDocumentRepository supportingDocumentRepo):base()
        {
            
        }
    }
}