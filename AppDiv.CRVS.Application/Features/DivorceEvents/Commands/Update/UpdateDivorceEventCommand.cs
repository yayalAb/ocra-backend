using AppDiv.CRVS.Application.Contracts.DTOs;
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
    public record UpdateDivorceEventCommand : IRequest<UpdateDivorceEventCommandResponse>
    {
        public Guid Id { get; set; }
        public virtual DivorcePartnersInfoDTO DivorcedWife { get; set; }
        public string DateOfMarriageEt { get; set; }
        public JObject DivorceReason { get; set; }
        public virtual AddCourtCaseRequest CourtCase { get; set; }
        public int NumberOfChildren { get; set; }
        public AddEventForDivorceRequest Event { get; set; }

    }
}