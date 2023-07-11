using System.Security.AccessControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Domain.Entities
{
    public class RevocationToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}