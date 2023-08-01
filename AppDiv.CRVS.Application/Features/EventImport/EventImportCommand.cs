using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.EventImport
{
    // Customer EventImportCommand with  response
    public class EventImportCommand : IRequest<object>
    {
        public JObject[] Events { get; set; }

    }

    public class EventImportCommandHandler : IRequestHandler<EventImportCommand, object>
    {
        private readonly IEventImportService _importEventService;


        public EventImportCommandHandler(IEventImportService importEventService)
        {
            _importEventService = importEventService;
        }
        public async Task<object> Handle(EventImportCommand request, CancellationToken cancellationToken)
        {

            var response = await _importEventService.ImportEvent(request.Events);

            return response;
        }
    }
}