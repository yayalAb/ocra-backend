using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IEventDocumentService
    {
        public Task<(Dictionary<string, string> userPhotos, Dictionary<string, List<BiometricImagesAtt>?> fingerPrint)> SaveSupportingDocumentsAsync(Event savedEvent, List<AddSupportingDocumentRequest>? eventSupportingDocs, List<AddSupportingDocumentRequest>? exapmtionSupportingDocs, Guid? paymentExapmtionId, CancellationToken cancellationToken);
        public bool saveSupportingDocuments(ICollection<SupportingDocument> eventDocs, ICollection<SupportingDocument>? examptionDocs, string eventType);
        public bool MoveSupportingDocuments(ICollection<SupportingDocument> eventDocs, ICollection<SupportingDocument>? examptionDocs, string eventType);
        public bool SaveCorrectionRequestSupportingDocuments(ICollection<AddSupportingDocumentRequest> eventDocs, ICollection<AddSupportingDocumentRequest>? examptionDocs, string eventType);
        public Task<(IEnumerable<SupportingDocument> supportingDocs, IEnumerable<SupportingDocument> examptionDocs)> createSupportingDocumentsAsync(IEnumerable<AddSupportingDocumentRequest> supportingDocs, IEnumerable<AddSupportingDocumentRequest>? examptionDocs, Guid EventId, Guid? examptionId, CancellationToken cancellationToken);
        public void savePhotos(Dictionary<string, string> personPhotos, string folderName= "PersonPhotos");
        public void saveFingerPrints(Dictionary<string, List<BiometricImagesAtt>?> fingerprints);
        public void MovePhotos(Dictionary<string, string> personPhotos, string eventType);
        public ExtractedDocDTO extractSupportingDocs(PersonIdObj idObj, IEnumerable<SupportingDocument> supportingDocs);
        public (Dictionary<string, string> userPhotos, IEnumerable<SupportingDocument> otherDocs) ExtractOldSupportingDocs(PersonIdObj idObj, IEnumerable<SupportingDocument> supportingDocs);
        public List<string> getFingerprintUrls(List<string> personIds);
        public List<Dictionary<string, string>> getSingleFingerprintUrls(string? personId);

    }
}
