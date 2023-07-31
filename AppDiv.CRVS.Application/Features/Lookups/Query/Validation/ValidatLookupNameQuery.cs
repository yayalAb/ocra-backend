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
    public class ValidatLookupNameQuery : IRequest<object>
    {
        public string lookupName { get; set; }
        public string lang { get; set; }

    }

    public class ValidatLookupNameQueryHandler : IRequestHandler<ValidatLookupNameQuery, object>
    {

        private readonly ILookupRepository _lookupRepository;

        public ValidatLookupNameQueryHandler(ILookupRepository lookupQueryRepository)
        {
            _lookupRepository = lookupQueryRepository;
        }
        public async Task<object> Handle(ValidatLookupNameQuery request, CancellationToken cancellationToken)
        {
            bool isValid = true;
            var selectedlookup = _lookupRepository.GetAll().Where
             (x => EF.Functions.Like(x.ValueStr, $"%{request.lookupName}%")).ToList();
            if (selectedlookup.FirstOrDefault() != null)
            {
                foreach (var value in selectedlookup)
                {
                    if (value.Value.Value<string>(request.lang) == request.lookupName)
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            return new { isValid = isValid };
        }
    }
}