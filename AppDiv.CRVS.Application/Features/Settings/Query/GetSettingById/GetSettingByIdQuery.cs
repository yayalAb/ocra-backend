
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Features.Settings.Query.GetAllSettings;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Settings.Query.GetSettingsById
{
    // Customer GetSettingByIdQuery with  response
    public class GetSettingByIdQuery : IRequest<Setting>
    {
        public Guid Id { get; set; }

    }

    public class GetSettingByIdQueryHandler : IRequestHandler<GetSettingByIdQuery, Setting>
    {
        private readonly IMediator _mediator;

        public GetSettingByIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<Setting> Handle(GetSettingByIdQuery request, CancellationToken cancellationToken)
        {
            var Settings = await _mediator.Send(new GetAllSettingQuery());
            var selectedSetting = Settings.FirstOrDefault(x => x.Id == request.Id);
            return CustomMapper.Mapper.Map<Setting>(selectedSetting);
            // return selectedCustomer;
        }
    }
}

