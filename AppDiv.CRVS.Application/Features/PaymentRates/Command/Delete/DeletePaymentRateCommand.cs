﻿using AppDiv.CRVS.Application.Common;
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
    public class DeletePaymentRateCommand : IRequest<BaseResponse>
    {
        public Guid[] Ids { get; set; }


    }

    // Customer delete command handler with string response as output
    public class DeletePaymentRateCommmandHandler : IRequestHandler<DeletePaymentRateCommand, BaseResponse>
    {
        private readonly IPaymentRateRepository _paymentRateRepository;
        public DeletePaymentRateCommmandHandler(IPaymentRateRepository paymentRateRepository)
        {
            _paymentRateRepository = paymentRateRepository;
        }

        public async Task<BaseResponse> Handle(DeletePaymentRateCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse();
            try
            {
                if (request.Ids != null && request.Ids.Length > 0)
                {
                    foreach (var item in request.Ids)
                    {
                        await _paymentRateRepository.DeleteAsync(item);

                    }
                    await _paymentRateRepository.SaveChangesAsync(cancellationToken);
                    response.Deleted("Payment rate");
                }
                else
                {
                    response.BadRequest("There is no payment rate with the specified id");
                }

            }
            catch (Exception exp)
            {
                response.BadRequest("Unable to delete the payment rate.");
            }
            return response;
        }
    }
}