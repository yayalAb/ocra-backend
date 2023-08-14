using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AppDiv.CRVS.Application.Features.EventImport
{
    // Customer EventImportCommand with  response
    public class EventImportCommand : IRequest<object>
    {
        public IFormFile Event { get; set; }

    }

    public class EventImportCommandHandler : IRequestHandler<EventImportCommand, object>
    {
        private readonly IFileExtractorService _importEventService;
        public EventImportCommandHandler(IFileExtractorService importEventService)
        {
            _importEventService = importEventService;
        }
        public async Task<object> Handle(EventImportCommand request, CancellationToken cancellationToken)
        {
            var response = _importEventService.ExtractFile(request.Event);
            return response;
        }
    }
}