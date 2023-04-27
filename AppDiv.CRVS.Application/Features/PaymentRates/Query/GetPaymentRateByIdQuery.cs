using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Customers.Query
{
    // Customer GetCustomerByIdQuery with Customer response
    public class GetPaymentRateByIdQuery : IRequest<PaymentRateDTO>
    {
        public Guid Id { get; private set; }

        public GetPaymentRateByIdQuery(Guid Id)
        {
            this.Id = Id;
        }

    }

    public class GetPaymentRateByIdHandler : IRequestHandler<GetPaymentRateByIdQuery, PaymentRateDTO>
    {
        private readonly IPaymentRateRepository _paymentRateRepository;

        public GetPaymentRateByIdHandler(IPaymentRateRepository paymentRateRepository)
        {
            _paymentRateRepository = paymentRateRepository;
        }
        public async Task<PaymentRateDTO> Handle(GetPaymentRateByIdQuery request, CancellationToken cancellationToken)
        {
            // var customers = await _mediator.Send(new GetAllCustomerQuery());
            var explicitLoadedProperties = new Dictionary<string, Utility.Contracts.NavigationPropertyType>
                                                {
                                                    { "PaymentTypeLookup", NavigationPropertyType.REFERENCE },
                                                    { "EventLookup", NavigationPropertyType.REFERENCE },
                                                    { "Address", NavigationPropertyType.REFERENCE }

                                                };
            var selectedPaymentRate = await _paymentRateRepository.GetWithAsync(request.Id, explicitLoadedProperties);
            return CustomMapper.Mapper.Map<PaymentRateDTO>(selectedPaymentRate);
            // return selectedCustomer;
        }
    }
}