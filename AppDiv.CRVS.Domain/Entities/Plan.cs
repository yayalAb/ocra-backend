using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Base;
using AppDiv.CRVS.Domain.Enums;
using AppDiv.CRVS.Utility.Services;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Plan : BaseAuditableEntity
    {
        public DateTime PlannedDate { get; set; }
        public string PlannedDateEt { get; set; }
        public uint BudgetYear { get; set; }    
        public Guid AddressId { get; set; }
        public Guid? ParentPlanId { get; set; }
        public long PopulationSize { get; set; }
        public ICollection<EventPlan> EventPlans { get; set; } = null!;
        
        public Plan ParentPlan { get; set; }
        public Address Address { get; set; }
        

        [NotMapped]
        public string? _PlannedDateEt
        {
            get { return PlannedDateEt; }
            set
            {
                PlannedDateEt = value;
                PlannedDate = new CustomDateConverter(PlannedDateEt).gorgorianDate; ;
            }
        }

    }
}