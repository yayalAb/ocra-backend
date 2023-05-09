using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.DivorceEvents.Command.Update
{
    // Customer Update command with CustomerResponse
    public record UpdateDivorceEventCommand() : IRequest<UpdateDivorceEventCommandResponse>
    {
        public Guid Id { get; set; }
        public virtual UpdatePersonalInfoRequest DivorcedWife { get; set; }
        public DateTime DataOfMarriage { get; set; }
        public DateTime DivorceDate { get; set; }
        public JObject DivorceReason { get; set; }
        public virtual UpdateCourtCaseRequest CourtCase { get; set; }
        public int NumberChildren { get; set; }
        public UpdateEventRequest Event { get; set; }

    }
}