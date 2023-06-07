using AppDiv.CRVS.Domain.Entities;
namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IDateAndAddressService
    {
        public (string, string) addressFormat(Guid? id);

        public (string[], string[]) SplitedAddress(string am, string or);
        public string[] SplitedAddressByLang(Guid? id);
    }
    public interface ILookupFromId
    {
        public string? GetLookupOr(Guid? id);
        public string? GetLookupAm(Guid? id);

    }
}