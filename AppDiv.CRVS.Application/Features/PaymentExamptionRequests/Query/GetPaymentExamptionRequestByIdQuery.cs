using AppDiv.CRVS.Application.Contracts.DTOs;
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
    public class GetPaymentExamptionRequestByIdQuery : IRequest<PaymentExamptionRequestDTO>
    {
        public Guid Id { get; private set; }

        public GetPaymentExamptionRequestByIdQuery(Guid Id)
        {
            this.Id = Id;
        }

    }

    public class GetPaymentExamptionRequestByIdHandler : IRequestHandler<GetPaymentExamptionRequestByIdQuery, PaymentExamptionRequestDTO>
    {
        private readonly IPaymentExamptionRequestRepository _PaymentExamptionRequestRepository;

        public GetPaymentExamptionRequestByIdHandler(IPaymentExamptionRequestRepository PaymentExamptionRequestRepository)
        {
            _PaymentExamptionRequestRepository = PaymentExamptionRequestRepository;
        }
        public async Task<PaymentExamptionRequestDTO> Handle(GetPaymentExamptionRequestByIdQuery request, CancellationToken cancellationToken)
        {

            var selectedPaymentExamptionRequest = _PaymentExamptionRequestRepository.GetAll()
            .Include(x => x.ExamptedBy)
            .Include(m => m.Address)
            .Where(x => x.Id == request.Id)
            .FirstOrDefault();
            return CustomMapper.Mapper.Map<PaymentExamptionRequestDTO>(selectedPaymentExamptionRequest);
            // return selectedCustomer;
        }
    }
}