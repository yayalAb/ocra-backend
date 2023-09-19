using System.Security.Cryptography;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;

namespace AppDiv.CRVS.Application.Service
{
    public class HelperService
    {
        private readonly ISettingRepository _settingRepository;

        public HelperService(ISettingRepository settingRepository)
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
        public static bool IsBase64String(string input)
        {
            try
            {
                var base64String = input.Substring(input.IndexOf(',') + 1);
                // Attempt to convert the input string to a byte array
                byte[] buffer = Convert.FromBase64String(base64String);
                return true;
            }
            catch (FormatException)
            {
                // If the conversion fails, catch the FormatException and return false
                return false;
            }
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
        public PasswordPolicy? getPasswordPolicySetting()
        {
            var passwordPolicy = _settingRepository.GetAll()
                .Where(s => s.Key.ToLower() == "passwordpolicy")
                .FirstOrDefault();
            return passwordPolicy == null ? null : new PasswordPolicy
            {
                Number = passwordPolicy.Value.Value<bool>("number"),
                LowerCase = passwordPolicy.Value.Value<bool>("lowerCase"),
                OtherChar = passwordPolicy.Value.Value<bool>("otherCharacter"),
                UpperCase = passwordPolicy.Value.Value<bool>("upperCase"),
                Min = passwordPolicy.Value.Value<int>("minLength"),
                Max = passwordPolicy.Value.Value<int>("maxLength")

            };
        }
        public static string GeneratePassword()
        {
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            Random random = new Random();
            char[] code = chars.OrderBy(c => random.Next()).Take(6).ToArray();
            Console.WriteLine("code ===========   " + new string(code));

            return new string(code);
        }
        public static async Task<Guid> GetWorkingAddressId(IUserResolverService _userResolverService, IPersonalInfoRepository _personalInfoRepository, Guid? personId)
        {
            if (personId != null)
            {
                var civilRegistrarUser = await _personalInfoRepository.GetUserByPersonalInfoId((Guid)personId) ?? throw new NotFoundException($"civil registrar user with personal info id {personId} is not found");
                return civilRegistrarUser.AddressId;
            }
            else
            {
                return _userResolverService.GetWorkingAddressId();
            }
        }
    }
}


