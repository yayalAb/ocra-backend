using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Extensions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Ranges.Query
{
    // Customer query with List<Customer> response
    public record GetAllRangeQuery : IRequest<PaginatedList<RangeDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public string? SearchString { get; set; }
    }

    public class GetAllRangeHandler : IRequestHandler<GetAllRangeQuery, PaginatedList<RangeDTO>>
    {
        private readonly IRangeRepository _rangeRepository;

        public GetAllRangeHandler(IRangeRepository rangeQueryRepository)
        {
            _rangeRepository = rangeQueryRepository;
        }
        public async Task<PaginatedList<RangeDTO>> Handle(GetAllRangeQuery request, CancellationToken cancellationToken)
        {
            var ranges = _rangeRepository.GetAll();
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                ranges = ranges.Where(
                    u => EF.Functions.Like(u.Key, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.Start.ToString(), "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.End.ToString(), "%" + request.SearchString + "%"));
            }
            return await ranges.Select(p => new RangeDTO
                {
                    Id = p.Id,
                    Key = p.Key,
                    Start = p.Start,
                    End = p.End
                })
            .PaginateAsync<RangeDTO, RangeDTO>(request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}