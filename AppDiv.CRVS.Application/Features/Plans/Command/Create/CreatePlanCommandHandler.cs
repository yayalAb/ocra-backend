using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;

namespace AppDiv.CRVS.Application.Features.Plans.Command.Create
{

    public class CreatePlanCommandHandler : IRequestHandler<CreatePlanCommand, CreatePlanCommandResponse>
    {
        private readonly IPlanRepository _planRepository;
        public CreatePlanCommandHandler(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }
        public async Task<CreatePlanCommandResponse> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
        {

            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var response = new CreatePlanCommandResponse();

            var validator = new CreatePlanCommandValidator(_planRepository);
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

                var plan = CustomMapper.Mapper.Map<Plan>(request.Plan);
                plan.PopulationSize = request.Plan.MalePopulationSize + request.Plan.FemalePopulationSize;
                await _planRepository.InsertAsync(plan, cancellationToken);
                var result = await _planRepository.SaveChangesAsync(cancellationToken);

                //var customerResponse = CustomerMapper.Mapper.Map<CustomerResponseDTO>(customer);
                // createCustomerCommandResponse.Customer = customerResponse;          
            }
            return response;
        }
    }
}
