using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Application.Contracts.DTOs;
namespace AppDiv.CRVS.Infrastructure.CouchModels;
public class LookupEntry
{
   public  EntityState State { get; set; }
   public  LookupCouchDTO Lookup { get; set; }
}