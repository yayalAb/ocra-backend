using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;


namespace AppDiv.CRVS.Application.Features.Settings.Commands.create
{
    // public class createSettingCommandHandler : IRequestHandler<createSettingCommand, createSettingCommandResponse>
    // {
    //     private readonly ISettingRepository _settingRepository;
    //     public createSettingCommandHandler(ISettingRepository settingRepository)
    //     {
    //         _settingRepository = settingRepository;
    //     }
    //     public async Task<createSettingCommandResponse> Handle(createSettingCommand request, CancellationToken cancellationToken)
    //     {

    //         // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request.customer);           

    //         var createSettingCommandResponse = new createSettingCommandResponse();

    //         var validator = new createSettingCommandValidator(_settingRepository);
    //         var validationResult = await validator.ValidateAsync(request, cancellationToken);

    //         //Check and log validation errors
    //         if (validationResult.Errors.Count > 0)
    //         {
    //             createSettingCommandResponse.Success = false;
    //             createSettingCommandResponse.ValidationErrors = new List<string>();
    //             foreach (var error in validationResult.Errors)
    //                 createSettingCommandResponse.ValidationErrors.Add(error.ErrorMessage);
    //             createSettingCommandResponse.Message = createSettingCommandResponse.ValidationErrors[0];
    //         }
    //         if (createSettingCommandResponse.Success)
    //         {
    //             var setting = new Setting
    //             {
    //                 Id = Guid.NewGuid(),
    //                 Key = request.Setting.Key,
    //                 Value = request.Setting.Value,
    //             };
    //             await _settingRepository.InsertAsync(setting, cancellationToken);
    //             var result = await _settingRepository.SaveChangesAsync(cancellationToken);
    //         }
    //         return createSettingCommandResponse;
    //     }
    // }
}
