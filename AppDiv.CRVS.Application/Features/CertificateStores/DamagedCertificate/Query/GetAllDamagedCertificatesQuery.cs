using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.CertificateStores.CertificateTransfers.Query
{
    public record GetAllDamagedCertificatesQuery : IRequest<PaginatedList<DamagedCertificatesDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetAllDamagedCertificatesHandler : IRequestHandler<GetAllDamagedCertificatesQuery, PaginatedList<DamagedCertificatesDTO>>
    {
        private readonly ICertificateRangeRepository _certificateRangeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserResolverService _userResolver;

        public GetAllDamagedCertificatesHandler(ICertificateRangeRepository certificateRangeRepository, 
                                                IUserRepository userRepository,
                                                IUserResolverService userResolver)
        {
            _certificateRangeRepository = certificateRangeRepository;
            _userRepository = userRepository;
            _userResolver = userResolver;
        }
        public async Task<PaginatedList<DamagedCertificatesDTO>> Handle(GetAllDamagedCertificatesQuery request, CancellationToken cancellationToken)
        {
            // return the paginated list of received certificate serial numbers by the user.
            return await PaginatedList<DamagedCertificatesDTO>
                            .CreateAsync(
                                _certificateRangeRepository.GetAll()
                                    .Include(c => c.User.PersonalInfo)
                                    .Where(c => c.IsDamaged == true && c.AddressId == _userResolver.GetWorkingAddressId())
                                    .OrderByDescending(c => c.CreatedAt)
                                    .Select(sn => new DamagedCertificatesDTO
                                    {
                                        Id = sn.Id,
                                        From = sn.From,
                                        To = sn.To,
                                        ReportBy = sn.User.PersonalInfo.FullNameLang,
                                    })
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}