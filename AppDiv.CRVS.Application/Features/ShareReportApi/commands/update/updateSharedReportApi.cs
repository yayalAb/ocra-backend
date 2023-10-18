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
    public class updateSharedReportApi : IRequest<object>
    {
        public Guid Id { get; set; }
        public string? ReportName { get; set; }
        public string?  ReportTitle  { get; set; }   
        public string?  Agrgate  { get; set; }    
        public string?  Filter  { get; set; }    
        public string?  Colums  { get; set; }    
        public string?  Other  { get; set; }    
        public string  Username  { get; set; }  
        public string?  UserRole  { get; set; }    
        public string  Email  { get; set; }
        public Boolean IsBasicInfo { get; set; } = false;
    }

    public class updateSharedReportApiHandler : IRequestHandler<updateSharedReportApi, object>
    {
        private readonly ISharedReportRepository _shareReportRepository;
        private readonly IMailService _mailService;
        private readonly SMTPServerConfiguration _config;
        public updateSharedReportApiHandler(
            IMailService mailService,
            IOptions<SMTPServerConfiguration> config,
            ISharedReportRepository shareReportRepository)
        {
            _mailService=mailService;
            _config=config.Value;
            _shareReportRepository = shareReportRepository;
        }
        public async Task<object> Handle(updateSharedReportApi request, CancellationToken cancellationToken)
        {
            if(request.Id==null||request.Id==Guid.Empty){
                throw new NotFoundException("Id must not be Empty");
            }
            var Report = await _shareReportRepository.GetAsync(request.Id);
            if(Report==null){
                throw new NotFoundException("Shared Report with the given Id does't Found"); 
                }
                if(!request.IsBasicInfo){
                    Report.ReportName = request.ReportName;
                    Report.ReportTitle = request.ReportTitle;
                    Report.Agrgate =request.Agrgate;    
                    Report.Filter =request.Filter;    
                    Report.Colums =request.Colums;    
                    Report.Other =request.Other; 
                }
                Report.Username =request.Username; 
                Report.UserRole =request.UserRole;    
                Report.Email =request.Email;
                Report.ClientId=Guid.NewGuid().ToString();
                Report.SHASecret=Guid.NewGuid().ToString();
                var content = "ClinetId : " + Report.ClientId +" SHASecret : "+ Report.SHASecret;
                var subject = "OCRVS";
                await _mailService.SendAsync(body: content, subject: subject, senderMailAddress: _config.SENDER_ADDRESS, receiver: Report.Email, cancellationToken);
                
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