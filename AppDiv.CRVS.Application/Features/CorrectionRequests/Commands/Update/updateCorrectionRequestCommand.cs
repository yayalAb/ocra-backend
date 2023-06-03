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

namespace AppDiv.CRVS.Application.Features.CorrectionRequests.Commands.Update
{
    // Customer create command with CustomerResponse
    public class updateCorrectionRequestCommand : IRequest<AddCorrectionRequest>
    {
        public Guid Id { get; set; }
        public JObject? Description { get; set; }
        public JObject Content { get; set; }
    }
    public class updateCorrectionRequestCommandHandler : IRequestHandler<updateCorrectionRequestCommand, AddCorrectionRequest>
    {
        private readonly ICorrectionRequestRepostory _CorrectionRequestRepostory;
        public updateCorrectionRequestCommandHandler(ICorrectionRequestRepostory CorrectionRequestRepostory)
        {
            _CorrectionRequestRepostory = CorrectionRequestRepostory;
        }
        public async Task<AddCorrectionRequest> Handle(updateCorrectionRequestCommand request, CancellationToken cancellationToken)
        {
            var correctionRequestData = await _CorrectionRequestRepostory.GetAsync(request.Id);
            if (correctionRequestData.currentStep != 0)
            {
                throw new Exception("you can not edit this request it is Approved");
            }
            correctionRequestData.Description = request.Content;
            correctionRequestData.Content = request.Description;
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