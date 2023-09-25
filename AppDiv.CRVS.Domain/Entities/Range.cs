using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class SystemRange : BaseAuditableEntity
    {
        public string Key { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }
}