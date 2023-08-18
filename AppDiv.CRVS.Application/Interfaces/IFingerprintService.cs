using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Models;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IFingerprintService
    {
     public Task<object> RegisterfingerPrintService(Dictionary<string, List<BiometricImagesAtt>?> fingerPrint);
   
    }
}