using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.CertificateStores.CertificateTransfers.Query
{
    // Customer query with List<Customer> response
    public record GetAllCertificateTransferQuery : IRequest<PaginatedList<CertificateTransferDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        // public string UserName { get; set; }
    }

    public class GetAllCertificateTransferHandler : IRequestHandler<GetAllCertificateTransferQuery, PaginatedList<CertificateTransferDTO>>
    {
        private readonly ICertificateTransferRepository _certificateStoreRepository;
        private readonly IUserRepository _userRepository;

        public GetAllCertificateTransferHandler(ICertificateTransferRepository certificateStoreQueryRepository, IUserRepository userRepository)
        {
            _certificateStoreRepository = certificateStoreQueryRepository;
            _userRepository = userRepository;
        }
        public async Task<PaginatedList<CertificateTransferDTO>> Handle(GetAllCertificateTransferQuery request, CancellationToken cancellationToken)
        {
            // var user = _userRepository.GetAll().Where(u => u.UserName == request.UserName).FirstOrDefault().Id;

            // var recieve = _certificateStoreRepository.GetAll().Where(c => c.RecieverId == user);
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
                                        SenderName = _userRepository.GetAll().Where(u => u.Id == sn.SenderId).FirstOrDefault().UserName,
                                    }).ToList()
                                , request.PageCount ?? 1, request.PageSize ?? 10);
            // var certificateStoreResponse = CustomMapper.Mapper.Map<List<CertificateTransferDTO>>(certificateStoreList);
            // return certificateStoreResponse;
        }
    }
}