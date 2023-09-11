﻿using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Features.DeathEvents.Command.Update
{
    // Customer create command with CustomerResponse
    public class UpdateDeathEventCommand : IRequest<UpdateDeathEventCommandResponse>
    {
        public Guid Id { get; set; }
        public string? BirthCertificateId { get; set; }
        public Guid? FacilityTypeLookupId { get; set; }
        public Guid? FacilityLookupId { get; set; }
        public Guid? DuringDeathId { get; set; }
        public Guid? DeathPlaceId { get; set; }
        public JObject? PlaceOfFuneral { get; set; }
        public AddDeathNotificationRequest? DeathNotification { get; set; }
        public AddEventForDeathRequest Event { get; set; }
        public bool IsFromCommand { get; set; } = false;
        public bool ValidateFirst { get; set; } = false;
    }
}