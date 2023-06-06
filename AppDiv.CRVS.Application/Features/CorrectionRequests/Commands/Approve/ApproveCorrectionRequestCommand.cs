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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.CorrectionRequests.Commands.Approve
{
    public class response
    {
        public AddCorrectionRequest data { get; set; }
        public BaseResponse Response { get; set; }
    }
    // Customer create command with CustomerResponse
    public class ApproveCorrectionRequestCommand : IRequest<response>
    {
        public Guid Id { get; set; }
        public bool IsApprove { get; set; } = false;
    }
    public class ApproveCorrectionRequestCommandHandler : IRequestHandler<ApproveCorrectionRequestCommand, response>
    {
        private readonly ICorrectionRequestRepostory _CorrectionRequestRepostory;
        private readonly IEventRepository _eventRepostory;
        private readonly IWorkflowService _WorkflowService;
        public ApproveCorrectionRequestCommandHandler(IEventRepository eventRepostory, ICorrectionRequestRepostory CorrectionRequestRepostory, IWorkflowService WorkflowService)
        {
            _CorrectionRequestRepostory = CorrectionRequestRepostory;
            _WorkflowService = WorkflowService;
            _eventRepostory = eventRepostory;
        }
        public async Task<response> Handle(ApproveCorrectionRequestCommand request, CancellationToken cancellationToken)
        {
            var response = await _WorkflowService.ApproveService(request.Id, "change", request.IsApprove, cancellationToken);
            string eventtype = "";
            if (response.Item1)
            {
                var EventType = await _eventRepostory.GetAsync(response.Item2);
                eventtype = EventType.EventType;
            }
            var modifiedLookup = _CorrectionRequestRepostory.GetAll()
            .Where(x => x.Id == request.Id)
            .Include(x => x.Request).FirstOrDefault();
            var CorrectionRequestResponse = CustomMapper.Mapper.Map<AddCorrectionRequest>(modifiedLookup);
            var response1 = new response
            {
                data = CorrectionRequestResponse,
                Response = new BaseResponse
                {
                    Success = true,
                    Message = eventtype,
                    Id = request.Id,
                    IsLast = response.Item1
                }
            };
            return response1;
        }
    }
}