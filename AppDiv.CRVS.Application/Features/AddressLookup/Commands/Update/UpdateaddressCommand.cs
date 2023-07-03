using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Update
{
    // Customer create command with CustomerResponse
    public class UpdateaddressCommand : IRequest<AddressDTO>
    {
        public Guid Id { get; set; }
        public JObject AddressName { get; set; }
        public string StatisticCode { get; set; }
        public string Code { get; set; }
        public string? CodePerfix { get; set; }
        public string? CodePostfix { get; set; }
        public int AdminLevel { get; set; }
        public Guid? AreaTypeLookupId { get; set; }
        public Guid? ParentAddressId { get; set; }
        public Guid? AdminTypeLookupId { get; set; }
    }

    public class UpdateaddressCommandHandler : IRequestHandler<UpdateaddressCommand, AddressDTO>
    {
        private readonly IAddressLookupRepository _addressLookupRepository;
        public UpdateaddressCommandHandler(IAddressLookupRepository addressLookupRepository)
        {
            _addressLookupRepository = addressLookupRepository;
        }
        public async Task<AddressDTO> Handle(UpdateaddressCommand request, CancellationToken cancellationToken)
        {
            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request);
            Address LookupEntity = new Address
            {
                Id = request.Id,
                AddressName = request.AddressName,
                StatisticCode = request.StatisticCode,
                Code = request.Code,
                CodePerfix = request.CodePerfix,
                CodePostfix = request.CodePostfix,
                AdminLevel = request.AdminLevel,
                AreaTypeLookupId = request?.AreaTypeLookupId,
                ParentAddressId = request?.ParentAddressId,
                AdminTypeLookupId = request?.AdminTypeLookupId
            };

            try
            {
                await _addressLookupRepository.UpdateAsync(LookupEntity, x => x.Id);
                await _addressLookupRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var modifiedLookup = await _addressLookupRepository.GetByIdAsync(request.Id);
            var LookupResponse = CustomMapper.Mapper.Map<AddressDTO>(modifiedLookup);
            return LookupResponse;
        }
    }
}