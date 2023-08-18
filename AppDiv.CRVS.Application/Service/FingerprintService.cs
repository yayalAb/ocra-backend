using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Models;
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace AppDiv.CRVS.Application.Service
{
    public class FingerprintService: IFingerprintService
    {

      private readonly IRequestApiService _requestApiService;
        public FingerprintService(IRequestApiService requestApiService)
        {
          _requestApiService=requestApiService;
        }

        public async Task<object>  RegisterfingerPrintService(Dictionary<string, List<BiometricImagesAtt>?> fingerPrint)
        {

           foreach (var item in fingerPrint)
           {

             var Create = new FingerPrintApiRequestDto
                    {
                        registrationID = item.Key,
                        images = new BiometricImages {
                              fingerprint= item.Value.Select(x=> new BiometricImagesAtt{
                                        position=x.position,
                                        base64Image=x.base64Image
                                                } ).ToList()
                                              }

                    };
           await _requestApiService.post("Register", Create); 
           }
           return "";

        }       
    }
}