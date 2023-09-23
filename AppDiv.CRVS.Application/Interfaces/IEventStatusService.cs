using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IEventStatusService
    {
        public   string ReturnEventStatus(string eventType,DateTime eventDate,DateTime EventRegDate);
    }
}