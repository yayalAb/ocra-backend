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

namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllKebele

{
    // Customer query with List<Customer> response
    public record GetAllKebeleQuery : IRequest<List<KebeleDTO>>
    {

    }

    public class GetAllKebeleQueryHandler : IRequestHandler<GetAllKebeleQuery, List<KebeleDTO>>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;

        public GetAllKebeleQueryHandler(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<List<KebeleDTO>> Handle(GetAllKebeleQuery request, CancellationToken cancellationToken)
        {
            var AddressList = await _AddresslookupRepository.GetAllAsync();
            var KebeleList = AddressList.Where(x => x.AdminLevel == 5);
            var FormatedRegion = KebeleList.Select(co => new KebeleDTO
            {
                id = co.Id,
                Kebele = co.AddressName["en"].ToString(),
                Woreda = co.ParentAddress?.AddressName["en"].ToString(),
                Zone = co.ParentAddress?.ParentAddress?.AddressName["en"].ToString(),
                Region = co.ParentAddress?.ParentAddress?.ParentAddress?.AddressName["en"].ToString(),
                Country = co.ParentAddress?.ParentAddress?.ParentAddress?.ParentAddress?.AddressName["en"].ToString(),
            });

            // var lookups = CustomMapper.Mapper.Map<List<KebeleDTO>>(AddressList);
            return FormatedRegion.ToList();

            // return (List<Customer>)await _customerQueryRepository.GetAllAsync();
        }

    }
}