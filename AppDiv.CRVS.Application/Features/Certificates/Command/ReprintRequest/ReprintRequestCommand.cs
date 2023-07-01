using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Certificates.Command.ReprintRequest
{
    // Customer create command with CustomerResponse
    public class ReprintRequestCommand : IRequest<BaseResponse>
    {

        public Guid Id { get; set; }
    }

    public class ReprintRequestCommandHandler : IRequestHandler<ReprintRequestCommand, BaseResponse>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEventPaymentRequestService _paymentRequestService;
        public ReprintRequestCommandHandler(IEventRepository eventRepository, IEventPaymentRequestService paymentRequestService, ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
            _paymentRequestService = paymentRequestService;
            _eventRepository = eventRepository;
        }
        public async Task<BaseResponse> Handle(ReprintRequestCommand request, CancellationToken cancellationToken)
        {
            var res = new BaseResponse
            {
                Message = "Payment Request Sent Successfuly",
                Success = true
            };
            var certificate = _certificateRepository.GetAll()
            .Include(x => x.Event)
            .ThenInclude(x => x.EventOwener)
            .Where(x => x.Id == request.Id).FirstOrDefault();
            if (certificate == null)
            {
                throw new Exception("Certificate not Found!");
            }

            try
            {
                try
                {
                    Console.WriteLine("payment rate : {0}", certificate.Event.EventType);
                    (float amount, string code) response = await _paymentRequestService.CreatePaymentRequest(certificate.Event.EventType, certificate.Event, "Reprint", null, false, false, cancellationToken);
                    if (response.amount == 0)
                    {
                        res.Message = "Payment Rate not found";
                        res.Success = false;
                    }
                }
                catch (Exception exp)
                {

                    throw new Exception(exp.Message);

                }

                var SelectedEvent = _eventRepository.GetAll().Where(x => x.Id == certificate.EventId).FirstOrDefault();
                SelectedEvent.OnReprintPaymentRequest = true;
                await _eventRepository.UpdateAsync(SelectedEvent, x => x.Id);
                var result = await _eventRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            // var modifiedCertificate = await _certificateRepository.GetAsync(request.Id);
            // var CertificateResponse = CustomMapper.Mapper.Map<CertificateDTO>(modifiedCertificate);

            return res;
        }
    }
}