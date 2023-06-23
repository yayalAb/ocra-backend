﻿using AppDiv.CRVS.Application.Common;
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

namespace AppDiv.CRVS.Application.Features.Certificates.Query
{
    // Customer query with List<Customer> response
    public record GetAllCertificateQuery : IRequest<PaginatedList<CertificateDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public Guid CivilRegOfficerId { get; set; }

    }

    public class GetAllCertificateHandler : IRequestHandler<GetAllCertificateQuery, PaginatedList<CertificateDTO>>
    {
        private readonly ICertificateRepository _CertificateRepository;

        public GetAllCertificateHandler(ICertificateRepository CertificateQueryRepository)
        {
            _CertificateRepository = CertificateQueryRepository;
        }
        public async Task<PaginatedList<CertificateDTO>> Handle(GetAllCertificateQuery request, CancellationToken cancellationToken)
        {
            return await PaginatedList<CertificateDTO>
                            .CreateAsync(
                                _CertificateRepository.GetAll().Where(x => x.Event.CivilRegOfficerId == request.CivilRegOfficerId).Select(c => new CertificateDTO
                                {
                                    Id = c.Id,
                                    EventId = c.EventId,
                                    Content = c.Content,
                                    Status = c.Status,
                                    AuthenticationStatus = c.AuthenticationStatus,
                                    PrintCount = c.PrintCount,
                                    CertificateSerialNumber = c.CertificateSerialNumber,
                                    // Description = g.Description.Value<string>("eng")
                                })
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}