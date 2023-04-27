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

namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllWoreda

{
    // Customer query with List<Customer> response
    public record GetAllWoredaQuery : IRequest<PaginatedList<WoredaDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetAllWoredaQueryHandler : IRequestHandler<GetAllWoredaQuery, PaginatedList<WoredaDTO>>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;

        public GetAllWoredaQueryHandler(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<PaginatedList<WoredaDTO>> Handle(GetAllWoredaQuery request, CancellationToken cancellationToken)
        {
            return await PaginatedList<WoredaDTO>
                .CreateAsync(
                     _AddresslookupRepository.GetAll()
                    .Where(a => a.ParentAddress == null)
                    .Select(a => new WoredaDTO
                    {
                        Id = a.Id,
                        Woreda = a.AddressName.Value<string>("en"),
                        Zone = a.ParentAddress != null && a.ParentAddress.ParentAddress != null
                                            ? a.ParentAddress.ParentAddress.AddressName.Value<string>("en")
                                            : null,
                        Region = a.ParentAddress != null && a.ParentAddress.ParentAddress != null && a.ParentAddress.ParentAddress.ParentAddress != null
                                            ? a.ParentAddress.ParentAddress.ParentAddress.AddressName.Value<string>("en")
                                            : null,
                        Country = a.ParentAddress != null && a.ParentAddress.ParentAddress != null && a.ParentAddress.ParentAddress.ParentAddress != null && a.ParentAddress.ParentAddress.ParentAddress.ParentAddress != null
                                            ? a.ParentAddress.ParentAddress.ParentAddress.ParentAddress.AddressName.Value<string>("en")
                                            : null,
                        Code = a.Code,
                        StatisticCode = a.StatisticCode
                    }).ToList()
                    , request.PageCount ?? 1, request.PageSize ?? 10);
        }

    }
}