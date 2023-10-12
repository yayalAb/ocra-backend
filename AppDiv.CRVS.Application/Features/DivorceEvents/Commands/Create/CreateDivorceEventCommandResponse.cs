using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.DivorceEvents.Command.Create
{
    public class CreateDivorceEventCommandResponse : BaseResponse
    {
        public bool IsManualRegistration { get; set; } = false;
        public Guid EventId { get; set; }
        public IDivorceEventRepository? divorceEventRepository;
        public CreateDivorceEventCommandResponse() : base()
        {

        }

        //  public CustomerResponseDTO Customer { get; set; }  
    }
}
