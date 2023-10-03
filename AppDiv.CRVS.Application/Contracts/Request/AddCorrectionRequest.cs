using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddCorrectionRequest
    {
        public Guid? Id { get; set; }
        public JArray? Description { get; set; }
        public Guid EventId { get; set; }
        public JObject Content { get; set; }
        public AddRequest Request { get; set; }
       public bool HasPayment { get; set; }=false;
    }
}