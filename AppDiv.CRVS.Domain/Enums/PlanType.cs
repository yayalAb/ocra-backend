using System.ComponentModel;

namespace AppDiv.CRVS.Domain.Enums
{
    public enum PlanType
    {
        [Description("Annual")]
        Annual,

        [Description("Semi-annual")]
        SemiAnnual,

        [Description("Quarter")]
        Quarter,

        [Description("Five-Year")]
        FiveYear
    }
}