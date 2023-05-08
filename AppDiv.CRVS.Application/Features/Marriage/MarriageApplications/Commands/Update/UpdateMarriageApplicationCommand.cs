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

namespace AppDiv.CRVS.Application.Features.MarriageApplications.Command.Update
{
    public record UpdateMarriageApplicationCommand(): IRequest<UpdateMarriageApplicationCommandResponse>
    {
        public Guid Id {get; set; }
        public DateTime ApplicationDate { get; set; }
        public Guid ApplicationAddressId { get; set; }
        public AddPersonalInfoRequest BrideInfo { get; set;}
        public AddPersonalInfoRequest GroomInfo { get; set;}
        public Guid CivilRegOfficerId { get; set; }

       
    }
    }