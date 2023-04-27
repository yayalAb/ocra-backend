using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.PaymentRates.Query
{
    // Customer query with List<Customer> response
    public record GetAllPaymentRateQuery : IRequest<IEnumerable<PaymentRateDTO>>
    {

    }

    public class GetAllPaymentRateHandler : IRequestHandler<GetAllPaymentRateQuery, IEnumerable<PaymentRateDTO>>
    {
        private readonly IPaymentRateRepository _paymentRateRepository;

        public GetAllPaymentRateHandler(IPaymentRateRepository paymentRateQueryRepository)
        {
            _paymentRateRepository = paymentRateQueryRepository;
        }
        public async Task<IEnumerable<PaymentRateDTO>> Handle(GetAllPaymentRateQuery request, CancellationToken cancellationToken)
        {
            var paymentRateList = await _paymentRateRepository.GetAllWithAsync(new string[] { "PaymentTypeLookup", "EventLookup", "Address" });
            var paymentRateResponse = CustomMapper.Mapper.Map<List<PaymentRateDTO>>(paymentRateList);
            return paymentRateResponse;
        }
    }
}