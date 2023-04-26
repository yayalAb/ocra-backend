using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddContactInfoRequest
    {
        public Guid Id { get; set; }
        // public string Email { get; set; }
        public string? Website { get; set; }
        public string? Phone { get; set; }
        public string? HouseNo { get; set; }
        public string? Linkdin { get; set; }
        // public DateTime CreatedDate { get; set; }
        // public Guid PersonId { get; set; }
    }
}