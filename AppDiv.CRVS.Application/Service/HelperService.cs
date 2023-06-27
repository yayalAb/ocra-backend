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

            return code.Substring(0, 6);
        }

        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = (endDate.Year - startDate.Year) * 12 + (endDate.Month - startDate.Month);

            // Check if the end day is less than the start day
            if (endDate.Day < startDate.Day)
            {
                // Subtract one month if end day is less than start day
                monthsApart--;
            }

            return monthsApart;
        }
    }
}


