using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using MediatR;


namespace AppDiv.CRVS.Application.Features.PaymentExamptionRequests.Query
{
    // Customer query with List<Customer> response
    public record GetAllPaymentExamptionRequestQuery : IRequest<PaginatedList<PaymentExamptionRequestGridDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetAllPaymentExamptionRequestHandler : IRequestHandler<GetAllPaymentExamptionRequestQuery, PaginatedList<PaymentExamptionRequestGridDTO>>
    {
        private readonly IPaymentExamptionRequestRepository _PaymentExamptionRequestRepository;

        public GetAllPaymentExamptionRequestHandler(IPaymentExamptionRequestRepository PaymentExamptionRequestQueryRepository)
        {
            _PaymentExamptionRequestRepository = PaymentExamptionRequestQueryRepository;
        }
        public async Task<PaginatedList<PaymentExamptionRequestGridDTO>> Handle(GetAllPaymentExamptionRequestQuery request, CancellationToken cancellationToken)
        {
            return await PaginatedList<PaymentExamptionRequestGridDTO>
                            .CreateAsync(
                                _PaymentExamptionRequestRepository.GetAll().Select(r => new PaymentExamptionRequestGridDTO
                                {
                                    Id = r.Id,
                                    Reason = r.ReasonLang,
                                    ExamptedClientId = r.ExamptedClientId,
                                    ExamptedClientFullName = r.ExamptedClientFullName,
                                    // ExamptedBy = r.ExamptedBy,
                                    NumberOfClient = r.NumberOfClient,
                                    CertificateType = r.CertificateType,
                                    Address = CustomMapper.Mapper.Map<AddressDTO>(r.Address)
                                    // Description = g.Description.Value<string>("eng")
                                })
                                , request.PageCount ?? 1, request.PageSize ?? 10);
            // var PaymentExamptionRequestResponse = CustomMapper.Mapper.Map<List<PaymentExamptionRequestDTO>>(PaymentExamptionRequestList);
            // return PaymentExamptionRequestResponse;
        }
    }
}