using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Enums;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class PlanDTO
    {
        public Guid Id { get; set; }
        public string EventType { get; set; }
        public string PlannedDateEt { get; set; }
        public int TargetAmount { get; set; }
        public int BudgetYear { get; set; }
        public string Remark { get; set; } = string.Empty;
    }
}