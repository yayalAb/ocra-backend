using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;

namespace AppDiv.CRVS.Application.Features.Ranges.Command.Create
{

    public class CreateRangeCommandHandler : IRequestHandler<CreateRangeCommand, CreateRangeCommandResponse>
    {
        private readonly IRangeRepository _rangeRepository;
        public CreateRangeCommandHandler(IRangeRepository rangeRepository)
        {
            _rangeRepository = rangeRepository;
        }
        public async Task<CreateRangeCommandResponse> Handle(CreateRangeCommand request, CancellationToken cancellationToken)
        {

            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var response = new CreateRangeCommandResponse();

            var validator = new CreateRangeCommandValidator(_rangeRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                response.Success = false;
                response.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    response.ValidationErrors.Add(error.ErrorMessage);
                response.Message = response.ValidationErrors[0];
            }
            if (response.Success)
            {

                var range = CustomMapper.Mapper.Map<SystemRange>(request.Range);

                await _rangeRepository.InsertAsync(range, cancellationToken);
                var result = await _rangeRepository.SaveChangesAsync(cancellationToken);
                response.Message = "Range Created Succesfully";
                //var customerResponse = CustomerMapper.Mapper.Map<CustomerResponseDTO>(customer);
                // createCustomerCommandResponse.Customer = customerResponse;          
            }
            return response;
        }
    }
}
