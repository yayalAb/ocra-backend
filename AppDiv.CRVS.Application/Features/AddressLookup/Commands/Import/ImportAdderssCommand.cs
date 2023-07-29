using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Import
{
    public record ImportAdderssCommand : IRequest<ImportAdderssCommandResponse>
    {
        public Guid? ParentAddressId { get; set; }
        public IFormFile Address { get; set; }
        public Guid? AreaTypeId { get; set; }

    }
}