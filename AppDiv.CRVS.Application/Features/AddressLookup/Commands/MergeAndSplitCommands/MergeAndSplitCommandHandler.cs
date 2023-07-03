using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.MergeAndSplitCommands
{
    public class MergeAndSplitCommandHandler : IRequestHandler<MergeAndSplitCommand, MergeAndSplitCommandResponse>
    {
        private readonly IAddressLookupRepository _AddressRepository;
        private readonly IMergeAndSplitAddressService _MergeAndSplitAddressService;
        public MergeAndSplitCommandHandler(IAddressLookupRepository AddresslookupRepository, IMergeAndSplitAddressService MergeAndSplitAddressService)
        {
            _AddressRepository = AddresslookupRepository;
            _MergeAndSplitAddressService = MergeAndSplitAddressService;
        }

        public async Task<MergeAndSplitCommandResponse> Handle(MergeAndSplitCommand request, CancellationToken cancellationToken)
        {
            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var CreateAddressCommadResponse = new MergeAndSplitCommandResponse();

            var validator = new MergeAndSplitCommandValidetor(_AddressRepository);
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
                await _MergeAndSplitAddressService.MergeAndSplitAddress(request.Address, cancellationToken);
                // var result = await _AddressRepository.SaveChangesAsync(cancellationToken);
            }
            return CreateAddressCommadResponse;
        }
    }
}
