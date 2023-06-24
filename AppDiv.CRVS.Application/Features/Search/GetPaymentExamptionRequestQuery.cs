using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace AppDiv.CRVS.Application.Features.Search
{
    // Customer GetPaymentExamptionRequestQuery with  response
    public class GetPaymentExamptionRequestQuery : IRequest<object>
    {
        public string? SearchString { get; set; }

    }

    public class GetPaymentExamptionRequestQueryHandler : IRequestHandler<GetPaymentExamptionRequestQuery, object>
    {
        private readonly IPaymentExamptionRequestRepository _paymentExamptionRequestRepository;

        public GetPaymentExamptionRequestQueryHandler(IPaymentExamptionRequestRepository paymentExamptionRequestRepository)
        {
            _paymentExamptionRequestRepository = paymentExamptionRequestRepository;
        }
        public Task<object> Handle(GetPaymentExamptionRequestQuery request, CancellationToken cancellationToken)
        {
            var SelectedInfo = _paymentExamptionRequestRepository.GetAllQueryable()
                                        .Where(model => model.status)
                                        .Where(model =>
                                            EF.Functions.Like(model.Id.ToString(), $"%{request.SearchString}%")
                                            || EF.Functions.Like(model.ReasonStr, $"%{request.SearchString}%")
                                            || EF.Functions.Like(model.ExamptedClientId, $"%{request.SearchString}%")
                                            || EF.Functions.Like(model.ExamptedClientFullName, $"%{request.SearchString}%")
                                            // || EF.Functions.Like(model.ExamptedBy, $"%{request.SearchString}%")
                                            || EF.Functions.Like(model.CertificateType, $"%{request.SearchString}%")
                                            || EF.Functions.Like(model.ReasonStr, $"%{request.SearchString}%")
                                            || EF.Functions.Like(model.Address.AddressNameStr, $"%{request.SearchString}%")
                                            ).Select(pe => new
                                            {
                                                Id = pe.Id,
                                                Reason = pe.ReasonLang,
                                                ExamptedClientId = pe.ExamptedClientId,
                                                ExamptedClientFullName = pe.ExamptedClientFullName,
                                                ExamptedDate = pe.ExamptedDate,
                                                // ExamptedBy = pe.ExamptedBy,
                                                NumberOfClient = pe.NumberOfClient,
                                                // AddressId = pe.AddressId,
                                                // AddressName = pe.Address.AddressNameLang,
                                                CertificateType = pe.CertificateType
                                            })
                                            .Take(50);
            return Task.FromResult<object>(SelectedInfo);
        }
    }
}