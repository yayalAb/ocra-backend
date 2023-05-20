using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Application.Features.Search
{
    // Customer GetPaymentExamptionRequestQuery with  response
    public class GetPaymentExamptionRequestQuery : IRequest<object>
    {
        public string SearchString { get; set; }

    }

    public class GetPaymentExamptionRequestQueryHandler : IRequestHandler<GetPaymentExamptionRequestQuery, object>
    {
        private readonly IPaymentExamptionRequestRepository _paymentExamptionRequestRepository;

        public GetPaymentExamptionRequestQueryHandler(IPaymentExamptionRequestRepository paymentExamptionRequestRepository)
        {
            _paymentExamptionRequestRepository = paymentExamptionRequestRepository;
        }
        public async Task<object> Handle(GetPaymentExamptionRequestQuery request, CancellationToken cancellationToken)
        {
            var SelectedInfo = _paymentExamptionRequestRepository.GetAllQueryable().Where(model =>
                                             EF.Functions.Like(model.ReasonStr, $"%{request.SearchString}%")
                                            || EF.Functions.Like(model.ExamptedClientId, $"%{request.SearchString}%")
                                            || EF.Functions.Like(model.ExamptedClientFullName, $"%{request.SearchString}%")
                                            || EF.Functions.Like(model.ExamptedBy, $"%{request.SearchString}%")
                                            || EF.Functions.Like(model.CertificateType, $"%{request.SearchString}%")
                                            || EF.Functions.Like(model.ReasonStr, $"%{request.SearchString}%")   
                                            ).ToList();

                                     

            return SelectedInfo;
        }
    }
}