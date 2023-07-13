using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Newtonsoft.Json.Linq;


namespace AppDiv.CRVS.Application.Features.DivorceEvents.Command.Create
{
    // Customer create command with CustomerResponse
    public record CreateDivorceEventCommand : IRequest<CreateDivorceEventCommandResponse>
    {
        public virtual DivorcePartnersInfoDTO DivorcedWife { get; set; }
        public string? WifeBirthCertificateId { get; set; }
        public string? HusbandBirthCertificate { get; set; }
        // public DateTime DataOfMarriage { get; set; }
        public string? DateOfMarriageEt { get; set; }
        public LanguageModel? DivorceReason { get; set; }
        public virtual AddCourtCaseRequest CourtCase { get; set; }
        public int? NumberOfChildren { get; set; }
        public AddEventForDivorceRequest Event { get; set; }

    }
}