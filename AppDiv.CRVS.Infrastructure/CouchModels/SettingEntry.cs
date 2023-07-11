using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Application.Contracts.DTOs;
namespace AppDiv.CRVS.Infrastructure.CouchModels;
public class SettingEntry
{
   public  EntityState State { get; set; }
   public  SettingDTO Setting { get; set; }
}