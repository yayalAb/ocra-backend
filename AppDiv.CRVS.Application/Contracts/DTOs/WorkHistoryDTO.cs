using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class WorkHistoryDTO
    {
        public string UserName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Address { get; set; }
        public IEnumerable<string> UserGroups { get; set; }

        public WorkHistoryDTO(WorkHistory history)
        {
            UserName = history.User.UserName;
            StartDate = history.StartDate;
            EndDate = history.CreatedAt;
            Address = history.Address.AddressNameLang;
            UserGroups = history.UserGroups?.Select(g => g.GroupName);
        }
    }
}