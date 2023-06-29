
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using AppDiv.CRVS.Application.Common;
using System.Text;
using AppDiv.CRVS.Domain.Repositories;

namespace AppDiv.CRVS.Application.Service
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IdentityService> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;


        // private readonly IConfiguration _configuration;
        // private readonly TokenGeneratorService _tokenGeneratorService;

        public IdentityService(UserManager<ApplicationUser> userManager, ILogger<IdentityService> logger, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;


            // _tokenGeneratorService = tokenGeneratorService;
        }
        public async Task<(Result result, IList<string>? roles, string? userId, bool isFirstTime)> AuthenticateUser(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                var authResult = await _signInManager.PasswordSignInAsync(user, password, true, true);
                if (authResult.IsLockedOut)
                {

                    var message = "AccountLocked!:\n you have excedded the maximum limit of login attempts please contact the Administrator";

                    return (Result.Failure(new string[] { message }), null, null, false);
                }
                if (!user.Status)
                {
                    var message = "Your Account is Diactivated please contact the Administrator";
                    return (Result.Failure(new string[] { message }), null, null, false);
                }

                if (await _userManager.CheckPasswordAsync(user, password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);




                    return (Result.Success(), userRoles, user.Id, !string.IsNullOrEmpty(user.Otp));


                }
            }


            string[] errors = new string[] { "Invalid login" };

            return (Result.Failure(errors), null, null, false);

        }

        public async Task<string> GetUserNameAsync(string userId)
        {
            var user = await _userManager.Users.FirstAsync(u => u.Id == userId);


            return user.UserName;
        }
        public async Task<Result> UnlockUserAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Result.Failure(new string[] { "could not find user with the given username" });
            }
            var response = await _userManager.SetLockoutEndDateAsync(user, DateTime.Now);
            if (response.Succeeded)
            {
                return Result.Success();
            }
            return Result.Failure((IEnumerable<string>)response.Errors);
        }
        // public string GetUserGroupId(string userId){
        //     return  _userManager.Users.First(u => u.Id == userId).UserGroupId;
        // }
        public async Task<(Result, string)> createUser(string userName, string email, Guid personalInfoId, Guid userGroupId)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return (Result.Failure(new string[] { "user with the given email already exists" }), string.Empty);
            }
            existingUser = await _userManager.FindByNameAsync(userName);
            if (existingUser != null)
            {
                return (Result.Failure(new string[] { "username is already taken" }), string.Empty);
            }
            var newUser = new ApplicationUser()
            {
                UserName = userName,
                Email = email,
                // UserGroupId = userGroupId,
                PersonalInfoId = personalInfoId
            };
            string password = GeneratePassword();
            newUser.Otp = password;
            var result = await _userManager.CreateAsync(newUser, password);
            if (!result.Succeeded)
            {
                return (result.ToApplicationResult(), string.Empty);
            }
            return (Result.Success(), password);
        }
        public async Task<(Result, string, string)> createUser(ApplicationUser user)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser != null)
            {
                return (Result.Failure(new string[] { "user with the given email already exists" }), string.Empty, string.Empty);
            }
            existingUser = await _userManager.FindByNameAsync(user.UserName);
            if (existingUser != null)
            {
                return (Result.Failure(new string[] { "username is already taken" }), string.Empty, string.Empty);
            }
            string password = GeneratePassword();
            user.Otp = password;
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return (result.ToApplicationResult(), string.Empty, string.Empty);
            }
            string userId = await _userManager.GetUserIdAsync(user);
            return (Result.Success(), password, userId);
        }

        public async Task<(Result, string)> ForgotPassword(string? email, string? userName)
        {
            var user = email != null
                            ? await _userManager.FindByEmailAsync(email)
                            : await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return (Result.Failure(new string[] { "could not find user with the given email" }), string.Empty);
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return (Result.Success(), token);
        }
        public async Task<Result> ResetPassword(string? email, string? userName, string password, string token)
        {
            var user = email != null
                        ? await _userManager.FindByEmailAsync(email)
                        : await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Result.Failure(new string[] { "user not found" });
            }
            var resetPassResult = await _userManager.ResetPasswordAsync(user, token, password);
            if (!resetPassResult.Succeeded)
            {
                var errors = resetPassResult.Errors.Select(e => e.Description);
                throw new Exception($"password reset failed! \n {string.Join(",", errors)}\n {token}");
            }
            user.Otp = null;
            await _userManager.UpdateAsync(user);
            return Result.Success();
        }

        public async Task<Result> ChangePassword(string userName, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Result.Failure(new string[] { "could not find user with the given username" });
            }
            var response = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!response.Succeeded)
            {
                throw new Exception($"Change password failed! ");
            }
            user.Otp = null;
            await _userManager.UpdateAsync(user);
            return Result.Success();
        }

        public async Task<Result> UpdateUser(string id, string userName, string email, Guid personalInfoId, string? otp, DateTime? otpExpiredDate)
        {

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return Result.Failure(new string[] { "could not find user with the given id" });
            }


            user.UserName = userName;
            user.Email = email;
            user.Otp = otp;
            user.OtpExpiredDate = otpExpiredDate;
            user.PersonalInfoId = personalInfoId;

            var response = await _userManager.UpdateAsync(user);

            if (!response.Succeeded)
            {
                throw new Exception($"User Updating failed! \n {response.Errors}");
            }

            return Result.Success();

        }

        public async Task<Result> UpdateUserAsync(ApplicationUser user)
        {
            var existingUser = await _userManager.Users
                    .Where(u => u.Id == user.Id)
                    .Include(u => u.UserGroups).FirstOrDefaultAsync();

            if (existingUser == null)
            {
                return Result.Failure(new string[] { "could not find user with the given id" });
            }

            existingUser.UserGroups.Clear();

            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            // existingUser.Otp = otp;
            // existingUser.OtpExpiredDate = otpExpiredDate;
            existingUser.PersonalInfo = user.PersonalInfo;
            existingUser.UserGroups = user.UserGroups;


            //if the user was locked and status is updated to true
            if (user.Status && existingUser.LockoutEnd > DateTime.Now && !existingUser.Status && existingUser.LockoutEnabled)
            {
                existingUser.LockoutEnd = DateTime.Now;
            }
            existingUser.Status = user.Status;

            var response = await _userManager.UpdateAsync(existingUser);

            if (!response.Succeeded)
            {
                throw new Exception($"User Updating failed! \n {response.Errors}");
            }

            return Result.Success();

        }
        public async Task<Result> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure(new string[] { "could not find user with the given id" });
            }
            var response = await _userManager.DeleteAsync(user);

            if (!response.Succeeded)
            {
                throw new Exception($"User Deleting failed! \n {response.Errors}");
            }

            return Result.Success();
        }

        private string GeneratePassword()
        {
            var options = _userManager.Options.Password;

            int length = options.RequiredLength;

            bool nonAlphanumeric = options.RequireNonAlphanumeric;
            bool digit = options.RequireDigit;
            bool lowercase = options.RequireLowercase;
            bool uppercase = options.RequireUppercase;

            StringBuilder password = new StringBuilder();
            Random random = new Random();

            while (password.Length < length)
            {
                char c = (char)random.Next(32, 126);

                password.Append(c);

                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }

            if (nonAlphanumeric)
                password.Append((char)random.Next(33, 48));
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));


            return password.ToString();
        }
        public IQueryable<ApplicationUser> AllUsers()
        {
            return _userManager.Users;
        }
        public IQueryable<ApplicationUser> AllUsersDetail()
        {
            return _userManager.Users
            .Include(u => u.PersonalInfo)
            .Include(p => p.PersonalInfo.BirthAddress)
            .Include(p => p.PersonalInfo.ResidentAddress)
            .Include(p => p.PersonalInfo.PlaceOfBirthLookup)
            .Include(p => p.PersonalInfo.NationalityLookup)
            .Include(p => p.PersonalInfo.TitleLookup)
            .Include(p => p.PersonalInfo.ReligionLookup)
            .Include(p => p.PersonalInfo.EducationalStatusLookup)
            .Include(p => p.PersonalInfo.TypeOfWorkLookup)
            .Include(p => p.PersonalInfo.MarraigeStatusLookup)
            .Include(p => p.PersonalInfo.NationLookup);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<ApplicationUser> GetUserByName(string userName)
        {



            return await _userManager.FindByNameAsync(userName);
        }
        public Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return _userManager.Users
                                    .Where(u => u.Id == userId)
                                    .Include(u => u.PersonalInfo)
                                    .ThenInclude(p => p.ContactInfo).SingleOrDefaultAsync();
        }

    }
}
