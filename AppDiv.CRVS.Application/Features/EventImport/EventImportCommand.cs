using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Features.EventImport
{
    // Customer EventImportCommand with  response
    public class EventImportCommand : IRequest<object>
    {
        public JArray Events { get; set; }

    }

    public class EventImportCommandHandler : IRequestHandler<EventImportCommand, object>
    {
        private readonly IEventRepository _eventRepository;

        public EventImportCommandHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
        public async Task<object> Handle(EventImportCommand request, CancellationToken cancellationToken)
        {
            Guid eventID = Guid.Empty;
            foreach (var item in request.Events)
            {
                // var

            }

            return "";
        }
    }
}