using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Create
{
    public class CreateAdderssCommandResponse : BaseResponse
    {
        public CreateAdderssCommandResponse() : base()
        {

        }
        public Guid Id { get; set; }  
    }
}
