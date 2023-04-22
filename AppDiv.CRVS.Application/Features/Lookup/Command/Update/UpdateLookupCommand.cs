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

namespace AppDiv.CRVS.Application.Features.Lookup.Command.Update
{
    // Customer create command with CustomerResponse
    public class UpdateLookupCommand : IRequest<LookupDTO>
    {

        public Guid Id { get; set; }
        public string Key { get; set; }
        public string valueStr { get; set; }
        public string descriptionStr { get; set; }
        public string StatisticCode { get; set; }
        public string Code { get; set; }
    }

    public class UpdateLookupCommandHandler : IRequestHandler<UpdateLookupCommand, LookupDTO>
    {
        private readonly ILookupRepository _lookupRepository;
        private readonly ILookupRepository _LookupQueryRepository;
        public UpdateLookupCommandHandler(ILookupRepository _lookupRepository)
        {
            _lookupRepository = _lookupRepository;
        }
        public async Task<LookupDTO> Handle(UpdateLookupCommand request, CancellationToken cancellationToken)
        {
            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request);
            LookupModel LookupEntity = new LookupModel
            {
                Id = request.Id,
                Key = request.Key,
                valueStr = request.valueStr,
                descriptionStr = request.descriptionStr,
                StatisticCode = request.StatisticCode,
                Code = request.Code,
            };
            try
            {
                await _lookupRepository.UpdateAsync(LookupEntity, x => x.Id);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var modifiedLookup = await _LookupQueryRepository.GetByIdAsync(request.Id);
            var LookupResponse = CustomMapper.Mapper.Map<LookupDTO>(modifiedLookup);
            return LookupResponse;
        }
    }
}