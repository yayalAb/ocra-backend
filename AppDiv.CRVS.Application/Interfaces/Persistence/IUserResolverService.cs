namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IUserResolverService
    {
        string GetUserEmail();
        string? GetUserId();
        string GetLocale();
        public Guid GetUserPersonalId();
        public int GetAdminLevel();
        public Guid GetWorkingAddressId();

    }
}
