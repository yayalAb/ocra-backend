
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
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
                await _supportingDocumentRepository.InsertAsync(mappedDocs , cancellationToken);
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

        public void savePhotos(Dictionary<string, string>personPhotos){
            var folder = Path.Combine("Resources", "PersonPhotos");

            personPhotos.ToList().ForEach(p => {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(),folder);
                _fileService.UploadBase64FileAsync(p.Value,p.Key,fullPath,FileMode.Create);
            });
            
        }

    }
}
