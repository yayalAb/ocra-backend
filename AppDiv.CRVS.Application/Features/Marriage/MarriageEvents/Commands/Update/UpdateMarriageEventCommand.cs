using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.MarriageEvents.Command.Update
{
    // Customer Update command with CustomerResponse
    public record UpdateMarriageEventCommand : IRequest<UpdateMarriageEventCommandResponse>
    {
        public Guid Id { get; set; }
        public Guid MarriageTypeId { get; set; }
        public Guid? ApplicationId { get; set; }
        public bool HasCamera { get; set; } = false;
        public virtual BrideInfoDTO BrideInfo { get; set; }
        public virtual AddEventForMarriageRequest Event { get; set; }
        public virtual ICollection<AddWitnessRequest> Witnesses { get; set; }
        public bool IsFromCommand { get; set; } = false;
        public bool ValidateFirst { get; set; } = false;


    }
}