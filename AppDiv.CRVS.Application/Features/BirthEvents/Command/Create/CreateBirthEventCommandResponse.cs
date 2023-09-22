﻿using AppDiv.CRVS.Application.Common;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Create
{
    // Response for birth create.
    public class CreateBirthEventCommandResponse : BaseResponse
    {
        public bool IsManualRegistration { get; set; } = false;
        public Guid EventId { get; set; }
        public CreateBirthEventCommandResponse() : base()
        {

        }
    }
}
