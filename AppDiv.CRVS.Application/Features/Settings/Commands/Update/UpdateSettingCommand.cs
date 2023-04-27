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

namespace AppDiv.CRVS.Application.Features.Settings.Commands.Update
{
    // Customer create command with CustomerResponse
    public class UpdateSettingCommand : IRequest<SettingDTO>
    {

        public Guid Id { get; set; }
        public string Key { get; set; }
        public JObject Value { get; set; }
    }

    public class UpdateSettingCommandHandler : IRequestHandler<UpdateSettingCommand, SettingDTO>
    {
        private readonly ISettingRepository _settingRepository;
        public UpdateSettingCommandHandler(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }
        public async Task<SettingDTO> Handle(UpdateSettingCommand request, CancellationToken cancellationToken)
        {
            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request);
            Setting settingEntity = new Setting
            {
                Id = request.Id,
                Key = request.Key,
                Value = request.Value,
            };
            try
            {
                await _settingRepository.UpdateAsync(settingEntity, x => x.Id);
                await _settingRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var modifiedsetting = await _settingRepository.GetByIdAsync(request.Id);
            var settingResponse = CustomMapper.Mapper.Map<SettingDTO>(modifiedsetting);
            return settingResponse;
        }
    }
}