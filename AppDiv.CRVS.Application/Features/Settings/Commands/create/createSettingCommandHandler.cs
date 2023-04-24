using System.Linq;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;

namespace AppDiv.CRVS.Application.Features.Settings.Commands.create
{

    public class createSettingCommandHandler : IRequestHandler<createSettingCommand, createSettingCommandResponse>
    {
        private readonly ISettingRepository _settingRepository;
        public CreateLookupCommandHandler(SettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }
        public async Task<CreateLookupCommadResponse> Handle(CreateLookupCommand request, CancellationToken cancellationToken)
        {

            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

            var CreateLookupCommadResponse = new CreateLookupCommadResponse();

            var validator = new createSettingCommandValidator(_settingRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            //Check and log validation errors
            if (validationResult.Errors.Count > 0)
            {
                createSettingCommandResponse.Success = false;
                createSettingCommandResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    createSettingCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                createSettingCommandResponse.Message = createSettingCommandResponse.ValidationErrors[0];
            }
            if (createSettingCommandResponse.Success)
            {
                //can use this instead of automapper
                var setting = new Setting
                {
                    Id = Guid.NewGuid(),
                    Key = request.setting.Key,
                    Value = request.setting.Value,
                };
                //
                await _settingRepository.InsertAsync(setting, cancellationToken);
                var result = await _settingRepository.SaveChangesAsync(cancellationToken);

                //var customerResponse = CustomerMapper.Mapper.Map<CustomerResponseDTO>(customer);
                // CreateLookupCommadResponse.Customer = customerResponse;          
            }
            return createSettingCommandResponse;
        }
    }
}
