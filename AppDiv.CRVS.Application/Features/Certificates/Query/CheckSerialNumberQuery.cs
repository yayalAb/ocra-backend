using AppDiv.CRVS.Application.Common;
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

namespace AppDiv.CRVS.Application.Features.Certificates.Query.Check
{
    // Customer create command with CustomerResponse
    public class CheckSerialNoValidation : IRequest<BaseResponse>
    {
        public string CertificateSerialNumber { get; set; }
        public Guid UserId { get; set; }
    }

    public class CheckSerialNoValidationHandler : IRequestHandler<CheckSerialNoValidation, BaseResponse>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ICertificateRangeRepository _certificateRange;
        private readonly IUserRepository _user;
        public CheckSerialNoValidationHandler(ICertificateRepository certificateRepository,
                                            ICertificateRangeRepository certificateRange,
                                            IUserRepository user)
        {
            _certificateRepository = certificateRepository;
            _certificateRange = certificateRange;
            _user = user;
        }
        public Task<BaseResponse> Handle(CheckSerialNoValidation request, CancellationToken cancellationToken)
        {

            var response = new BaseResponse();
            try
            {
                var user = _user.GetSingle(request.UserId == Guid.Empty ? "2f20c2f6-b3ab-47a4-a36e-0b4fb9b94006" : request.UserId.ToString());
                var inRange = _certificateRange.GetAll().FirstOrDefault(r => r.AddressId == user.AddressId
                                                                && request.CertificateSerialNumber.CompareTo(r.From.ToString()) >= 0
                                                                && request.CertificateSerialNumber.CompareTo(r.To.ToString()) <= 0);
                bool isDuplicated = _certificateRepository.GetAll()
                                        .Where(c => c.CertificateSerialNumber == request.CertificateSerialNumber).Any();
                if (inRange == null || isDuplicated)
                {
                    response.Status = 400;
                    response.Message = "Serial Number out of range";
                }

            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            return Task.FromResult(response);
        }
    }
}