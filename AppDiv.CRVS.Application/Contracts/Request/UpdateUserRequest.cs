namespace AppDiv.CRVS.Application.Contracts.Request;
public class UpdateUserRequest
{
    public string Id { get; set; }
    public string PreferedLanguage { get; set; } = "oro";
    public Guid AddressId { get; set; }
    public string? UserImage { get; set; }
    public string FingerPrintApiUrl { get; set; } = "localhost";
    public UpdatePersonalInfoRequest PersonalInfo { get; set; }
}