namespace AppDiv.CRVS.Application.Contracts.DTOs;
public class SettingDTO
{
    public Guid Id{ get; set; }
    public string Key { get; set; }
    public Json Value { get; set; } = true;
}