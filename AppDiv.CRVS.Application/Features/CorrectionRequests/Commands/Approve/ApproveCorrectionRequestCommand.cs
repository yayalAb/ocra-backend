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
        public Guid? ReasonLookupId { get; set; }
    }
    public class ApproveCorrectionRequestCommandHandler : IRequestHandler<ApproveCorrectionRequestCommand, BaseResponse>
    {
        private readonly ICorrectionRequestRepostory _CorrectionRequestRepostory;
        // private readonly IEventRepository _eventRepostory;
        private readonly IWorkflowService _WorkflowService;
        private readonly IUserResolverService _UserResolverService;
        private readonly IUserRepository _UserRepository;

        private readonly IMediator _mediator;


        public ApproveCorrectionRequestCommandHandler(IUserRepository UserRepository, IUserResolverService UserResolverService, IMediator mediator, ICorrectionRequestRepostory CorrectionRequestRepostory, IWorkflowService WorkflowService)
        {
            _CorrectionRequestRepostory = CorrectionRequestRepostory;
            _WorkflowService = WorkflowService;
            _UserResolverService = UserResolverService;
            _UserRepository = UserRepository;
            _mediator = mediator;
        }
        public async Task<BaseResponse> Handle(ApproveCorrectionRequestCommand request, CancellationToken cancellationToken)
        {
            // string userId = _UserResolverService.GetUserId();
            // if (String.IsNullOrEmpty(userId))
            // {
            //     throw new Exception("User Not Found/unauthorized access ");
            // }
            // var userGroups = _UserRepository.GetAll()
            // .Include(x => x.UserGroups)
            // .Where(x => x.Id == userId).Select(x => x.UserGroups.Select(n => n.Id)).ToList();
            // if (userGroups == null)
            // {
            //     throw new Exception("User Group Not Found/unauthorized access ");
            // }

            try

            {
                var response = await _WorkflowService.ApproveService(request.Id, "change", request.IsApprove, request.Comment, request.ReasonLookupId, false, cancellationToken);
                if (response.Item1)
                {
                    var modifiedEvent = _CorrectionRequestRepostory.GetAll()
                   .Include(x => x.Event)
                   .AsNoTracking()
                   .Where(x => x.RequestId == request.Id)
                   .Include(x => x.Request).FirstOrDefault();
                    var CorrectionRequestResponse = CustomMapper.Mapper.Map<AddCorrectionRequest>(modifiedEvent);
                    if (modifiedEvent.Event.EventType == "Adoption")
                    {
                        UpdateAdoptionCommand AdoptionCommand = CorrectionRequestResponse.Content.ToObject<UpdateAdoptionCommand>();
                        AdoptionCommand.IsFromCommand = true;
                        var response1 = await _mediator.Send(AdoptionCommand);
                        // await _eventRepostory.SaveChangesAsync(cancellationToken);
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
                        // await _eventRepostory.SaveChangesAsync(cancellationToken);
                    }
                    else if (modifiedEvent.Event.EventType == "Marriage")
                    {
                        UpdateMarriageEventCommand MarriageCommand = CorrectionRequestResponse.Content.ToObject<UpdateMarriageEventCommand>();
                        MarriageCommand.IsFromCommand = true;
                        var response1 = await _mediator.Send(MarriageCommand);
                        // await _eventRepostory.SaveChangesAsync(cancellationToken);
                    }
                    // await _eventRepostory.SaveChangesAsync(cancellationToken);
                }
                var Response = new BaseResponse
                {
                    Success = true,
                    Message = request.IsApprove ? "Correction Request Approved Successfuly" : "Correction Request Rejected Successfuly",
                };
                // await transaction.CommitAsync();
                return Response;
            }
            catch (Exception)
            {
                // await transaction.RollbackAsync();
                throw;
            }
            //     }

            // });
        }
    }
}