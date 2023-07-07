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
    public class GetCertificateTransferByUserQuery : IRequest<PaginatedList<CertificateTransferDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetCertificateTransferByUserHandler : IRequestHandler<GetCertificateTransferByUserQuery, PaginatedList<CertificateTransferDTO>>
    {
        private readonly ICertificateTransferRepository _certificateStoreRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserResolverService _userResolver;

        public GetCertificateTransferByUserHandler(ICertificateTransferRepository certificateStoreRepository, 
                                                    IUserRepository userRepository,
                                                    IUserResolverService userResolver)
        {
            _certificateStoreRepository = certificateStoreRepository;
            _userRepository = userRepository;
            _userResolver = userResolver;
        }
        public async Task<PaginatedList<CertificateTransferDTO>> Handle(GetCertificateTransferByUserQuery request, CancellationToken cancellationToken)
        {
            // return the paginated list of sent certificate serial numbers by the user.
            return await PaginatedList<CertificateTransferDTO>
                            .CreateAsync(
                                _certificateStoreRepository.GetAll()
                                    .Where(c => c.SenderId == _userResolver.GetUserId())
                                    .OrderByDescending(c => c.CreatedAt)
                                    .Select(sn => new CertificateTransferDTO
                                    {
                                        Id = sn.Id,
                                        From = sn.From,
                                        To = sn.To,
                                        Status = sn.Status,
                                        RecieverName = _userRepository.GetSingle(sn.RecieverId).UserName,
                                    })
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}