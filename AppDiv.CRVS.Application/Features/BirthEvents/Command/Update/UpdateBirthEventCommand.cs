using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Update
{
    // Birth update command with Birth update response
    public class UpdateBirthEventCommand : IRequest<UpdateBirthEventCommandResponse>
    {
        public Guid Id { get; set; }
        public Guid FatherId { get; set; }
        public Guid MotherId { get; set; }
        public Guid? FacilityTypeLookupId { get; set; }
        public Guid? FacilityLookupId { get; set; }
        public Guid? BirthPlaceId { get; set; }
        public Guid? TypeOfBirthLookupId { get; set; }
        public Guid EventId { get; set; }

        public virtual FatherInfoDTO Father { get; set; }
        public virtual MotherInfoDTO Mother { get; set; }
        public virtual AddEventForBirthRequest Event { get; set; }
        public virtual AddBirthNotificationRequest? BirthNotification { get; set; }
        public bool IsFromCommand { get; set; } = false;
        public bool ValidateFirst { get; set; } = false;

    }
}