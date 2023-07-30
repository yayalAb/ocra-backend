using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Lookups.Query.Validation
{
    public class ValidatLookupNameQuery : IRequest<bool>
    {
        public string lookupName { get; set; }
        public string lang { get; set; }

    }

    public class ValidatLookupNameQueryHandler : IRequestHandler<ValidatLookupNameQuery, bool>
    {

        private readonly ILookupRepository _lookupRepository;

        public ValidatLookupNameQueryHandler(ILookupRepository lookupQueryRepository)
        {
            _lookupRepository = lookupQueryRepository;
        }
        public async Task<bool> Handle(ValidatLookupNameQuery request, CancellationToken cancellationToken)
        {
            var selectedlookup = _lookupRepository.GetAll().Where
             (x => EF.Functions.Like(x.ValueStr, $"%{request.lookupName}%"));
            return CustomMapper.Mapper.Map<bool>(selectedlookup);
        }
    }
}