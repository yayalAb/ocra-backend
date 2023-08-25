using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
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

        public async Task<BaseResponse>  RegisterfingerPrintService(Dictionary<string, List<BiometricImagesAtt>?> fingerPrint)
        {
          

           foreach (var item in fingerPrint)
           {
             var Create = new FingerPrintApiRequestDto
                    {
                        registrationID = item.Key,
                        images = new BiometricImages {
                              fingerprint= item.Value.Select(x=> new BiometricImagesAtt{
                                        position=x.position,
                                        base64Image=x.base64Image.Replace("data:image/jpeg;base64,", "")
                                                } ).ToList()
                                              }
                                };
              var responseBody= await _requestApiService.post("Register", Create);
             var ApiResponse = JsonSerializer.Deserialize<FingerPrintResponseDto>(responseBody);
                    if (ApiResponse.operationResult == "MATCH_FOUND")
                    { 
                      return new BaseResponse{
                        Message="Duplicated Finger print",
                        Success=false,
                        Status=204
                        
                      };
                    }
          
           }
          return new BaseResponse{
                        Message="Successfuly Registerd",
                        Success=true,
                        Status=200
                      };

        }       
    }
}