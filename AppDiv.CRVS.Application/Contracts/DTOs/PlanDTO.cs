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
        public PlanType PlanType { get; set; }
        public Guid AddressId { get; set; }
        public Guid ParentPlanId { get; set; }
        public string EventType { get; set; }
        public string PlannedDateEt { get; set; }
        public string StartDateEt { get; set; }
        public string EndDateEt { get; set; }
        public int TargetAmount { get; set; }
        public string Remark { get; set; } = string.Empty;
        public Guid PlannedById { get; set; }
    }
}