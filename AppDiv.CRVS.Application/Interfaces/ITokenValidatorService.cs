using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface ITokenValidatorService
    {
        Task<bool> ValidateAsync(JwtSecurityToken token);
    }
}