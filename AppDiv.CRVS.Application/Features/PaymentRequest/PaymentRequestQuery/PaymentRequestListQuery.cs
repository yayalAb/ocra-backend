
using System.Text.RegularExpressions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.PaymentRequest.PaymentRequestQuery
{
    // Customer PaymentRequestListQuery with  response
    public class PaymentRequestListQuery : IRequest<List<PaymentRequestListDTO>>
    {

    }

    public class GetListOfLookupQueryHandler : IRequestHandler<PaymentRequestListQuery, List<PaymentRequestListDTO>>
    {
        private readonly IPaymentRequestRepository _PaymentRequestRepository;

        public GetListOfLookupQueryHandler(IPaymentRequestRepository PaymentRequestRepository)
        {
            _PaymentRequestRepository = PaymentRequestRepository;
        }
        public async Task<List<PaymentRequestListDTO>> Handle(PaymentRequestListQuery request, CancellationToken cancellationToken)
        {
            var paymentRequestList = _PaymentRequestRepository.GetAll()
            .Include(x => x.Request)
            .Include(x => x.Request.CivilRegOfficer)
            .Select(x => new PaymentRequestListDTO
            {
                Id = x.Id,
                RequestType = x.Request.RequestType,
                Amount = x.Amount,
                RequestedBy = x.Request.CivilRegOfficer.FirstNameLang + " " + x.Request.CivilRegOfficer.MiddleNameLang + " " + x.Request.CivilRegOfficer.LastNameLang,
                RequestedDate = x.CreatedAt,
            });
            return paymentRequestList.ToList();
        }
    }
}

