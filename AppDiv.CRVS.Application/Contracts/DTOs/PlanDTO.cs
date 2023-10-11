using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Enums;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class PlanDTO
    {
        public Guid Id { get; set; }
        public string PlannedDateEt { get; set; }
        public uint BudgetYear { get; set; }
        public Guid? ParentPlanId { get; set; }
        public Guid AddressId { get; set; }
        public string Address { get; set; }
        public AddressResponseDTOE AddressResponseDTO { get; set; }
        public long MalePopulationSize { get; set; }
        public long FemalePopulationSize { get; set; }
        public long PopulationSize { get; set; }
        public ICollection<UpdateEventPlan> EventPlans { get; set; }
    }

    public class PlanGridDTO
    {
        public Guid Id { get; set; }
        public string EventType { get; set; }
        public string PlannedDateEt { get; set; }
        public uint? ActiveTargetAmount { get; set; }
        public uint? OtherTargetAmount { get; set; }
        public uint? FormNotification { get; set; }
        public uint BudgetYear { get; set; }
        public Guid? ParentPlanId { get; set; }
        public Guid AddressId { get; set; }
        public string Address { get; set; }
        public AddressResponseDTOE AddressResponseDTO { get; set; }
        public int ActualOccurance { get; set; }
        public long MalePopulationSize { get; set; }
        public long FemalePopulationSize { get; set; }
        public long PopulationSize { get; set; }
        public string Remark { get; set; } = string.Empty;
    }
}