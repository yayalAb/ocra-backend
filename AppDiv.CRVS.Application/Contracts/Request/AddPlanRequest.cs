using AppDiv.CRVS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddPlanRequest
    {
        public string EventType { get; set; }
        public string PlannedDateEt { get; set; }
        public uint TargetAmount { get; set; }
        public uint BudgetYear { get; set; }
        public string Remark { get; set; } = string.Empty;
    }
}