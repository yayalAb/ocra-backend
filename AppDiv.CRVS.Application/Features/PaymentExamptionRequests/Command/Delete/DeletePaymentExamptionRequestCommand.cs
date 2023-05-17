using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.PaymentExamptionRequests.Command.Delete
{
    // Customer create command with BaseResponse response
    public class DeletePaymentExamptionRequestCommand : IRequest<BaseResponse>
    {
        public Guid Id { get; private set; }

        public DeletePaymentExamptionRequestCommand(Guid Id)
        {
            this.Id = Id;
        }
    }

    // Customer delete command handler with BaseResponse response as output
    public class DeletePaymentExamptionRequestCommmandHandler : IRequestHandler<DeletePaymentExamptionRequestCommand, BaseResponse>
    {

        private readonly IPaymentExamptionRequestRepository _PaymentExamptionRequestRepository;
        public DeletePaymentExamptionRequestCommmandHandler(IPaymentExamptionRequestRepository PaymentExamptionRequestRepository)
        {
            _PaymentExamptionRequestRepository = PaymentExamptionRequestRepository;
        }

        public async Task<BaseResponse> Handle(DeletePaymentExamptionRequestCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse();
            try
            {
                var PaymentExamptionRequestEntity = await _PaymentExamptionRequestRepository.GetAsync(request.Id);
                if (PaymentExamptionRequestEntity != null)
                {
                    await _PaymentExamptionRequestRepository.DeleteAsync(request.Id);
                    await _PaymentExamptionRequestRepository.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    response.Success = false;
                    response.Message = "There is no PaymentExamptionRequest with the specified id";
                }


            }
            catch (Exception exp)
            {
                response.Success = false;
                response.Message = exp.Message;
                throw (new ApplicationException(exp.Message));
            }
            response.Success = true;
            response.Message = "Payment Examption Request information has been deleted!";

            return response;
        }
    }
}