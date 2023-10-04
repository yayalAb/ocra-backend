using AppDiv.CRVS.Domain.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddPlanRequest
    {
        public string PlannedDateEt { get; set; }
        public uint BudgetYear { get; set; }
        public Guid? ParentPlanId { get; set; }
        public Guid AddressId { get; set; }
        public long PopulationSize { get; set; }
        public ICollection<AddEventPlan> EventPlans { get; set; }
    }
    public class AddEventPlan
    {
        public string EventType { get; set; }
        public uint ActiveTargetAmount { get; set; }
        public uint OtherTargetAmount { get; set; }
        public int ActualOccurance { get; set; }
        public JObject Remark { get; set; }
    }
    public class UpdateEventPlan 
    {
        public Guid Id { get; set; }
        public string EventType { get; set; }
        public uint ActiveTargetAmount { get; set; }
        public uint OtherTargetAmount { get; set; }
        public int ActualOccurance { get; set; }
        public JObject Remark { get; set; }
    }
}