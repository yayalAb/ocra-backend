using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class CorrectionApprovalDTO
    {
        public string? requestBy { get; set; }
        public JObject? OldData { get; set; }
        public JObject? NewData { get; set; }
        public int? CurrentStep { get; set; }
        public NotificationData? NotificationData {get; set; }


    }
}