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

namespace AppDiv.CRVS.Application.Features.MarriageApplications.Command.Create
{
    // Customer create command with CustomerResponse
    public record CreateMarriageApplicationCommand: IRequest<CreateMarriageApplicationCommandResponse>
    {
        public string ApplicationDateEt { get; set; }
        public Guid? ApplicationAddressId { get; set; }
        public BrideInfoDTO BrideInfo { get; set;}
        public GroomInfoDTO GroomInfo { get; set;}
        public Guid CivilRegOfficerId { get; set; }

       
    }
    }