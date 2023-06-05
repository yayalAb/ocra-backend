namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IDateAndAddressService
    {
        public (string, string) addressFormat(Guid? id);

        public (string[], string[]) SplitedAddress(string am, string or);
        public string[] SplitedAddressByLang(Guid? id);
    }
}