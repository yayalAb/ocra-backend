using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class LoginAuditGridDTO
    {
        public Guid? Id { get; set; }
        public string? UserName { get; set; }
        public string? Action { get; set; }
        public string? IpAddress { get; set; }
        public string? AuditDate { get; set; }
        public string? Address { get; set; }
        public LoginAuditGridDTO(LoginHistory? history)
        {
            var convertor = new CustomDateConverter();
            Id = history?.Id;
            AuditDate = history?.EventDate is not null ? convertor.GregorianToEthiopic((DateTime)history.EventDate) : null;
            UserName = history?.User.UserName;
            Address = $"{history?.User?.Address?.ParentAddress?.ParentAddress?.AddressNameLang}/{history?.User?.Address?.ParentAddress?.AddressNameLang}/{history?.User?.Address?.AddressNameLang}".Trim('/');
            Action = history?.EventType;
            IpAddress = history?.IpAddress;
            
        }
    }
}