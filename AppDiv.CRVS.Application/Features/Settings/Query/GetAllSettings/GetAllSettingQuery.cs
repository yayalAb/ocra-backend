
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


namespace AppDiv.CRVS.Application.Features.Settings.Query.GetAllSettings

{
    // Customer query with List<Customer> response
    public record GetAllSettingQuery : IRequest<List<SettingDTO>>
    {

    }

    public class GetAllSettingQueryHandler : IRequestHandler<GetAllSettingQuery, List<SettingDTO>>
    {
        private readonly ISettingRepository _settingsRepository;

        public GetAllSettingQueryHandler(ISettingRepository settingsQueryRepository)
        {
            _settingsRepository = settingsQueryRepository;
        }
        public async Task<List<SettingDTO>> Handle(GetAllSettingQuery request, CancellationToken cancellationToken)
        {
            var settingsList = await _settingsRepository.GetAllAsync();
            var settings = CustomMapper.Mapper.Map<List<SettingDTO>>(settingsList);
            return settings;

            // return (List<Customer>)await _customerQueryRepository.GetAllAsync();
        }
    }
}