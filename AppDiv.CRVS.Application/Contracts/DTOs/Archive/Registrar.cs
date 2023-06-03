using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive
{
    public class RegistrarArchive : Person
    {
        public string? RelationShipOr { get; set; }
        public string? RelationShipAm { get; set; }
    }
}