
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
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Enums;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Contracts.DTOs;
using CouchDB.Driver.Query.Extensions;

namespace AppDiv.CRVS.Application.Service
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IdentityService> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserResolverService _userResolverService;
        private readonly HelperService _helperService;


        // private readonly IConfiguration _configuration;
        // private readonly TokenGeneratorService _tokenGeneratorService;

        public IdentityService(UserManager<ApplicationUser> userManager, ILogger<IdentityService> logger, SignInManager<ApplicationUser> signInManager, IUserResolverService userResolverService, HelperService helperService)
        {
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
            _userResolverService = userResolverService;
            _helperService = helperService;


            // _tokenGeneratorService = tokenGeneratorService;
        }
        public async Task<(Result result, IList<string>? roles, string? userId, AuthStatus status)> AuthenticateUser(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                var authResult = await _signInManager.PasswordSignInAsync(user, password, true, true);
                if (authResult.IsLockedOut)
                {

                    var message = "AccountLocked!:\n you have excedded the maximum limit of login attempts please contact the Administrator";

                    return (Result.Failure(new string[] { message }), null, null, AuthStatus.Locked);
                }
                if (!user.Status)
                {
                    var message = "Your Account is Diactivated please contact the Administrator";
                    return (Result.Failure(new string[] { message }), null, null, AuthStatus.Diactivated);
                }

                if (await _userManager.CheckPasswordAsync(user, password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var status = string.IsNullOrEmpty(user.Otp)
                                ? user.OtpExpiredDate != null && user.OtpExpiredDate > DateTime.Now
                                    ? AuthStatus.Active
                                    : AuthStatus.OtpExpired
                                : user.OtpExpiredDate == null
                                    ? AuthStatus.FirstTimeLogin
                                    : AuthStatus.OtpUnverified;
                    return (Result.Success(), userRoles, user.Id, status);


                }

            }


            string[] errors = new string[] { "Invalid username or password" };

            return (Result.Failure(errors), null, null, AuthStatus.UnAuthenticated);

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
            string password = HelperService.GeneratePassword();
            newUser.Otp = password;
            newUser.OtpExpiredDate = null;
            newUser.CreatedAt = DateTime.UtcNow;
            newUser.CreatedBy = _userResolverService.GetUserId() != null ? new Guid(_userResolverService.GetUserId()!) : Guid.Empty;
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
            string password = HelperService.GeneratePassword();
            user.Otp = password;
            user.OtpExpiredDate = null;
            user.CreatedAt = DateTime.UtcNow;
            user.CreatedBy = _userResolverService.GetUserId() != null ? new Guid(_userResolverService.GetUserId()!) : Guid.Empty;
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
            var isLoginOtp = user.Otp != null;
            var resetPassResult = await _userManager.ResetPasswordAsync(user, token, password);
            if (!resetPassResult.Succeeded)
            {
                var errors = resetPassResult.Errors.Select(e => e.Description);
                throw new NotFoundException($"password reset failed! \n {string.Join(",", errors)}\n {token}");
            }
            if (isLoginOtp)//login otp
            {
                user.Otp = null;
                user.OtpExpiredDate = DateTime.Now.AddDays(_helperService.getOtpExpiryDurationSetting());
            }
            else
            {
                user.PasswordResetOtp = null;
                user.PasswordResetOtpExpiredDate = null;
            }
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
                throw new NotFoundException($"Change password failed! ");
            }
            user.Otp = null;
            user.OtpExpiredDate = DateTime.Now.AddDays(_helperService.getOtpExpiryDurationSetting());
            await _userManager.UpdateAsync(user);
            return Result.Success();
        }

        public async Task<Result> UpdateResetOtp(string id, string? otp, DateTime? otpExpiredDate)
        {

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return Result.Failure(new string[] { "could not find user with the given id" });
            }
            user.PasswordResetOtp = otp;
            user.PasswordResetOtpExpiredDate = otpExpiredDate;
            var response = await _userManager.UpdateAsync(user);

            if (!response.Succeeded)
            {
                throw new NotFoundException($"User Updating failed! \n {response.Errors}");
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
            existingUser.PersonalInfo = user.PersonalInfo;
            existingUser.UserGroups = user.UserGroups;
            existingUser.SelectedAdminType = user.SelectedAdminType;
            existingUser.PreferedLanguage = user.PreferedLanguage;
            existingUser.AddressId = user.AddressId;
            existingUser.CanRegisterEvent = user.CanRegisterEvent;
            existingUser.FingerPrintApiUrl = user.FingerPrintApiUrl;
            //if the user was locked and status is updated to true
            if (user.Status && existingUser.LockoutEnd > DateTime.Now && existingUser.LockoutEnabled)
            {
                existingUser.LockoutEnd = DateTime.Now;
            }
            existingUser.Status = user.Status;
            existingUser.ModifiedAt = DateTime.UtcNow;
            existingUser.ModifiedBy = _userResolverService.GetUserId() != null ? new Guid(_userResolverService.GetUserId()!) : Guid.Empty;

            var response = await _userManager.UpdateAsync(existingUser);

            if (!response.Succeeded)
            {
                throw new NotFoundException($"User Updating failed! \n {response.Errors}");
            }


            return Result.Success();

        }
        public async Task<(Result result, string? email, string? phone)> ReGenerateOtp(string userId, string otp, DateTime otpExpiry)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return (Result.Failure(new string[] { "could not find user with the given id" }), null, null);
            }
            user.Otp = otp;
            user.OtpExpiredDate = otpExpiry;
            await _userManager.UpdateAsync(user);
            return (Result.Success(), user.Email, user.PhoneNumber);
        }
        public async Task<(Result result , string? userId , IList<string>? roles)> VerifyOtp(string userName, string otp)
        {
            var user = await _userManager.Users.Where(x => x.UserName == userName).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new NotFoundException($"user with username {userName} is not found");
            }
            if (user.Otp != otp)
            {
                throw new AuthenticationException("invalid otp");
            }
            user.Otp = null;
            user.OtpExpiredDate = DateTime.Now.AddDays(_helperService.getOtpExpiryDurationSetting());
            await _userManager.UpdateAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            return (result:Result.Success(), userId:user.Id, roles);
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
                throw new NotFoundException($"User Deleting failed! \n {response.Errors}");
            }

            return Result.Success();
        }

        public async Task<(bool exists , string? userId)> Exists(string arg, string searchBy)
        {
            ApplicationUser? user;
            switch (searchBy.ToLower())
            {
                case "email":
                    user = await _userManager.FindByEmailAsync(arg);
                   break;
                case "username":
                    user = await _userManager.FindByNameAsync(arg);
                    break;
                case "phone":
                    user =  _userManager.Users.Where(u => u.PhoneNumber == arg).FirstOrDefault();
                    break;
                default:
                    throw new NotFoundException("invalid search by string");
            }
               return (exists: user!= null, userId : user?.Id);
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
        public async Task<ApplicationUser> GetSingleUserAsync(string userId)
        {
            return await _userManager.Users
                                .Include(u => u.UserGroups)
                                .FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task<bool> CheckPhoneNumber(string phoneNumber)
        {
            return await _userManager.Users
                                .AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            if (user.Status && user.LockoutEnd > DateTime.Now && user.LockoutEnabled)
            {
                user.LockoutEnd = DateTime.Now;
            }
            user.ModifiedAt = DateTime.UtcNow;
            user.ModifiedBy = _userResolverService.GetUserId() != null ? new Guid(_userResolverService.GetUserId()!) : Guid.Empty;

            await _userManager.UpdateAsync(user);
        }

        public async Task<List<RoleDto>> GetUserRoles(string userId)
        {
            var userData =  _userManager.Users.Where(u => u.Id == userId).Include(u => u.UserGroups).FirstOrDefault()
                                        ??throw new NotFoundException("user not found");
            var userRoles = userData.UserGroups.SelectMany(ug => ug.Roles
            .Select(r => new RoleDto
            {
                page = r.Value<string>("page") ?? "",
                title = r.Value<string>("title") ?? "",
                canAdd = r.Value<bool>("canAdd"),
                canDelete = r.Value<bool>("canDelete"),
                canViewDetail = r.Value<bool>("canViewDetail"),
                canView = r.Value<bool>("canView"),
                canUpdate = r.Value<bool>("canUpdate")
            })).GroupBy(r => r.page.Trim(), StringComparer.OrdinalIgnoreCase).Select(g => new RoleDto
            {
                page = g.Key,
                title = g.FirstOrDefault()?.title ?? "",
                canAdd = g.Aggregate(false, (acc, x) => acc || x.canAdd),
                canDelete = g.Aggregate(false, (acc, x) => acc || x.canDelete),
                canUpdate = g.Aggregate(false, (acc, x) => acc || x.canUpdate),
                canView = g.Aggregate(false, (acc, x) => acc || x.canView),
                canViewDetail = g.Aggregate(false, (acc, x) => acc || x.canViewDetail)
            }).ToList();
            if (userData.CanRegisterEvent != null && userData.CanRegisterEvent == true)
            {
                Console.WriteLine($"yyyyyy..........{userData.CanRegisterEvent}");
                string marriage = Enum.GetName<Page>(Page.Marriage)?.ToLower()!;
                string birth = Enum.GetName<Page>(Page.Birht)?.ToLower()!;
                string adoption = Enum.GetName<Page>(Page.Adoption)?.ToLower()!;
                string death = Enum.GetName<Page>(Page.Death)?.ToLower()!;
                string divorce = Enum.GetName<Page>(Page.Devorce)?.ToLower()!;
                string marriageApplication = Enum.GetName<Page>(Page.MarriageApplication)?.ToLower()!;
                string marriageApplicationList = Enum.GetName<Page>(Page.MarriageApplicationList)?.ToLower()!;

                var pageNamesToMatch = new List<string>
                    { "marriage","birth","adoption","death","divorce","marriageapplication","marriageapplicationlist"};

                userRoles
                    .Where(p => pageNamesToMatch.Contains(p.page.ToLower()))
                    .ToList()
                    .ForEach(r =>
                    {
                        r.canAdd = true;
                        r.canView = true;
                        r.canUpdate = true;
                        r.canViewDetail = true;
                        r.canDelete = true;
                    });
            }
            return userRoles?? new List<RoleDto>();
        }
    }
}
