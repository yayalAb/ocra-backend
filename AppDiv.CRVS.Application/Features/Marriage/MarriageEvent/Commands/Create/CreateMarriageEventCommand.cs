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

namespace AppDiv.CRVS.Application.Features.MarriageEvent.Command.Create
{
    // Customer create command with CustomerResponse
    public record CreateMarriageEventCommand(): IRequest<CreateMarriageEventCommandResponse>
    {
        public Guid MarriageTypeId { get; set; }
        public Guid ApplicationId { get; set; }
        public virtual PersonalInfo BrideInfo { get; set; }
        public virtual Event Event { get;set;}
        public virtual ICollection<Witness> Witnesses { get; }
    }
    }