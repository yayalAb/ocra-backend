
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddCourtRequest
    {
        public Guid? Id { get; set; }
        public virtual Guid? AddressId { get; set; }
        public LanguageModel Name { get; set; }
        public LanguageModel? Description { get; set; }
    }
}