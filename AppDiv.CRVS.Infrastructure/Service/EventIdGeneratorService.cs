
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Infrastructure.Services
{
    public class EventIdGeneratorService : IEventIdGeneratorService

    {
        private readonly ILogger<EventIdGeneratorService> _logger;
        private readonly CRVSDbContext _dbContext;

        public EventIdGeneratorService(ILogger<EventIdGeneratorService> logger, CRVSDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        public async void GenerateId(Guid addressId)
        {
        //     var address = await _dbContext.Addresses.FindAsync(addressId);
        //     if (address == null)
        //     {
        //         throw new NotFoundException($"address with id {addressId} is not found");
        //     }
        //     var dateSetting = await _dbContext.Settings
        //                         .Where(s => s.Key == "dateSetting")
        //                         .FirstOrDefaultAsync();
        //     if(dateSetting == null){
        //         throw new Exception("date setting not found");
        //     }
        //     var code = "";
        //     // if(DateTime.Now.Year != ){}
        }


    }
}
