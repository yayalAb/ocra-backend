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
        public uint TargetAmount { get; set; }
        public uint BudgetYear { get; set; }
        public string ParentPlan { get; set; }
        public string Address { get; set; }
        public int ActualOccurance { get; set; }
        public long PopulationSize { get; set; }
        public string Remark { get; set; } = string.Empty;
    }
}