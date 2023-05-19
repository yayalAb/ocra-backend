using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IEventDocumentService
    {
         public bool saveSupportingDocuments(ICollection<SupportingDocument> eventDocs , ICollection<SupportingDocument>? examptionDocs,string eventType );

    }
}
