using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Features.ProfileChangeRequests.Commands.Create
{

    public record CreateProfileChangeRequestCommand : IRequest<BaseResponse>
    {
        public string UserId { get; set; }
        public string? Remark {get;set;}
        public JObject Content {get;set;}
        public  AddRequest? Request { get; set; }
    }
}