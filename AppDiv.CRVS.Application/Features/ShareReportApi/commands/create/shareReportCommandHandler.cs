using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;
using Newtonsoft.Json;
using AppDiv.CRVS.Utility.Services;
using AppDiv.CRVS.Utility.Config;
using Microsoft.Extensions.Options;

namespace AppDiv.CRVS.Application.Features.ShareReportApi.commands.create
{

    public class shareReportCommandHandler : IRequestHandler<CreateReportApiCommands, object>
    {
        private readonly ISharedReportRepository _ReportRepository;
        private readonly IReportStoreRepostory  _reportStoreRepo;
        private readonly IUserResolverService _UserResolver;
        private readonly IUserRepository _userReportory;
        private readonly IMailService _mailService;
        private readonly SMTPServerConfiguration _config;



        public shareReportCommandHandler(
            IMailService mailService,
            IOptions<SMTPServerConfiguration> config,
            IReportStoreRepostory  reportStoreRepo,
            ISharedReportRepository ReportRepository,
            IUserResolverService UserResolver, 
            
            IUserRepository userReportory)
        {
            _ReportRepository = ReportRepository;
            _UserResolver = UserResolver;
            _userReportory = userReportory;
            _config = config.Value;
            _reportStoreRepo=reportStoreRepo;
            _mailService=mailService;
        }
        public async Task<object> Handle(CreateReportApiCommands request, CancellationToken cancellationToken)
        { 
                var user = _userReportory.GetAll().Where(x => x.PersonalInfoId == _UserResolver.GetUserPersonalId()).FirstOrDefault();
                if (user == null)
                {
                    throw new NotFoundException("User Does Not Found");
                }
                var SharedReport = new SharedReport
                {
                    ReportName =request.ReportName,
                    ReportTitle  =request.ReportTitle,   
                    Agrgate  =request.Agrgate,    
                    Filter  =request.Filter,    
                    Colums  =request.Colums,    
                    Other  =request.Other,    
                    Username  =request.Username,  
                    UserRole  =request.UserRole,
                    Status=true,    
                    ClientId  =Guid.NewGuid().ToString(),   
                    SHASecret  =Guid.NewGuid().ToString(),   
                    Email  =request.Email,
                };
                await _ReportRepository.InsertAsync(SharedReport, cancellationToken);
                await _ReportRepository.SaveChangesAsync(cancellationToken);
                   var content ="ClinetId : " + SharedReport.ClientId +"+ \n SHASecret : "+SharedReport.SHASecret;
                    var subject = "OCRVS";
                    await _mailService.SendAsync(body: content, subject: subject, senderMailAddress: _config.SENDER_ADDRESS, receiver: SharedReport.Email, cancellationToken);

            return new {
                Id=SharedReport.Id,
                clientId=SharedReport.ClientId,
                SHASecret=SharedReport.SHASecret,
                Email=SharedReport.Email 
            };
        }
    }
}
