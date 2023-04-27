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
    public record GetAllWoredaQuery : IRequest<List<WoredaDTO>>
    {

    }

    public class GetAllWoredaQueryHandler : IRequestHandler<GetAllWoredaQuery, List<WoredaDTO>>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;

        public GetAllWoredaQueryHandler(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<List<WoredaDTO>> Handle(GetAllWoredaQuery request, CancellationToken cancellationToken)
        {
            var AddressList = await _AddresslookupRepository.GetAllAsync();
            var WoredaList = AddressList.Where(x => x.AdminLevel == 4);
            var FormatedWoreda = WoredaList.Select(co => new WoredaDTO
            {
                id = co.Id,
                Woreda = co.AddressName["en"].ToString(),
                Zone = co.ParentAddress?.AddressName["en"].ToString(),
                Region = co.ParentAddress?.ParentAddress?.AddressName["en"].ToString(),
                Country = co.ParentAddress?.ParentAddress?.ParentAddress?.AddressName["en"].ToString(),
                Code = co.Code,
                StatisticCode = co.StatisticCode
            });

            // var lookups = CustomMapper.Mapper.Map<List<WoredaDTO>>(AddressList);
            return FormatedWoreda.ToList();

            // return (List<Customer>)await _customerQueryRepository.GetAllAsync();
        }

    }
}