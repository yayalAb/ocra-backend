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

namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllKebele

{
    // Customer query with List<Customer> response
    public record GetAllKebeleQuery : IRequest<PaginatedList<KebeleDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetAllKebeleQueryHandler : IRequestHandler<GetAllKebeleQuery, PaginatedList<KebeleDTO>>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;

        public GetAllKebeleQueryHandler(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<PaginatedList<KebeleDTO>> Handle(GetAllKebeleQuery request, CancellationToken cancellationToken)
        {

            return await PaginatedList<KebeleDTO>
                           .CreateAsync(
                                _AddresslookupRepository.GetAll()
                                .Include(a => a.ParentAddress)
                               .Where(a => a.AdminLevel == 5)
                               .Select(a => new KebeleDTO
                               {
                                   Id = a.Id,
                                   Kebele = a.AddressName.Value<string>("en"),
                                   Woreda = a.ParentAddress != null? a.ParentAddress.AddressName.Value<string>("en"):null,
                                   Zone = a.ParentAddress !=null && a.ParentAddress.ParentAddress != null
                                            ? a.ParentAddress.ParentAddress.AddressName.Value<string>("en")
                                            :null,
                                   Region = a.ParentAddress !=null && a.ParentAddress.ParentAddress != null && a.ParentAddress.ParentAddress.ParentAddress !=null
                                            ? a.ParentAddress.ParentAddress.ParentAddress.AddressName.Value<string>("en")
                                            :null,
                                   Country = a.ParentAddress !=null && a.ParentAddress.ParentAddress != null && a.ParentAddress.ParentAddress.ParentAddress !=null && a.ParentAddress.ParentAddress.ParentAddress.ParentAddress !=null
                                            ? a.ParentAddress.ParentAddress.ParentAddress.ParentAddress.AddressName.Value<string>("en")
                                            :null,
                                   Code = a.Code,
                                   StatisticCode = a.StatisticCode

                               }).ToList()
                               , request.PageCount ?? 1, request.PageSize ?? 10);


            // var lookups = CustomMapper.Mapper.Map<List<KebeleDTO>>(AddressList);
            // return FormatedRegion.ToList();

            // return (List<Customer>)await _customerQueryRepository.GetAllAsync();
        }

    }
}