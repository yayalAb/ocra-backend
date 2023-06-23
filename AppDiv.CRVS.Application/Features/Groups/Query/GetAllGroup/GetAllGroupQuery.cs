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

namespace AppDiv.CRVS.Application.Features.Groups.Query.GetAllGroup

{
    // Customer query with List<Customer> response
    public record GetAllGroupQuery : IRequest<PaginatedList<FetchGroupDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetAllGroupQueryHandler : IRequestHandler<GetAllGroupQuery, PaginatedList<FetchGroupDTO>>
    {
        private readonly IGroupRepository _groupRepository;

        public GetAllGroupQueryHandler(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }
        public async Task<PaginatedList<FetchGroupDTO>> Handle(GetAllGroupQuery request, CancellationToken cancellationToken)
        {
            var grouplist = _groupRepository.GetAll();
            return await PaginatedList<FetchGroupDTO>
                            .CreateAsync(
                                _groupRepository.GetAll().Select(g => new FetchGroupDTO
                                {
                                    Id = g.Id,
                                    GroupName = g.GroupName,
                                    Description = g.Description.Value<string>("eng")
                                })

                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}