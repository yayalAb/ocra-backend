using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;


namespace AppDiv.CRVS.Domain.Base
{
    // Base entity or auditable entity
    public abstract class BaseAuditableEntity
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        public virtual Guid? CreatedBy { get; set; }
        public virtual Guid? ModifiedBy { get; set; }
        [NotMapped]
        protected string? lang { get; set; }
        public BaseAuditableEntity()
        {
            this.CreatedAt = DateTime.Now;
            this.Id = Guid.NewGuid();
            var httpContext = new HttpContextAccessor().HttpContext;
            if (httpContext != null && httpContext.Request.Headers.ContainsKey("lang"))
            {
                httpContext.Request.Headers.TryGetValue("lang", out StringValues headerValue);
                this.lang = headerValue.FirstOrDefault();
            }
            else
            {
                this.lang = "en";
            }

        }
    }
}
