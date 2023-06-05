using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using AppDiv.CRVS.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.CertificateStores.CertificateTransfers.Command.Update
{
    // Customer create command with CustomerResponse
    public class UpdateCertificateTransferCommand : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }
        // public string? SenderId { get; set; }
        // public string RecieverId { get; set; }
        // public bool Status { get; set; }
        // public int From { get; set; }
        // public int To { get; set; }
    }

    public class UpdateCertificateTransferCommandHandler : IRequestHandler<UpdateCertificateTransferCommand, BaseResponse>
    {
        private readonly ICertificateTransferRepository _CertificateTransferRepository;
        public UpdateCertificateTransferCommandHandler(ICertificateTransferRepository CertificateTransferRepository)
        {
            _CertificateTransferRepository = CertificateTransferRepository;
        }
        public async Task<BaseResponse> Handle(UpdateCertificateTransferCommand request, CancellationToken cancellationToken)
        {
            var CertificateTransferResponse = new BaseResponse();
            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request);
            var certificateTransfer = await _CertificateTransferRepository.GetAsync(request.Id);
            certificateTransfer.Status = true;
            // var CertificateTransfer = CustomMapper.Mapper.Map<CertificateSerialTransfer>(t);


            try
            {
                await _CertificateTransferRepository.UpdateAsync(certificateTransfer, x => x.Id);
                var result = await _CertificateTransferRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            // var modifiedCertificateTransfer = await _CertificateTransferRepository.GetAsync(request.Id);
            // var CertificateTransferResponse = CustomMapper.Mapper.Map<BaseResponse>(modifiedCertificateTransfer);

            return CertificateTransferResponse;
        }
    }
}