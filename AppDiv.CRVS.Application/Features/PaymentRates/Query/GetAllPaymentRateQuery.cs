using AppDiv.CRVS.Application.Common;
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
    public record GetAllPaymentRateQuery : IRequest<PaginatedList<FetchPaymentRateDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
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
            // var formatedLookup = lookups.Select(lo => new LookupForGridDTO
            // {
            //     id = lo.Id,
            //     Key = lo.Key,
            //     Value = lo?.Value["en"]?.ToString(),
            //     StatisticCode = lo?.StatisticCode,
            //     Code = lo?.Code


            // });
            // var paymentRateList = await _paymentRateRepository.GetAll(new string[] { "PaymentTypeLookup", "EventLookup", "Address" });
            return await PaginatedList<FetchPaymentRateDTO>
                            .CreateAsync(
                                _paymentRateRepository.GetAll().Select(pr => new FetchPaymentRateDTO
                                {
                                    Id = pr.Id,
                                    PaymentTypeId = pr.PaymentTypeLookupId,
                                    PaymentType = pr.PaymentTypeLookup.ValueLang != null ? pr.PaymentTypeLookup.ValueLang : "",
                                    EventId = pr.EventLookupId,
                                    Event = pr.EventLookup.ValueLang != null ? pr.EventLookup.ValueLang : "",
                                    IsForeign = pr.IsForeign,
                                    Amount = pr.Amount,
                                    Status = pr.Status,
                                    Backlog = pr.Backlog
                                    // Description = g.Description.Value<string>("eng")
                                })
                                , request.PageCount ?? 1, request.PageSize ?? 10);
            // var paymentRateResponse = CustomMapper.Mapper.Map<List<PaymentRateDTO>>(paymentRateList);
            // return paymentRateResponse;
        }
    }
}