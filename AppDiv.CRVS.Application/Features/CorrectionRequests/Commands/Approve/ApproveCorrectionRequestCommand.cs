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
        public JObject? Description { get; set; }
    }
    public class ApproveCorrectionRequestCommandHandler : IRequestHandler<ApproveCorrectionRequestCommand, response>
    {
        private readonly ICorrectionRequestRepostory _CorrectionRequestRepostory;
        public ApproveCorrectionRequestCommandHandler(ICorrectionRequestRepostory CorrectionRequestRepostory)
        {
            _CorrectionRequestRepostory = CorrectionRequestRepostory;
        }
        public async Task<response> Handle(ApproveCorrectionRequestCommand request, CancellationToken cancellationToken)
        {
            var isLastStep = false;
            var correctionRequestData = await _CorrectionRequestRepostory.GetAsync(request.Id);
            correctionRequestData.Content = request.Description;
            correctionRequestData.Request.currentStep = +1;
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
                    Message = "Sucessfully Approved",
                    Id = request.Id,
                    IsLast = isLastStep
                }
            };

            return response;
        }
    }
}