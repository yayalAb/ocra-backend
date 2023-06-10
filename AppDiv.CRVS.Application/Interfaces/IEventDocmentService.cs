using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IEventDocumentService
    {
        public bool saveSupportingDocuments(ICollection<SupportingDocument> eventDocs, ICollection<SupportingDocument>? examptionDocs, string eventType);
        public bool SaveCorrectionRequestSupportingDocuments(ICollection<SupportingDocument> eventDocs, ICollection<SupportingDocument>? examptionDocs, string eventType);
        public Task<(IEnumerable<SupportingDocument> supportingDocs, IEnumerable<SupportingDocument> examptionDocs)> createSupportingDocumentsAsync(IEnumerable<AddSupportingDocumentRequest> supportingDocs, IEnumerable<AddSupportingDocumentRequest> examptionDocs, Guid EventId, Guid? examptionId, CancellationToken cancellationToken);
        public void savePhotos(Dictionary<string, string> personPhotos);
        public (Dictionary<string, string> userPhotos, IEnumerable<SupportingDocument> otherDocs) extractSupportingDocs(PersonIdObj idObj, IEnumerable<SupportingDocument> supportingDocs);

    }
}
