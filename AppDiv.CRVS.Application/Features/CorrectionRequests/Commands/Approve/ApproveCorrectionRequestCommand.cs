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
        private readonly IWorkflowService _WorkflowService;
        public ApproveCorrectionRequestCommandHandler(ICorrectionRequestRepostory CorrectionRequestRepostory, IWorkflowService WorkflowService)
        {
            _CorrectionRequestRepostory = CorrectionRequestRepostory;
            _WorkflowService = WorkflowService;
        }
        public async Task<response> Handle(ApproveCorrectionRequestCommand request, CancellationToken cancellationToken)
        {
            var correctionRequestData = _CorrectionRequestRepostory.GetAll()
            .Include(x => x.Request)
            .Where(x => x.Id == request.Id).FirstOrDefault();

            if (correctionRequestData.Request.currentStep > 0 &&
         correctionRequestData.Request.currentStep <= (_WorkflowService.GetLastWorkflow("change") + 1))
            {
                if (request.IsApprove)
                {
                    correctionRequestData.Request.currentStep--;
                }
                else
                {
                    correctionRequestData.Request.currentStep++;
                }
            }
            else
            {
                throw new Exception("the Correction Request on idel state ");
            }
            try
            {
                await _CorrectionRequestRepostory.UpdateAsync(correctionRequestData, x => x.Id);
                await _CorrectionRequestRepostory.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var modifiedLookup = _CorrectionRequestRepostory.GetAll()
            .Where(x => x.Id == request.Id)
            .Include(x => x.Request).FirstOrDefault();
            var CorrectionRequestResponse = CustomMapper.Mapper.Map<AddCorrectionRequest>(modifiedLookup);

            var response = new response
            {
                data = CorrectionRequestResponse,
                Response = new BaseResponse
                {
                    Success = true,
                    Message = "Adoption",
                    Id = request.Id,
                    IsLast = correctionRequestData.Request.currentStep == 0
                }
            };

            return response;
        }
    }
}