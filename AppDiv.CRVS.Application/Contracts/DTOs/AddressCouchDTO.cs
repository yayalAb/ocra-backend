using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


using System.ComponentModel.DataAnnotations.Schema;


namespace AppDiv.CRVS.Application.Contracts.DTOs;
public class AddressCouchDTO
{
    public Guid Id { get; set; }
    public int AdminLevel { get; set; }
    public string AddressNameStr { get; set; }
    public bool Status { get; set; } = false;
    public Guid? OldAddressId { get; set; }
    public Guid? ParentAddressId { get; set; }
    [NotMapped]
    public JObject AddressName
    {
        get
        {
            return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(AddressNameStr) ? "{}" : AddressNameStr);
        }
        set
        {
            AddressNameStr = value.ToString();
        }
    }
    public virtual AddressCouchDTO ParentAddress { get; set; }
    public virtual ICollection<AddressCouchDTO> ChildAddresses { get; set; }
    public virtual LookupCouchDTO AdminTypeLookup { get; set; }
    
}