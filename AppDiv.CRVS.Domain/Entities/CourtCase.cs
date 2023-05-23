using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using EthiopianCalendar;

namespace AppDiv.CRVS.Domain.Entities
{
    public class CourtCase : BaseAuditableEntity
    {
        public Guid CourtId { get; set; }
        public string CourtCaseNumber { get; set; }
        public DateTime ConfirmedDate { get; set; }
        public string? ConfirmedDateEt { get; set; }

        public virtual Court Court { get; set; }
        public virtual AdoptionEvent AdoptionEventCourtCase { get; set; }

        public virtual DivorceEvent DivorceEventCourtCase { get; set; }
        [NotMapped]
        public string? _ConfirmedDateEt
        {
            get { return ConfirmedDateEt; }
            set
            {
                ConfirmedDateEt = value;
                ConfirmedDate = new EthiopianDate(DateTime.Parse(ConfirmedDateEt).Year, DateTime.Parse(ConfirmedDateEt).Month, DateTime.Parse(ConfirmedDateEt).Day).ToGregorianDate();
            }
        }
    }
}