namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IUserResolverService
    {
        string GetUserEmail();
        string? GetUserId();
        string GetLocale();
        public Guid GetUserPersonalId();
        public Guid GetUserPersonalIdFromAccessTokenParam();
        public Guid GetWorkingAddressIdFromAccessTokenParam();
        public int GetAdminLevel();
        public Guid GetWorkingAddressId();

    }
}
