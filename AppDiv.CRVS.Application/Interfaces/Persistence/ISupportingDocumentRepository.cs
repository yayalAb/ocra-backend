using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface ISupportingDocumentRepository : IBaseRepository<SupportingDocument>
    {
        (string BrideImage, string GroomImage) MarriageImage();

    }
}