using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
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