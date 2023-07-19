using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Create
{
    // Birth Event create command
    public record CreateBirthEventCommand(AddBirthEventRequest BirthEvent) : IRequest<CreateBirthEventCommandResponse>
    {
        // public Guid? Id {get;set;}
        // public Guid? FacilityTypeLookupId { get; set; }
        // public Guid? FacilityLookupId { get; set; }
        // public Guid? BirthPlaceId { get; set; }
        // public Guid? TypeOfBirthLookupId { get; set; }
        // // public Guid EventId { get; set; }
        // public virtual FatherInfoDTO Father { get; set; }
        // public virtual MotherInfoDTO Mother { get; set; }
        // public virtual AddEventForBirthRequest Event { get; set; }
        // public virtual AddBirthNotificationRequest? BirthNotification { get; set; } = null;
        
    }
}