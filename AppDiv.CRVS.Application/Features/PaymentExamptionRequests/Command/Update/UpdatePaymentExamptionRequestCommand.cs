using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.PaymentExamptionRequests.Command.Update
{
    // Customer create command with CustomerResponse
    public class UpdatePaymentExamptionRequestCommand : IRequest<PaymentExamptionRequestDTO>
    {

        public Guid Id { get; set; }
        public JObject Reason { get; set; }
        public string? ExamptedClientId { get; set; }
        public string? ExamptedClientFullName { get; set; }
        public DateTime ExamptedDate { get; set; }
        public string ExamptedBy { get; set; }
        public string? NumberOfClient { get; set; }

        public UpdatePaymentExamptionRequestCommand()
        {
            this.ExamptedDate = DateTime.Now;
        }
    }

    public class UpdatePaymentExamptionRequestCommandHandler : IRequestHandler<UpdatePaymentExamptionRequestCommand, PaymentExamptionRequestDTO>
    {
        private readonly IPaymentExamptionRequestRepository _PaymentExamptionRequestRepository;
        public UpdatePaymentExamptionRequestCommandHandler(IPaymentExamptionRequestRepository PaymentExamptionRequestRepository)
        {
            _PaymentExamptionRequestRepository = PaymentExamptionRequestRepository;
        }
        public async Task<PaymentExamptionRequestDTO> Handle(UpdatePaymentExamptionRequestCommand request, CancellationToken cancellationToken)
        {
            var PaymentExamptionRequest = CustomMapper.Mapper.Map<PaymentExamptionRequest>(request);

            try
            {
                await _PaymentExamptionRequestRepository.UpdateAsync(PaymentExamptionRequest, x => x.Id);
                var result = await _PaymentExamptionRequestRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            var modifiedPaymentExamptionRequest = await _PaymentExamptionRequestRepository.GetAsync(request.Id);
            var PaymentExamptionRequestResponse = CustomMapper.Mapper.Map<PaymentExamptionRequestDTO>(modifiedPaymentExamptionRequest);

            return PaymentExamptionRequestResponse;
        }
    }
}