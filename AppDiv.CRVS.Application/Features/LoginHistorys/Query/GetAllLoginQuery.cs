using System.Text.RegularExpressions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Application.Common;

namespace AppDiv.CRVS.Application.Features.LoginHistorys.Query
{
    public class GetAllLoginQuery : IRequest<PaginatedList<LoginHistory>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }
    public class GetAllLoginQueryHandler : IRequestHandler<GetAllLoginQuery, PaginatedList<LoginHistory>>
    {
        private readonly ILoginHistoryRepository _LoginHistoryRepo;

        public GetAllLoginQueryHandler(ILoginHistoryRepository LoginHistoryRepo)
        {
            _LoginHistoryRepo = LoginHistoryRepo;
        }
        public async Task<PaginatedList<LoginHistory>> Handle(GetAllLoginQuery request, CancellationToken cancellationToken)
        {

            return await PaginatedList<LoginHistory>
                            .CreateAsync(
                                _LoginHistoryRepo.GetAll()
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }

    }
}




