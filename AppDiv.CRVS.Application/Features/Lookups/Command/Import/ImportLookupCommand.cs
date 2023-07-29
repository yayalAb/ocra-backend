using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AppDiv.CRVS.Application.Features.Lookups.Command.Import
{
    // Customer Import command with CustomerResponse
    public record ImportLookupCommand : IRequest<ImportLookupCommadResponse>
    {
        public IFormFile lookups { get; set; }
    }
}



