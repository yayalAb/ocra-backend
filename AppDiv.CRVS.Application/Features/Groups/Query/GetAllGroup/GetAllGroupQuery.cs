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

namespace AppDiv.CRVS.Application.Features.Groups.Query.GetAllGroup

{
    // Customer query with List<Customer> response
    public record GetAllGroupQuery : IRequest<List<GroupDTO>>
    {

    }

    public class GetAllGroupQueryHandler : IRequestHandler<GetAllGroupQuery, List<GroupDTO>>
    {
        private readonly IGroupRepository _groupRepository;

        public GetAllGroupQueryHandler(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }
        public async Task<List<GroupDTO>> Handle(GetAllGroupQuery request, CancellationToken cancellationToken)
        {
            var grouplist = await _groupRepository.GetAllAsync();
            var groups = CustomMapper.Mapper.Map<List<GroupDTO>>(grouplist);
            return groups;

            // return (List<Customer>)await _customerQueryRepository.GetAllAsync();
        }
    }
}