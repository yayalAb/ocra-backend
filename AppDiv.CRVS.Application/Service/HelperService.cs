using System.Security.Cryptography;
namespace AppDiv.CRVS.Application.Service
{
    public class HelperService
    {
        public static string GenerateRandomCode()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[4];
            rngCryptoServiceProvider.GetBytes(randomBytes);

            var randomValue = BitConverter.ToUInt32(randomBytes, 0);
            var code = randomValue.ToString("D6");

            return code;
        }
    }
}

