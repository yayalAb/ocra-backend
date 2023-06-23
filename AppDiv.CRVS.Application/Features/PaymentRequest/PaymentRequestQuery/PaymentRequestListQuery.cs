
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
using AppDiv.CRVS.Application.Common;

namespace AppDiv.CRVS.Application.Features.PaymentRequest.PaymentRequestQuery
{
    // Customer PaymentRequestListQuery with  response
    public class PaymentRequestListQuery : IRequest<PaginatedList<PaymentRequestListDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetListOfLookupQueryHandler : IRequestHandler<PaymentRequestListQuery, PaginatedList<PaymentRequestListDTO>>
    {
        private readonly IPaymentRequestRepository _PaymentRequestRepository;

        public GetListOfLookupQueryHandler(IPaymentRequestRepository PaymentRequestRepository)
        {
            _PaymentRequestRepository = PaymentRequestRepository;
        }
        public async Task<PaginatedList<PaymentRequestListDTO>> Handle(PaymentRequestListQuery request, CancellationToken cancellationToken)
        {
            var paymentRequestList = _PaymentRequestRepository.GetAll()
            .Include(x => x.Request)
            .Include(x => x.Request.CivilRegOfficer)
            .Include(x => x.Event)
            .Where(x => x.status == false)
            .Select(x => new PaymentRequestListDTO
            {
                Id = x.Id,
                RequestType = x.Reason.Value<string>("en"),
                Amount = x.Amount,
                RequestedBy = x.Event.EventOwener.FirstNameLang + " " + x.Event.EventOwener.MiddleNameLang + " " + x.Event.EventOwener.LastNameLang,
                RequestedDate = x.CreatedAt,
            });
            return
            await PaginatedList<PaymentRequestListDTO>
                           .CreateAsync(
                                paymentRequestList
                               , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}

