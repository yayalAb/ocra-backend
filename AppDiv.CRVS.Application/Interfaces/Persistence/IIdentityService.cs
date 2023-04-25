using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Domain;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<string> GetUserNameAsync(string userId);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<(Result result, IList<string>? roles, string? userId)> AuthenticateUser(string email, string password);
        Task<(Result result, string password)> createUser(string userName, string email, Guid personalInfoId, Guid userGroupId);
        Task<(Result result, string password)> createUser(ApplicationUser user);

        Task<(Result result, string resetToken)> ForgotPassword(string email);
        Task<Result> ResetPassword(string email, string password, string token);
        Task<Result> ChangePassword(string email, string oldPassword, string newPassword);
        Task<Result> UpdateUser(string id, string userName, string email, Guid personalInfoId, string? opt, DateTime? otpExpiredDate);
        Task<Result> UpdateUserAsync(ApplicationUser user);
        IQueryable<ApplicationUser> AllUsers();
        Task<IEnumerable<ApplicationUser>> AllUsersDetailAsync();
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<Result> DeleteUser(string userId);
        // string GetUserGroupId(string userId);
    }
}

