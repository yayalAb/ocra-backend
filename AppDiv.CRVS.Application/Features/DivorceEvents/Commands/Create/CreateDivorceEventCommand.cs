using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;

using MediatR;
using Newtonsoft.Json.Linq;


namespace AppDiv.CRVS.Application.Features.DivorceEvents.Command.Create
{
    // Customer create command with CustomerResponse
    public record CreateDivorceEventCommand : IRequest<CreateDivorceEventCommandResponse>
    {
        public virtual DivorcePartnersInfoDTO DivorcedWife { get; set; }
        public DateTime DataOfMarriage { get; set; }
        public DateTime DivorceDate { get; set; }
        public JObject DivorceReason { get; set; }
        public virtual AddCourtCaseRequest CourtCase { get; set; }
        public int NumberOfChildren { get; set; }
        public AddEventForDivorceRequest Event { get; set; }

    }
}