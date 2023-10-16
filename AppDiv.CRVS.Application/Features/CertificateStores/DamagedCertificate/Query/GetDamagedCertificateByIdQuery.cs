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
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.CertificateStores.CertificateTransfers.Query
{
    public class GetDamagedCertificateByIdQuery : IRequest<DamagedCertificatesDTO>
    {
        public Guid Id { get; set; }
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetDamagedCertificateByIdHandler : IRequestHandler<GetDamagedCertificateByIdQuery, DamagedCertificatesDTO>
    {
        private readonly ICertificateRangeRepository _certificateRangeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserResolverService _userResolver;

        public GetDamagedCertificateByIdHandler(ICertificateRangeRepository certificateRangeRepository, 
                                                    IUserRepository userRepository,
                                                    IUserResolverService userResolver)
        {
            _certificateRangeRepository = certificateRangeRepository;
            _userRepository = userRepository;
            _userResolver = userResolver;
        }
        public async Task<DamagedCertificatesDTO> Handle(GetDamagedCertificateByIdQuery request, CancellationToken cancellationToken)
        {
            // return the paginated list of sent certificate serial numbers by the user.
            return _certificateRangeRepository.GetAll()
                                    .Include(c => c.User.PersonalInfo)
                                    .Where(c => c.Id == request.Id)
                                    .OrderByDescending(c => c.CreatedAt)
                                    .Select(sn => new DamagedCertificatesDTO
                                    {
                                        Id = sn.Id,
                                        From = sn.From,
                                        To = sn.To,
                                        ReportBy = sn.User.PersonalInfo.FullNameLang
                                    }).SingleOrDefault();
        }
    }
}