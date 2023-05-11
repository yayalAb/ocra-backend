using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;


namespace AppDiv.CRVS.Application.Features.PaymentExamptionRequests.Query
{
    // Customer query with List<Customer> response
    public record GetAllPaymentExamptionRequestQuery : IRequest<PaginatedList<PaymentExamptionRequestDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetAllPaymentExamptionRequestHandler : IRequestHandler<GetAllPaymentExamptionRequestQuery, PaginatedList<PaymentExamptionRequestDTO>>
    {
        private readonly IPaymentExamptionRequestRepository _PaymentExamptionRequestRepository;

        public GetAllPaymentExamptionRequestHandler(IPaymentExamptionRequestRepository PaymentExamptionRequestQueryRepository)
        {
            _PaymentExamptionRequestRepository = PaymentExamptionRequestQueryRepository;
        }
        public async Task<PaginatedList<PaymentExamptionRequestDTO>> Handle(GetAllPaymentExamptionRequestQuery request, CancellationToken cancellationToken)
        {
            return await PaginatedList<PaymentExamptionRequestDTO>
                            .CreateAsync(
                                _PaymentExamptionRequestRepository.GetAll().Select(r => new PaymentExamptionRequestDTO
                                {
                                    Id = r.Id,
                                    ReasonStr = r.ReasonStr,
                                    Reason = r.Reason,
                                    ExamptedClientId = r.ExamptedClientId,
                                    ExamptedClientFullName = r.ExamptedClientFullName,
                                    ExamptedBy = r.ExamptedBy,
                                    NumberOfClient = r.NumberOfClient
                                    // Description = g.Description.Value<string>("eng")
                                }).ToList()
                                , request.PageCount ?? 1, request.PageSize ?? 10);
            // var PaymentExamptionRequestResponse = CustomMapper.Mapper.Map<List<PaymentExamptionRequestDTO>>(PaymentExamptionRequestList);
            // return PaymentExamptionRequestResponse;
        }
    }
}