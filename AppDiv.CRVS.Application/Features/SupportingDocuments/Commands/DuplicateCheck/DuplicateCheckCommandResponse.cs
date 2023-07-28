using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;

namespace AppDiv.CRVS.Application.Features.SupportingDocuments.Commands.DuplicateCheck
{
    public class DuplicateCheckCommandResponse :BaseResponse
    {
        public bool isEventDuplicate {get;set;} = false;
        public bool hasDuplicatePersonalInfo {get;set;} = false;
        public Guid? DuplicateEventId {get;set;}
        public DuplicateCheckCommandResponse() :base()
        {
            
        }
    }
}