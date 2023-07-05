namespace AppDiv.CRVS.Domain.Entities
{
    public class LanguageModel
    {
        public string? en { get; set; }
        public string? am { get; set; }
        public string? or { get; set; }
        
        public override string ToString()
        {
            return $"{en}, {am}, {or}";
        }
    }
}