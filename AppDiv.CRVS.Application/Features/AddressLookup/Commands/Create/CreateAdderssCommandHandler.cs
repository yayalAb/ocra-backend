using System.Linq;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Create
{

    public class CreateAdderssCommandHandler : IRequestHandler<CreateAdderssCommand, CreateAdderssCommandResponse>
    {
        private readonly IAddressLookupRepository _AddressRepository;
        public CreateAdderssCommandHandler(IAddressLookupRepository AddresslookupRepository)
        {
            _AddressRepository = AddresslookupRepository;
        }
        public async Task<CreateAdderssCommandResponse> Handle(CreateAdderssCommand request, CancellationToken cancellationToken)
        {

            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var CreateAddressCommadResponse = new CreateAdderssCommandResponse();

            var validator = new CreateAdderssCommandValidator(_AddressRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                CreateAddressCommadResponse.Success = false;
                CreateAddressCommadResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    CreateAddressCommadResponse.ValidationErrors.Add(error.ErrorMessage);
                CreateAddressCommadResponse.Message = CreateAddressCommadResponse.ValidationErrors[0];
            }
            if (CreateAddressCommadResponse.Success)
            {
                //can use this instead of automapper
                var Address = new Address
                {
                    Id = Guid.NewGuid(),
                    StatisticCode = request.Address.AddressNameStr,
                    Code = request.Address.Code,
                    AdminLevelLookupId = request.Address.AdminLevelLookupId,
                    AreaTypeLookupId = request.Address.AreaTypeLookupId,
                    ParentAddressId = request.Address.ParentAddressId
                };
                //
                await _AddressRepository.InsertAsync(Address, cancellationToken);
                var result = await _AddressRepository.SaveChangesAsync(cancellationToken);

                //var customerResponse = CustomerMapper.Mapper.Map<CustomerResponseDTO>(customer);
                // CreateLookupCommadResponse.Customer = customerResponse;          
            }
            return CreateAddressCommadResponse;
        }
    }
}
