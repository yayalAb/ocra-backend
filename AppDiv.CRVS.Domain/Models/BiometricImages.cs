

namespace AppDiv.CRVS.Domain.Models
{
    public class BiometricImagesAtt
    {
        public int position { get; set; }
        public string base64Image { get; set; }

    }
    public class BiometricImages
    {
        public List<BiometricImagesAtt> fingerprint { get; set; } = new List<BiometricImagesAtt>();
        public List<BiometricImagesAtt?> face {get;set;} = new List<BiometricImagesAtt>();

    }
}