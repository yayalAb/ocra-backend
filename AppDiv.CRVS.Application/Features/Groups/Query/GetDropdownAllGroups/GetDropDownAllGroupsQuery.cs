using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
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

namespace AppDiv.CRVS.Application.Features.Groups.Query.GetDropDownAllGroups

{
    // Customer query with List<Customer> response
    public record GetDropDownAllGroups : IRequest<List<DropDownDto>>
    {

    }

    public class GetDropDownAllGroupsHandler : IRequestHandler<GetDropDownAllGroups, List<DropDownDto>>
    {
        private readonly IGroupRepository _groupRepository;

        public GetDropDownAllGroupsHandler(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }
        public async Task<List<DropDownDto>> Handle(GetDropDownAllGroups request, CancellationToken cancellationToken)
        {
            return _groupRepository.GetAll().Select(g => new DropDownDto{
                Key= g.Id,
                Value = g.GroupName
            }).ToList();

        }
    }
}