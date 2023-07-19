using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;

using MediatR;

namespace AppDiv.CRVS.Application.Features.MarriageEvents.Command.Create
{
    // Customer create command with CustomerResponse
    public record CreateMarriageEventCommand : IRequest<CreateMarriageEventCommandResponse>
    {
        public Guid? Id {get;set;}
        public Guid MarriageTypeId { get; set; }
        public Guid? ApplicationId { get; set; }
        public string? BirthCertificateGroomId { get; set; }
        public string? BirthCertificateBrideId { get; set; }
        public bool HasCamera { get; set; } = false;
        public bool HasVideo { get; set; } = false;
        public virtual BrideInfoDTO BrideInfo { get; set; }
        public virtual AddEventForMarriageRequest Event { get; set; }
        public virtual ICollection<AddWitnessRequest> Witnesses { get; set; }
        public DateTime? CreatedAt {get; set; }
        public Guid? CreatedBy {get; set; }
    }
}