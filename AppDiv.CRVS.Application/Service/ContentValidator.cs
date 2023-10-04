using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Update;
using AppDiv.CRVS.Application.Features.BirthEvents.Command.Update;
using AppDiv.CRVS.Application.Features.DeathEvents.Command.Update;
using AppDiv.CRVS.Application.Features.DivorceEvents.Command.Update;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Update;
using AppDiv.CRVS.Application.Features.User.Command.Update;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Service
{
    public class ContentValidator : IContentValidator
    {

        private readonly IMediator _mediator;

        public ContentValidator(IMediator mediator)
        {
            this._mediator = mediator;
           
        }
        public async Task<BaseResponse> ValidateAsync(string eventType, JObject content, bool IsUpdate = true)
        {
            var response = new BaseResponse();
            try
            {
                switch (eventType)
                {
                    case "Adoption":
                        UpdateAdoptionCommand adoptionCommand = content.ToObject<UpdateAdoptionCommand>();
                        adoptionCommand.IsFromCommand = true;
                        adoptionCommand.ValidateFirst = IsUpdate;
                        response = await _mediator.Send(adoptionCommand);
                        break;
                    case "Birth":
                        UpdateBirthEventCommand birthCommand = content.ToObject<UpdateBirthEventCommand>();
                        birthCommand.IsFromCommand = true;
                        birthCommand.ValidateFirst = IsUpdate;
                        response = await _mediator.Send(birthCommand);
                        break;
                    case "Death":
                        UpdateDeathEventCommand deathCommand = content.ToObject<UpdateDeathEventCommand>();
                        deathCommand.IsFromCommand = true;
                        deathCommand.ValidateFirst = IsUpdate;
                        response = await _mediator.Send(deathCommand);
                        break;
                    case "Divorce":
                        UpdateDivorceEventCommand divorceCommand = content.ToObject<UpdateDivorceEventCommand>();
                        divorceCommand.IsFromCommand = true;
                        divorceCommand.ValidateFirst = IsUpdate;
                        response = await _mediator.Send(divorceCommand);
                        break;
                    case "Marriage":
                        UpdateMarriageEventCommand marriageCommand = content.ToObject<UpdateMarriageEventCommand>();
                        marriageCommand.IsFromCommand = true;
                        marriageCommand.ValidateFirst = IsUpdate;
                        response = await _mediator.Send(marriageCommand);
                        break;
                    default:
                        throw new ArgumentException($"Invalid eventType: {eventType}");
                }
            }
            catch (System.Exception e)
            {
                response.BadRequest("Unable to parse the Content..." + e.Message);
                // throw;
            }
            return response;
        }
        public async Task<BaseResponse> ValidateUserDataAsync(ApplicationUser oldData, JObject content, bool IsUpdate = true)
        {
            var response = new BaseResponse();
            try
            {
                var newUserObj = content.ToObject<UpdateUserRequest>();
        

                UpdateUserCommand updateUserCommand = new UpdateUserCommand{
                    Id = oldData.Id,
                    UserName = oldData.UserName,
                    Status = oldData.Status,
                    Email = oldData.Email,
                    PreferedLanguage = newUserObj.PreferedLanguage,
                    AddressId = newUserObj.AddressId,
                    UserImage = newUserObj.UserImage,
                    UserGroups = oldData.UserGroups.Select(g => g.Id).ToList(),
                    SelectedAdminType = oldData.SelectedAdminType,
                    CanRegisterEvent = oldData.CanRegisterEvent,
                    FingerPrintApiUrl = oldData.FingerPrintApiUrl,
                    IsFromCommand = true,
                    ValidateFirst = !IsUpdate,
                    PersonalInfo = newUserObj.PersonalInfo

                };
                response = await _mediator.Send(updateUserCommand);
     
            }
            catch (System.Exception e)
            {
                response.BadRequest("Unable to parse the Content..." + e.Message);
                // throw;
            }
            return response;
        }
    }
}