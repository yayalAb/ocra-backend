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

namespace AppDiv.CRVS.Application.Features.MarriageEvents.Command.Create
{
    // Customer create command with CustomerResponse
    public record CreateMarriageEventCommand(): IRequest<CreateMarriageEventCommandResponse>
    {
        public Guid MarriageTypeId { get; set; }
        public Guid ApplicationId { get; set; }
        public virtual UpdatePersonalInfoRequest BrideInfo { get; set; }
        public virtual AddEventRequest Event { get;set;}
        public virtual ICollection<AddWitnessRequest> Witnesses { get; }
    }
    }