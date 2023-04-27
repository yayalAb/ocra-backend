using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.PaymentRates.Command.Delete
{
    // Customer create command with string response
    public class DeletePaymentRateCommand : IRequest<String>
    {
        public Guid Id { get; private set; }

        public DeletePaymentRateCommand(Guid Id)
        {
            this.Id = Id;
        }
    }

    // Customer delete command handler with string response as output
    public class DeletePaymentRateCommmandHandler : IRequestHandler<DeletePaymentRateCommand, String>
    {
        private readonly IPaymentRateRepository _paymentRateRepository;
        public DeletePaymentRateCommmandHandler(IPaymentRateRepository paymentRateRepository)
        {
            _paymentRateRepository = paymentRateRepository;
        }

        public async Task<string> Handle(DeletePaymentRateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var paymentRateEntity = await _paymentRateRepository.GetByIdAsync(request.Id);
                if (paymentRateEntity != null)
                {
                    await _paymentRateRepository.DeleteAsync(request.Id);
                    await _paymentRateRepository.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    return "There is no payment rate with the specified id";
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Payment Rate information has been deleted!";
        }
    }
}