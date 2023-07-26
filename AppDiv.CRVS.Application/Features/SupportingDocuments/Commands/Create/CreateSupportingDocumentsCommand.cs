
using AppDiv.CRVS.Domain.Entities;
using MediatR;

namespace AppDiv.CRVS.Application.Features.SupportingDocuments.Commands.Create
{
    public class CreateSupportingDocumentsCommand : IRequest<CreateSupportingDocumentsCommandResponse>
    {
        public Guid EventId {get;set;}
        public Guid? PaymentExamptionId {get;set;}
        public List<AddSupportingDocumentRequest> EventSupportingDocuments {get;set;}
        
        public List<AddSupportingDocumentRequest>? ExamptionSupportingDocuments {get;set;}
    }
}