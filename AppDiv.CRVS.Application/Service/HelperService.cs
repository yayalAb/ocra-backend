using System.Security.Cryptography;
using AppDiv.CRVS.Application.Interfaces.Persistence;

namespace AppDiv.CRVS.Application.Service
{
    public class HelperService
    {
        private readonly ISettingRepository _settingRepository;

        public HelperService(ISettingRepository settingRepository )
        {
            _settingRepository = settingRepository;
        }
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
        public int getOtpExpiryDurationSetting()
        {
            var generalSetting = _settingRepository.GetAll().Where(s => s.Key.ToLower() == "generalsetting").FirstOrDefault();
            int expiryDuration = 15;
            if (generalSetting != null)
            {
                expiryDuration = generalSetting.Value.Value<int?>("otp_expiry_duration_in_days") ?? expiryDuration;
            }
            return expiryDuration;
        }
    }
}


