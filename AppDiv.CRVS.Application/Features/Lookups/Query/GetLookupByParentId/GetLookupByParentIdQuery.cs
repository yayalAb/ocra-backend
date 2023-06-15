using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Lookups.Query.GetLookupByParentId
{

    public class GetLookupByParentIdQuery : IRequest<List<LookupByKeyDTO>>
    {
        public Guid Id { get; set; }
    }

    public class GetLookupByParentIdQueryHandler : IRequestHandler<GetLookupByParentIdQuery, List<LookupByKeyDTO>>
    {
        private readonly ILookupRepository _lookupRepository;

        public GetLookupByParentIdQueryHandler(ILookupRepository lookupQueryRepository)
        {
            _lookupRepository = lookupQueryRepository;
        }
        public async Task<List<LookupByKeyDTO>> Handle(GetLookupByParentIdQuery request, CancellationToken cancellationToken)
        {
            Lookup? parent = await _lookupRepository.GetAll()
                                .Where(l => l.Id == request.Id)
                                .FirstOrDefaultAsync();
            if (parent == null)
            {
                throw new NotFoundException($"parent lookup with id {request.Id} is not found");
            }
            return await _lookupRepository.GetAll().Where(l => EF.Functions.Like(parent.ValueStr, "%\"en\": \"" + l.Key + "\"%")
                            || EF.Functions.Like(parent.ValueStr, "%\"am\": \"" + l.Key + "\"%")
                            || EF.Functions.Like(parent.ValueStr, "%\"or\": \"" + l.Key + "\"%")
            ).Select(lo => new LookupByKeyDTO
            {
                id = lo.Id,
                Key = parent.ValueLang,
                Value = lo.ValueLang,
            }).ToListAsync();

        }
    }
}




