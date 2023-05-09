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
    public record UpdateMarriageEventCommand(): IRequest<UpdateMarriageEventCommandResponse>
    {
        public Guid Id { get; set; }
        public Guid MarriageTypeId { get; set; }
        public Guid ApplicationId { get; set; }
        public virtual UpdatePersonalInfoRequest BrideInfo { get; set; }
        public virtual UpdateEventRequest Event { get;set;}
        public virtual ICollection<UpdateWitnessRequest> Witnesses { get; }
    }
    }