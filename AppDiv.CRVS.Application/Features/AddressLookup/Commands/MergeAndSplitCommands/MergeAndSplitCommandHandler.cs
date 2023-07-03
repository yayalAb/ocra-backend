using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.MergeAndSplitCommands
{
    public class MergeAndSplitCommandHandler : IRequestHandler<MergeAndSplitCommand, MergeAndSplitCommandResponse>
    {
        private readonly IAddressLookupRepository _AddressRepository;
        private readonly ILogger<MergeAndSplitCommandHandler> _logger;
        public MergeAndSplitCommandHandler(IAddressLookupRepository AddresslookupRepository, ILogger<MergeAndSplitCommandHandler> logger)
        {
            _AddressRepository = AddresslookupRepository;
            _logger = logger;
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
                //can use this instead of automapper
                foreach (var add in request.Address)
                {
                    var Address = new Address
                    {
                        Id = Guid.NewGuid(),
                        AddressName = add.AddressName,
                        StatisticCode = add.StatisticCode,
                        Code = add.Code,
                        AdminLevel = add.AdminLevel,
                        AreaTypeLookupId = add.AreaTypeLookupId,
                        ParentAddressId = add.ParentAddressId,
                        AdminTypeLookupId = add.AdminTypeLookupId,
                        OldAddressId = add.OldAddressId
                    };
                    var oldAddress = _AddressRepository.GetAll().Where(x => x.Id == add.OldAddressId).FirstOrDefault();
                    oldAddress.Status = true;
                    await _AddressRepository.UpdateAsync(oldAddress, x => x.Id);
                    await _AddressRepository.InsertAsync(Address, cancellationToken);
                }

                //
                var result = await _AddressRepository.SaveChangesAsync(cancellationToken);

                //var customerResponse = CustomerMapper.Mapper.Map<CustomerResponseDTO>(customer);
                // CreateLookupCommadResponse.Customer = customerResponse;          
            }
            return CreateAddressCommadResponse;
        }
    }
}
