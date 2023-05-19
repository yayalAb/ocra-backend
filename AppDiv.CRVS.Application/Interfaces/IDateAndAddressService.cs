namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IDateAndAddressService
    {
        public (string, string) addressFormat(Guid? id);

        public (string[], string[]) SplitedAddress(Guid? id);
    }
}