
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Enums;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppDiv.CRVS.Application.Service
{
    public class EventDocumentService : IEventDocumentService
    {


        private readonly IFileService _fileService;
        private readonly ISupportingDocumentRepository _supportingDocumentRepository;

        public EventDocumentService(IFileService fileService, ISupportingDocumentRepository supportingDocumentRepository)
        {
            _fileService = fileService;
            _supportingDocumentRepository = supportingDocumentRepository;
        }
        public bool saveSupportingDocuments(ICollection<SupportingDocument> eventDocs, ICollection<SupportingDocument>? examptionDocs, string eventType)
        {

            var supportingDocFolder = Path.Combine("Resources", "SupportingDocuments", eventType);
            var examptiondocFolder = Path.Combine("Resources", "ExamptionDocuments", eventType);
            var fullPathSupporting = Path.Combine(Directory.GetCurrentDirectory(), supportingDocFolder);
            var fullPathExamption = Path.Combine(Directory.GetCurrentDirectory(), supportingDocFolder);


            eventDocs?.ToList().ForEach(doc =>
            {
                _fileService.UploadBase64FileAsync(doc.base64String, doc.Id.ToString(), fullPathSupporting, FileMode.Create);
            });
            examptionDocs?.ToList().ForEach(doc =>
            {
                _fileService.UploadBase64FileAsync(doc.base64String, doc.Id.ToString(), fullPathExamption, FileMode.Create);
            });
            return true;
        }

        public bool SaveCorrectionRequestSupportingDocuments(ICollection<SupportingDocument> eventDocs, ICollection<SupportingDocument>? examptionDocs, string eventType)
        {

            var supportingDocFolder = Path.Combine("Resources", "CorrectionRequest", "SupportingDocuments", eventType);
            var examptiondocFolder = Path.Combine("Resources", "CorrectionRequest", "ExamptionDocuments", eventType);
            var fullPathSupporting = Path.Combine(Directory.GetCurrentDirectory(), supportingDocFolder);
            var fullPathExamption = Path.Combine(Directory.GetCurrentDirectory(), supportingDocFolder);


            eventDocs?.ToList().ForEach(doc =>
            {
                _fileService.UploadBase64FileAsync(doc.base64String, doc.Id.ToString(), fullPathSupporting, FileMode.Create);
            });
            examptionDocs?.ToList().ForEach(doc =>
            {
                _fileService.UploadBase64FileAsync(doc.base64String, doc.Id.ToString(), fullPathExamption, FileMode.Create);
            });
            return true;
        }

        public async Task<(IEnumerable<SupportingDocument> supportingDocs, IEnumerable<SupportingDocument> examptionDocs)> createSupportingDocumentsAsync(IEnumerable<AddSupportingDocumentRequest> supportingDocs, IEnumerable<AddSupportingDocumentRequest> examptionDocs, Guid EventId, Guid? examptionId, CancellationToken cancellationToken)
        {
            IEnumerable<SupportingDocument> mappedDocs = new List<SupportingDocument>();
            IEnumerable<SupportingDocument> mappedExamptionDocs = new List<SupportingDocument>();
            if (supportingDocs != null)
            {
                mappedDocs = CustomMapper.Mapper.Map<IEnumerable<SupportingDocument>>(supportingDocs);
                mappedDocs.ToList().ForEach(doc =>
                {
                    doc.EventId = EventId;
                });
                await _supportingDocumentRepository.InsertAsync(mappedDocs, cancellationToken);
            }

            if (examptionDocs != null)
            {
                mappedExamptionDocs = CustomMapper.Mapper.Map<IEnumerable<SupportingDocument>>(examptionDocs);
                mappedExamptionDocs.ToList().ForEach(doc =>
                {
                    doc.PaymentExamptionId = examptionId;
                });
                await _supportingDocumentRepository.InsertAsync(mappedExamptionDocs, cancellationToken);
            }
            return (supportingDocs: mappedDocs, examptionDocs: mappedExamptionDocs);
        }

        public void savePhotos(Dictionary<string, string> personPhotos)
        {
            var folder = Path.Combine("Resources", "PersonPhotos");

            personPhotos.ToList().ForEach(p =>
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), folder);
                _fileService.UploadBase64FileAsync(p.Value, p.Key, fullPath, FileMode.Create);
            });

        }
        public (Dictionary<string, string> userPhotos, IEnumerable<SupportingDocument> otherDocs) extractSupportingDocs(PersonIdObj idObj, IEnumerable<SupportingDocument> supportingDocs)
        {
            Dictionary<string, string> userPhotos = new Dictionary<string, string>();
            supportingDocs.Where(d => d.Type.ToLower() == "webcam").ToList().ForEach(doc =>
            {

                if (doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Bride)!.ToLower() || doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Wife)!.ToLower())
                {
                    userPhotos.Add(idObj.WifeId?.ToString(), doc.base64String);
                    supportingDocs.ToList().Remove(doc);
                }
                else if (doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Groom)!.ToLower() || doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Husband)!.ToLower())
                {
                    userPhotos.Add(idObj.HusbandId.ToString(), doc.base64String);
                    supportingDocs.ToList().Remove(doc);
                }
                else if (doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Registrar)!.ToLower())
                {
                    userPhotos.Add(idObj.RegistrarId.ToString(), doc.base64String);
                    supportingDocs.ToList().Remove(doc);
                }
                else if (doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Child)!.ToLower())
                {
                    userPhotos.Add(idObj.ChildId.ToString(), doc.base64String);
                    supportingDocs.ToList().Remove(doc);
                }
                else if (doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Deceased)!.ToLower())
                {
                    userPhotos.Add(idObj.DeceasedId.ToString(), doc.base64String);
                    supportingDocs.ToList().Remove(doc);
                }
            });
            if (idObj.WitnessIds != null)
            {

                for (int i = 0; i < idObj.WitnessIds?.Count; i++)
                {

                    var witnessDoc = supportingDocs
                                .Where(doc => doc.Type.ToLower() == "webcam"
                                && doc.Label.ToLower() == $"{Enum.GetName<DocumentLabel>(DocumentLabel.Witness)?.ToLower()}{i}")
                                .FirstOrDefault();

                    if (witnessDoc != null)
                    {
                        var witnessId = idObj.WitnessIds.ToList()[i].ToString();
                        userPhotos.Add(witnessId, witnessDoc.base64String);
                        supportingDocs.ToList().Remove(witnessDoc);
                    }
                }
            }
            return (userPhotos: userPhotos, otherDocs: supportingDocs);
        }

    }
}
