using AppDiv.CRVS.Application.Contracts.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Infrastructure.CouchModels
{
    public class PaymentRateEntry
    {
        public  EntityState State { get; set; }
        public Guid PaymentRateId  {get; set;} 
        public  PaymentRateCouchDTO? PaymentRate { get; set; }
    }
}