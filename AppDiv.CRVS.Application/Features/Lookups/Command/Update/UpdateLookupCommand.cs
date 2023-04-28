using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Lookups.Command.Update
{
    // Customer create command with CustomerResponse
    public class UpdateLookupCommand : IRequest<LookupDTO>
    {

        public Guid Id { get; set; }
        public string Key { get; set; }
        public JObject Value { get; set; }
        public JObject? Description { get; set; }
        public string? StatisticCode { get; set; }
        public string? Code { get; set; }
    }

    public class UpdateLookupCommandHandler : IRequestHandler<UpdateLookupCommand, LookupDTO>
    {
        private readonly ILookupRepository _lookupRepository;
        public UpdateLookupCommandHandler(ILookupRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;
        }
        public async Task<LookupDTO> Handle(UpdateLookupCommand request, CancellationToken cancellationToken)
        {
            // _log.LogCritical(request.Value.ToString());
            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request);
            Lookup LookupEntity = new Lookup
            {
                Id = request.Id,
                Key = request.Key,
                Value = request.Value,
                Description = request?.Description,
                StatisticCode = request?.StatisticCode,
                Code = request?.Code,
            };
            try
            {
                await _lookupRepository.UpdateAsync(LookupEntity, x => x.Id);
                await _lookupRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw;
            }
            // var modifiedLookup = await _lookupRepository.GetByIdAsync(request.Id);
            var LookupResponse = CustomMapper.Mapper.Map<LookupDTO>(LookupEntity);
            return LookupResponse;
        }
    }
}