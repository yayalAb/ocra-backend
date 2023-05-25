using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.PaymentExamptionRequests.Command.Create
{
    // Customer create command with CustomerResponse
    public record CreatePaymentExamptionRequestCommand : IRequest<CreatePaymentExamptionRequestCommandResponse>
    {
        public LanguageModel Reason { get; set; }
        public string? ExamptedClientId { get; set; }
        public string? ExamptedClientFullName { get; set; }
        public DateTime ExamptedDate { get; set; } = DateTime.Now;
        public string ExamptedBy { get; set; }
        public int? NumberOfClient { get; set; }
        public Guid? AddressId { get; set; }
        public string CertificateType { get; set; }


    }
}