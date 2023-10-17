using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Application.Features.CertificateStores.CertificateTransfers.Command.Create
{
    public record UpdateDamagedCertificatesCommand : IRequest<UpdateDamagedCertificatesCommandResponse>
    {
        public Guid Id { get; set; }
        public string From { get; set; }
        public string To  { get; set; }
    }
}