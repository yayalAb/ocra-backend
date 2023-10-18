using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Config;
using AppDiv.CRVS.Utility.Services;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.ShareReportApi.commands.update
{
    // Customer create command with CustomerResponse
    public class DeactivateApi : IRequest<object>
    {
        public Guid Id { get; set; }
    }

    public class DeactivateApiHandler : IRequestHandler<DeactivateApi, object>
    {
        private readonly ISharedReportRepository _shareReportRepository;
        private readonly IMailService _mailService;
        private readonly SMTPServerConfiguration _config;
        public DeactivateApiHandler(
            IMailService mailService,
            IOptions<SMTPServerConfiguration> config,
            ISharedReportRepository shareReportRepository)
        {
            _mailService=mailService;
            _config=config.Value;
            _shareReportRepository = shareReportRepository;
        }
        public async Task<object> Handle(DeactivateApi request, CancellationToken cancellationToken)
        {
            if(request.Id==null||request.Id==Guid.Empty){
                throw new NotFoundException("Id must not be Empty");
            }
            var Report = await _shareReportRepository.GetAsync(request.Id);
            if(Report==null){
                throw new NotFoundException("Shared Report with the given Id does't Found"); 
                }
                Report.Status = !Report.Status;
            try
            {
                await _shareReportRepository.UpdateAsync(Report, x => x.Id);
                var result = await _shareReportRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw new NotFoundException(exp.Message);
            }
            return new {
               Report.Id,
               Report.ClientId,
               Report.SHASecret,
               Report.Email 
            };
        }
    }
}