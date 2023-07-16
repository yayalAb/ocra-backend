using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class WorkHistoryDTO
    {
        public string? UserName { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Address { get; set; }
        public IEnumerable<string>? UserGroups { get; set; }

        public WorkHistoryDTO(WorkHistory? history)
        {
            var _convertor = new CustomDateConverter();
            UserName = history?.User.UserName;
            StartDate = _convertor.GregorianToEthiopic(history!.StartDate);
            EndDate = _convertor.GregorianToEthiopic(history.CreatedAt);
            Address = history?.Address?.AddressNameLang;
            UserGroups = history?.UserGroups?.Select(g => g.GroupName);
        }
    }
}