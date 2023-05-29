using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.PaymentRates.Command.Update
{
    // Customer create command with CustomerResponse
    public class UpdatePaymentRateCommand : IRequest<PaymentRateDTO>
    {

        public Guid Id { get; set; }
        public Guid PaymentTypeLookupId { get; set; }
        public Guid EventLookupId { get; set; }
        public bool IsForeign { get; set; }
        public float Amount { get; set; }
        public bool Status { get; set; }
    }

    public class UpdatePaymentRateCommandHandler : IRequestHandler<UpdatePaymentRateCommand, PaymentRateDTO>
    {
        private readonly IPaymentRateRepository _paymentRateRepository;
        public UpdatePaymentRateCommandHandler(IPaymentRateRepository paymentRateRepository)
        {
            _paymentRateRepository = paymentRateRepository;
        }
        public async Task<PaymentRateDTO> Handle(UpdatePaymentRateCommand request, CancellationToken cancellationToken)
        {
            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request);

            var paymentRate = new PaymentRate()
            {
                Id = request.Id,
                PaymentTypeLookupId = request.PaymentTypeLookupId,
                EventLookupId = request.EventLookupId,
                IsForeign = request.IsForeign,
                Amount = request.Amount,
                Status = request.Status,
                ModifiedAt = DateTime.Now
            };


            try
            {
                await _paymentRateRepository.UpdateAsync(paymentRate, x => x.Id);
                var result = await _paymentRateRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            var modifiedPaymentRate = await _paymentRateRepository.GetByIdAsync(request.Id);
            var paymentRateResponse = CustomMapper.Mapper.Map<PaymentRateDTO>(modifiedPaymentRate);

            return paymentRateResponse;
        }
    }
}