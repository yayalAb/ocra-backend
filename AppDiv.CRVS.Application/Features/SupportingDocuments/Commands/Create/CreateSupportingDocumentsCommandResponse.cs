using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces.Persistence;

namespace AppDiv.CRVS.Application.Features.SupportingDocuments.Commands.Create
{
    public class CreateSupportingDocumentsCommandResponse :BaseResponse
    {
        public CreateSupportingDocumentsCommandResponse(ISupportingDocumentRepository supportingDocumentRepo):base()
        {
            
        }
    }
}