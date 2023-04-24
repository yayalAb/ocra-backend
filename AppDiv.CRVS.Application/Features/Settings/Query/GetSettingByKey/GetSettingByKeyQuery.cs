using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Settings.Query.GetAllSettings;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Settings.Query.GetSettingByKey
{

    public class GetSettingByKeyQuery : IRequest<List<SettingDTO>>
    {
        public string Key { get; set; }


    }

    public class GetSettingByKeyQueryHandler : IRequestHandler<GetSettingByKeyQuery, List<SettingDTO>>
    {
        private readonly IMediator _mediator;


        public GetSettingByKeyQueryHandler(IMediator mediator)
        {
            _mediator = mediator;

        }
        public async Task<List<SettingDTO>> Handle(GetSettingByKeyQuery request, CancellationToken cancellationToken)
        {
            var Allsettings = await _mediator.Send(new GetAllSettingQuery());


            var settings = CustomMapper.Mapper.Map<List<SettingDTO>>(Allsettings.Where(x => x.Key == request.Key));

            return settings;
        }
    }
}




