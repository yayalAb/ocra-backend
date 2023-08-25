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
      private readonly IPersonDuplicateRepository _personDuplicationRepo;
      private readonly IUserResolverService _userResolver;
        public FingerprintService(IUserResolverService userResolver,IPersonDuplicateRepository personDuplicationRepo,IRequestApiService requestApiService)
        {
          _requestApiService=requestApiService;
          _personDuplicationRepo=personDuplicationRepo;
          _userResolver=userResolver;
        }

        public async Task<BaseResponse>  RegisterfingerPrintService(Dictionary<string, List<BiometricImagesAtt>?> fingerPrint, CancellationToken cancellationToken)
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
             var ApiResponse = JsonSerializer.Deserialize<IdentifyFingerDuplicationDto>(responseBody);
                    if (ApiResponse.operationResult == "MATCH_FOUND")
                    { 
                    //     var Duplcated=new PersonDuplicate{
                    //         OldPersonId=new Guid(ApiResponse.bestResult.id),
                    //         NewPersonId=new Guid(Create.registrationID),
                    //         FoundWith="Fingerprint",
                    //         CorrectedBy =_userResolver.GetUserId()
                    //   };
                    //  await _personDuplicationRepo.InsertAsync(Duplcated,cancellationToken);
                      return new BaseResponse{
                        Message="Duplicated Fingerprint",
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