using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs;
public class SettingDTO
{
    public Guid Id { get; set; }
    public string Key { get; set; }
    public JObject Value { get; set; }
}