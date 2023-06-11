using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;


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
            var correctionRequestData = _CorrectionRequestRepostory.GetAll()
            .Include(x => x.Request).Where(x => x.Id == request.Id).FirstOrDefault();
            if (correctionRequestData.Request.currentStep != 0)
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
            var modifiedCorrectionRequest = _CorrectionRequestRepostory.GetAll()
            .Where(x => x.Id == request.Id)
            .Include(x => x.Request).FirstOrDefault();
            var CorrectionRequestResponse = CustomMapper.Mapper.Map<AddCorrectionRequest>(modifiedCorrectionRequest);
            return CorrectionRequestResponse;
        }
    }
}