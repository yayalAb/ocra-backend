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
    public record GetAllCertificateTransferQuery : IRequest<PaginatedList<CertificateTransferDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetAllCertificateTransferHandler : IRequestHandler<GetAllCertificateTransferQuery, PaginatedList<CertificateTransferDTO>>
    {
        private readonly ICertificateTransferRepository _certificateStoreRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserResolverService _userResolver;

        public GetAllCertificateTransferHandler(ICertificateTransferRepository certificateStoreQueryRepository, 
                                                IUserRepository userRepository,
                                                IUserResolverService userResolver)
        {
            _certificateStoreRepository = certificateStoreQueryRepository;
            _userRepository = userRepository;
            _userResolver = userResolver;
        }
        public async Task<PaginatedList<CertificateTransferDTO>> Handle(GetAllCertificateTransferQuery request, CancellationToken cancellationToken)
        {
            // return the paginated list of received certificate serial numbers by the user.
            return await PaginatedList<CertificateTransferDTO>
                            .CreateAsync(
                                _certificateStoreRepository.GetAll()
                                    .Where(c => c.RecieverId == _userResolver.GetUserId())
                                    .Select(sn => new CertificateTransferDTO
                                    {
                                        Id = sn.Id,
                                        From = sn.From,
                                        To = sn.To,
                                        Status = sn.Status,
                                        SenderName = _userRepository.GetSingle(sn.SenderId).UserName,
                                    })
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}