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
        public PlanType PlanType { get; set; }
        public Guid AddressId { get; set; }
        public Guid ParentPlanId { get; set; }
        public string EventType { get; set; }
        public DateTime PlannedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PlannedDateEt { get; set; }
        public string StartDateEt { get; set; }
        public string EndDateEt { get; set; }
        public int TargetAmount { get; set; }
        public string Remark { get; set; } = string.Empty;
        public string PlannedById { get; set; }

        public virtual Address Address { get; set; }
        public virtual Plan ParentPlan { get; set; }
        public virtual ICollection<Plan> ChildPlans { get; set; }
        public virtual ApplicationUser PlannedBy { get; set; }

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
        [NotMapped]
        public string? _StartDateEt
        {
            get { return StartDateEt; }
            set
            {
                StartDateEt = value;
                StartDate = new CustomDateConverter(StartDateEt).gorgorianDate; ;
            }
        }

        [NotMapped]
        public string? _EndDateEt
        {
            get { return EndDateEt; }
            set
            {
                EndDateEt = value;
                EndDate = new CustomDateConverter(EndDateEt).gorgorianDate; ;
            }
        }


    }
}