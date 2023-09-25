using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class WorkHistoryAuditGridDTO
    {
        public Guid? Id { get; set; }
        public string? UserName { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Roles { get; set; }
        public string? Address { get; set; }
        public WorkHistoryAuditGridDTO(WorkHistory? history)
        {
            var convertor = new CustomDateConverter();
            Id = history?.Id;
            UserName = history?.User.UserName;
            StartDate = convertor.GregorianToEthiopic(history!.StartDate);
            EndDate = convertor.GregorianToEthiopic(history!.CreatedAt);
            Roles = string.Join(", ",history?.UserGroups?.Select(g => g.GroupName)!);   
            Address = $"{history?.Address?.ParentAddress?.ParentAddress?.AddressNameLang}/{history?.Address?.ParentAddress?.AddressNameLang}/{history?.Address?.AddressNameLang}".Trim('/');
        }
    }
}