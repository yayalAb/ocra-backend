using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface ITokenGeneratorService
    {
        //public string GenerateToken(string userName, string password);
        public string GenerateJWTToken((string userId, string userName, Guid personId,  IList<string> roles, Guid userAddressId , int adminLevel) userDetails);
    }
}
