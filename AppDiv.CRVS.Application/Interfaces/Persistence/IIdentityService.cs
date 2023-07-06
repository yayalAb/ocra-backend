using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain.Enums;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<string> GetUserNameAsync(string userId);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<ApplicationUser> GetUserByName(string userName);
        Task<(Result result, IList<string>? roles, string? userId, AuthStatus status)> AuthenticateUser(string email, string password);
        Task<(Result result, string password)> createUser(string userName, string email, Guid personalInfoId, Guid userGroupId);
        Task<(Result result, string password, string id)> createUser(ApplicationUser user);

        Task<(Result result, string resetToken)> ForgotPassword(string? email, string? userName);
        Task<Result> ResetPassword(string? email, string? userName, string password, string token);
        Task<Result> ChangePassword(string userName, string oldPassword, string newPassword);
        Task<Result> UpdateResetOtp(string id, string? otp, DateTime? otpExpiredDate);
        Task<Result> UpdateUserAsync(ApplicationUser user);
        IQueryable<ApplicationUser> AllUsers();
        IQueryable<ApplicationUser> AllUsersDetail();
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<ApplicationUser> GetSingleUserAsync(string userId);
        Task<Result> DeleteUser(string userId);
        public Task<Result> UnlockUserAsync(string userName);
        public Task<Result> VerifyOtp(string userName, string otp);
        public Task<(Result result, string? email, string? phone)> ReGenerateOtp(string userId, string otp, DateTime otpExpiry);
        Task<bool> Exists(string arg, bool isEmail);
        public string GeneratePassword();
        // string GetUserGroupId(string userId);
    }
}

