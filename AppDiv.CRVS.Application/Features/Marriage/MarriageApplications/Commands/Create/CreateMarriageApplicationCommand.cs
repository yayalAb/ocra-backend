using AppDiv.CRVS.Application.Contracts.DTOs;
using MediatR;


namespace AppDiv.CRVS.Application.Features.MarriageApplications.Command.Create
{
    // Customer create command with CustomerResponse
    public record CreateMarriageApplicationCommand: IRequest<CreateMarriageApplicationCommandResponse>
    {
        public Guid? Id {get;set;}
        public string ApplicationDateEt { get; set; }
        public Guid? ApplicationAddressId { get; set; }
        public BrideInfoDTO BrideInfo { get; set;}
        public GroomInfoDTO GroomInfo { get; set;}
        public Guid CivilRegOfficerId { get; set; }
        public DateTime? CreatedAt {get;set;}
        public Guid? CreatedBy {get;set;}

       
    }
    }