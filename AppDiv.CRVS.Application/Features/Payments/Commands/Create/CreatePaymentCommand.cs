
using MediatR;

namespace AppDiv.CRVS.Application.Features.Payments.Command.Create
{
    // Customer create command with CustomerResponse
    public record CreatePaymentCommand : IRequest<CreatePaymentCommandResponse>
    {
        public Guid PaymentRequestId { get; set; }
        public Guid PaymentWayLookupId { get; set; }
        public string BillNumber { get; set; }
    }
}