using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.PaymentRates.Query
{
    // Customer query with List<Customer> response
    public record GetAllPaymentRateQuery : IRequest<PaginatedList<FetchPaymentRateDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public string? SearchString { get; set; }
    }

    public class GetAllPaymentRateHandler : IRequestHandler<GetAllPaymentRateQuery, PaginatedList<FetchPaymentRateDTO>>
    {
        private readonly IPaymentRateRepository _paymentRateRepository;

        public GetAllPaymentRateHandler(IPaymentRateRepository paymentRateQueryRepository)
        {
            _paymentRateRepository = paymentRateQueryRepository;
        }
        public async Task<PaginatedList<FetchPaymentRateDTO>> Handle(GetAllPaymentRateQuery request, CancellationToken cancellationToken)
        {
            var paymentRates = _paymentRateRepository.GetAll();
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                paymentRates = paymentRates.Where(
                    u => EF.Functions.Like(u.PaymentTypeLookup.ValueStr, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.EventLookup.ValueStr, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.Amount.ToString(), "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.Backlog.ToString(), "%" + request.SearchString + "%"));
            }
            return await PaginatedList<FetchPaymentRateDTO>
                            .CreateAsync(
                                paymentRates.Select(pr => new FetchPaymentRateDTO
                                {
                                    Id = pr.Id,
                                    PaymentTypeId = pr.PaymentTypeLookupId,
                                    PaymentType = pr.PaymentTypeLookup.ValueLang != null ? pr.PaymentTypeLookup.ValueLang : "",
                                    EventId = pr.EventLookupId,
                                    Event = pr.EventLookup.ValueLang != null ? pr.EventLookup.ValueLang : "",
                                    IsForeign = pr.IsForeign,
                                    Amount = pr.Amount,
                                    Status = pr.Status,
                                    Backlog = pr.Backlog,
                                    HasCamera = pr.HasCamera,
                                    HasVideo = pr.HasVideo
                                    // Description = g.Description.Value<string>("eng")
                                })
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}