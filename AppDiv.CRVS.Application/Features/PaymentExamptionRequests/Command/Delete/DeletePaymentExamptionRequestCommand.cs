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
    // Customer create command with string response
    public class DeletePaymentExamptionRequestCommand : IRequest<String>
    {
        public Guid Id { get; private set; }

        public DeletePaymentExamptionRequestCommand(Guid Id)
        {
            this.Id = Id;
        }
    }

    // Customer delete command handler with string response as output
    public class DeletePaymentExamptionRequestCommmandHandler : IRequestHandler<DeletePaymentExamptionRequestCommand, String>
    {
        private readonly IPaymentExamptionRequestRepository _PaymentExamptionRequestRepository;
        public DeletePaymentExamptionRequestCommmandHandler(IPaymentExamptionRequestRepository PaymentExamptionRequestRepository)
        {
            _PaymentExamptionRequestRepository = PaymentExamptionRequestRepository;
        }

        public async Task<string> Handle(DeletePaymentExamptionRequestCommand request, CancellationToken cancellationToken)
        {
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
                    return "There is no PaymentExamptionRequest with the specified id";
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Payment Examption Request information has been deleted!";
        }
    }
}