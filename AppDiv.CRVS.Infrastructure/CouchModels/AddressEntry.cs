using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Application.Contracts.DTOs;
namespace AppDiv.CRVS.Infrastructure.CouchModels;
public class AddressEntry
{
   public  EntityState State { get; set; }
   public  AddressCouchDTO Address { get; set; }
}