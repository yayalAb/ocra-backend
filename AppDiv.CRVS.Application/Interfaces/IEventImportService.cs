using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IEventImportService
    {
        public   Task<object> ImportEvent(JArray eventObj);
    }
}