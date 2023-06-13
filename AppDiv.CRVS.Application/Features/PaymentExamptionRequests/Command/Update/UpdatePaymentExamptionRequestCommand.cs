using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces;
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
        public LanguageModel Reason { get; set; }
        public string? ExamptedClientId { get; set; }
        public string? ExamptedClientFullName { get; set; }
        public DateTime ExamptedDate { get; set; } = DateTime.Now;
        public string? ExamptedById { get; set; }
        public string ExamptedDateEt { get; set; }
        public int? NumberOfClient { get; set; }
        public Guid? AddressId { get; set; }
        public string CertificateType { get; set; }
        public AddRequest Request { get; set; }
        public UpdatePaymentExamptionRequestCommand()
        {
            this.ExamptedDate = DateTime.Now;
        }
    }

    public class UpdatePaymentExamptionRequestCommandHandler : IRequestHandler<UpdatePaymentExamptionRequestCommand, PaymentExamptionRequestDTO>
    {
        private readonly IPaymentExamptionRequestRepository _PaymentExamptionRequestRepository;
        private readonly IWorkflowService _WorkflowService;
        public UpdatePaymentExamptionRequestCommandHandler(IWorkflowService WorkflowService, IPaymentExamptionRequestRepository PaymentExamptionRequestRepository)
        {
            _PaymentExamptionRequestRepository = PaymentExamptionRequestRepository;
            _WorkflowService = WorkflowService;
        }
        public async Task<PaymentExamptionRequestDTO> Handle(UpdatePaymentExamptionRequestCommand request, CancellationToken cancellationToken)
        {
            var PaymentExamptionRequest = CustomMapper.Mapper.Map<PaymentExamptionRequest>(request);
            PaymentExamptionRequest.Request.RequestType = "payment exemption";
            PaymentExamptionRequest.Request.NextStep = _WorkflowService.GetNextStep("payment exemption", PaymentExamptionRequest.Request.currentStep, true);
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