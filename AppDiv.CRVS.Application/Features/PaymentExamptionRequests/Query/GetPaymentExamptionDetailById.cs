
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Customers.Query
{
    // Customer GetCustomerByIdQuery with Customer response
    public class GetPaymentExamptionDetailById : IRequest<PaymentExamptionDetailDTO>
    {
        public Guid Id { get; set; }

    }

    public class GetPaymentExamptionDetailByIdHandler : IRequestHandler<GetPaymentExamptionDetailById, PaymentExamptionDetailDTO>
    {
        private readonly IPaymentExamptionRequestRepository _PaymentExamptionRequestRepository;

        public GetPaymentExamptionDetailByIdHandler(IPaymentExamptionRequestRepository PaymentExamptionRequestRepository)
        {
            _PaymentExamptionRequestRepository = PaymentExamptionRequestRepository;
        }
        public async Task<PaymentExamptionDetailDTO> Handle(GetPaymentExamptionDetailById request, CancellationToken cancellationToken)
        {

            var selectedPaymentExamptionRequest = _PaymentExamptionRequestRepository.GetAll()
            .Include(x => x.ExamptedBy)
            .Include(x => x.ExamptedBy.PersonalInfo)
            .Include(m => m.Address)
            .Where(x => x.Id == request.Id).Select(x => new PaymentExamptionDetailDTO
            {
                Id = x.Id,
                RequestedBy = x.ExamptedBy.PersonalInfo.FirstNameLang + " " + x.ExamptedBy.PersonalInfo.MiddleNameLang + " " + x.ExamptedBy.PersonalInfo.LastNameLang,
                NationalId = x.ExamptedClientId,
                Fullname = x.ExamptedClientFullName,
                ExamptedDate = x.ExamptedDateEt,
                CertificateType = x.CertificateType,
                ReasonOr = x.Reason.Value<string>("or"),
                ReasonAm = x.Reason.Value<string>("am"),
                NumberOfClient = (int?)x.NumberOfClient,
                RequestedAddress = x.Address.AddressNameLang,
                Status = x.status,
                RequestId = x.RequestId

            })
            .FirstOrDefault();
            if (selectedPaymentExamptionRequest == null)
            {
                throw new NotFoundException("Payment Examption Request With A given Id Does Not Found, or an error occured on geting Detail");
            }
            return selectedPaymentExamptionRequest;
        }
    }
}