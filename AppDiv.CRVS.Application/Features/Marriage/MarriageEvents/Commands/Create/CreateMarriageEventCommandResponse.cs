using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.MarriageEvents.Command.Create
{
    public class CreateMarriageEventCommandResponse : BaseResponse
    {
        public bool IsManualRegistration { get; set; } = false;
        public Guid EventId { get; set; }
        public IMarriageEventRepository? marriageEventRepository;

        public CreateMarriageEventCommandResponse() : base()
        {

        }
        //  public CustomerResponseDTO Customer { get; set; }  
    }
}
