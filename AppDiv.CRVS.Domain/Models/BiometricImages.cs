

namespace AppDiv.CRVS.Domain.Models
{
    public class BiometricImagesAtt
    {
        public int position { get; set; }
        public string base64Image { get; set; }

    }
    public class BiometricImages
    {
        public BiometricImagesAtt[] fingerprint { get; set; }

    }
}