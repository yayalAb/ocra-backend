using System;
using System.Collections.Generic;
using System.Linq;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MediatR;
using System;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Update;
using Newtonsoft.Json.Linq;
using AppDiv.CRVS.Application.Features.BirthEvents.Command.Update;
using AppDiv.CRVS.Application.Features.DeathEvents.Command.Update;
using AppDiv.CRVS.Application.Features.DivorceEvents.Command.Update;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Update;

namespace AppDiv.CRVS.Application.Features.CorrectionRequests.Commands.Approve
{

    // Customer create command with CustomerResponse
    public class ApproveCorrectionRequestCommand : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }
        public string? Comment { get; set; }
        public bool IsApprove { get; set; } = false;
    }
    public class ApproveCorrectionRequestCommandHandler : IRequestHandler<ApproveCorrectionRequestCommand, BaseResponse>
    {
        private readonly ICorrectionRequestRepostory _CorrectionRequestRepostory;
        private readonly IEventRepository _eventRepostory;
        private readonly IWorkflowService _WorkflowService;

        private readonly IMediator _mediator;


        public ApproveCorrectionRequestCommandHandler(IMediator mediator, IEventRepository eventRepostory, ICorrectionRequestRepostory CorrectionRequestRepostory, IWorkflowService WorkflowService)
        {
            _CorrectionRequestRepostory = CorrectionRequestRepostory;
            _WorkflowService = WorkflowService;
            _eventRepostory = eventRepostory;
            _mediator = mediator;
        }
        public async Task<BaseResponse> Handle(ApproveCorrectionRequestCommand request, CancellationToken cancellationToken)
        {
            var response = await _WorkflowService.ApproveService(request.Id, "change", request.IsApprove, request.Comment, false, cancellationToken);
            if (response.Item1)
            {


                var modifiedEvent = _CorrectionRequestRepostory.GetAll()
                .Include(x => x.Event)
                .Where(x => x.RequestId == request.Id)
                .Include(x => x.Request).FirstOrDefault();
                var CorrectionRequestResponse = CustomMapper.Mapper.Map<AddCorrectionRequest>(modifiedEvent);
                if (modifiedEvent.Event.EventType == "Adoption")
                {
                    UpdateAdoptionCommand AdoptionCommand = CorrectionRequestResponse.Content.ToObject<UpdateAdoptionCommand>();
                    AdoptionCommand.IsFromCommand = true;
                    var response1 = await _mediator.Send(AdoptionCommand);
                }
                else if (modifiedEvent.Event.EventType == "Birth")
                {
                    UpdateBirthEventCommand BirthCommand = CorrectionRequestResponse.Content.ToObject<UpdateBirthEventCommand>();
                    BirthCommand.IsFromCommand = true;
                    var response1 = await _mediator.Send(BirthCommand);
                }
                else if (modifiedEvent.Event.EventType == "Death")
                {
                    UpdateDeathEventCommand DeathCommand = CorrectionRequestResponse.Content.ToObject<UpdateDeathEventCommand>();
                    DeathCommand.IsFromCommand = true;
                    var response1 = await _mediator.Send(DeathCommand);
                }
                else if (modifiedEvent.Event.EventType == "Divorce")
                {
                    UpdateDivorceEventCommand DivorceCommand = CorrectionRequestResponse.Content.ToObject<UpdateDivorceEventCommand>();
                    DivorceCommand.IsFromCommand = true;
                    var response1 = await _mediator.Send(DivorceCommand);
                }
                else if (modifiedEvent.Event.EventType == "Marriage")
                {
                    UpdateMarriageEventCommand MarriageCommand = CorrectionRequestResponse.Content.ToObject<UpdateMarriageEventCommand>();
                    MarriageCommand.IsFromCommand = true;
                    var response1 = await _mediator.Send(MarriageCommand);
                }
                // await _eventRepostory.SaveChangesAsync(cancellationToken);
            }
            var Response = new BaseResponse
            {
                Success = true,
                Message = request.IsApprove ? "Correction Request Approved Successfuly" : "Correction Request Rejected Successfuly",
            };
            return Response;
        }
    }
}