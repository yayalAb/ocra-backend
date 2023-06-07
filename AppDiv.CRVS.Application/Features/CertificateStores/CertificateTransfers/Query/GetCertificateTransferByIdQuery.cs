using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.CertificateStores.CertificateTransfers.Query
{
    // Customer GetCustomerByUserQuery with Customer response
    public class GetCertificateTransferByUserQuery : IRequest<PaginatedList<CertificateTransferDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetCertificateTransferByUserHandler : IRequestHandler<GetCertificateTransferByUserQuery, PaginatedList<CertificateTransferDTO>>
    {
        private readonly ICertificateTransferRepository _certificateStoreRepository;
        private readonly IUserRepository _userRepository;

        public GetCertificateTransferByUserHandler(ICertificateTransferRepository certificateStoreRepository, IUserRepository userRepository)
        {
            _certificateStoreRepository = certificateStoreRepository;
            _userRepository = userRepository;
        }
        public async Task<PaginatedList<CertificateTransferDTO>> Handle(GetCertificateTransferByUserQuery request, CancellationToken cancellationToken)
        {
            var transfers = _certificateStoreRepository.GetAll().Where(t => t.SenderId == "");
            return await PaginatedList<CertificateTransferDTO>
                            .CreateAsync(
                                _certificateStoreRepository.GetAll()
                                    // .Where(c => c.RecieverId == user)
                                    .Select(sn => new CertificateTransferDTO
                                    {
                                        Id = sn.Id,
                                        From = sn.From,
                                        To = sn.To,
                                        Status = sn.Status,
                                        RecieverName = _userRepository.GetAll().Where(u => u.Id == sn.RecieverId).FirstOrDefault().UserName,
                                    }).ToList()
                                , request.PageCount ?? 1, request.PageSize ?? 10);
            // return selectedCustomer;
        }
    }
}