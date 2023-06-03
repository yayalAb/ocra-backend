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

namespace AppDiv.CRVS.Application.Features.CorrectionRequests.Commands.Approve
{
    // Customer create command with CustomerResponse
    public class ApproveCorrectionRequestCommand : IRequest<AddCorrectionRequest>
    {
        public Guid Id { get; set; }
        public JObject? Description { get; set; }
        public JObject Content { get; set; }
    }
    public class ApproveCorrectionRequestCommandHandler : IRequestHandler<ApproveCorrectionRequestCommand, AddCorrectionRequest>
    {
        private readonly ICorrectionRequestRepostory _CorrectionRequestRepostory;
        public ApproveCorrectionRequestCommandHandler(ICorrectionRequestRepostory CorrectionRequestRepostory)
        {
            _CorrectionRequestRepostory = CorrectionRequestRepostory;
        }
        public async Task<AddCorrectionRequest> Handle(ApproveCorrectionRequestCommand request, CancellationToken cancellationToken)
        {
            var correctionRequestData = await _CorrectionRequestRepostory.GetAsync(request.Id);
            correctionRequestData.Description = request.Content;
            correctionRequestData.Content = request.Description;
            correctionRequestData.currentStep = +1;
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
            var LookupResponse = CustomMapper.Mapper.Map<AddCorrectionRequest>(modifiedLookup);
            return LookupResponse;
        }
    }
}