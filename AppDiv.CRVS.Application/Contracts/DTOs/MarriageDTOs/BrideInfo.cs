using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class BrideInfoDTO : UpdatePersonalInfoDTO
    {
        public Guid? BirthAddressId { get; set; }
        public AddressResponseDTOE? BirthAddressResponseDTO { get; set; }
        // public DateTime BirthDate { get; set; }
        public string BirthDateEt { get; set; }

    }
}