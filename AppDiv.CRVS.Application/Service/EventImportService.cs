using Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Features.DeathEvents.Command.Update;
using AppDiv.CRVS.Application.Interfaces;
using MediatR;
using Newtonsoft.Json.Linq;
using AppDiv.CRVS.Application.Features.DeathEvents.Command.Create;
using AppDiv.CRVS.Application.Features.BirthEvents.Command.Create;
using AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Create;
using AppDiv.CRVS.Application.Features.DivorceEvents.Command.Create;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Create;
using Newtonsoft.Json;

namespace AppDiv.CRVS.Application.Service
{
    public class EventImportService : IEventImportService
    {
        private readonly IMediator _mediator;
        public EventImportService(IMediator mediator)
        {
            this._mediator = mediator;
        }
        public async Task<object> ImportEvent(JObject[] eventObj)
        {
            BaseResponse response = new BaseResponse();
            string eventType = "";
            JObject[] ResponseObject = null;

            foreach (JObject item in eventObj)
            {

                eventType = (string)item["eventType"];
                Console.WriteLine("Event Type : {0}", eventType);
                try
                {
                    switch (eventType.ToLower())
                    {
                        case "Birth":
                            var newBirthObject = new
                            {
                                birthEvent = item
                            };
                            JObject birthEvent = JObject.FromObject(newBirthObject);
                            CreateBirthEventCommand birthCommand = item.ToObject<CreateBirthEventCommand>();
                            response = await _mediator.Send(birthCommand);
                            break;
                        case "adoption":
                            var newAdoptionObject = new
                            {
                                adoption = item
                            };
                            JObject adoption = JObject.FromObject(newAdoptionObject);
                            CreateAdoptionCommand adoptionCommand = item.ToObject<CreateAdoptionCommand>();
                            response = await _mediator.Send(adoptionCommand);
                            break;
                        case "divorce":
                            CreateDivorceEventCommand divorceCommand = item.ToObject<CreateDivorceEventCommand>();
                            response = await _mediator.Send(divorceCommand); break;
                        case "death":
                            var newDeathObject = new
                            {
                                deathEvent = item
                            };
                            JObject deathEvent = JObject.FromObject(newDeathObject);
                            Console.WriteLine(deathEvent);
                            CreateDeathEventCommand deathCommand = deathEvent.ToObject<CreateDeathEventCommand>();
                            response = await _mediator.Send(deathCommand);
                            break;
                        case "marriage":
                            CreateMarriageEventCommand marriageCommand = item.ToObject<CreateMarriageEventCommand>();
                            response = await _mediator.Send(marriageCommand); break;
                    }

                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("exception : {0}", ex);
                    throw;
                }

            }

            return response;
        }

    }
}