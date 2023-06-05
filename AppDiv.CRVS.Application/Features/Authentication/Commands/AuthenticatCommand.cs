using System;
using System.Collections.Generic;
using System.Linq;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.Authentication.Commands
{
    // Customer create command with CustomerResponse
    public class AuthenticatCommand : IRequest<BaseResponse>
    {
        public Guid RequestId { get; set; }
        public bool IsApprove { get; set; }
    }
    public class AuthenticatCommandHandler : IRequestHandler<AuthenticatCommand, BaseResponse>
    {
        private readonly IAuthenticationRepository _AuthenticationnRequestRepostory;
        private readonly ICertificateRepository _CertificateRepository;
        private readonly IWorkflowService _WorkflowService;
        public AuthenticatCommandHandler(IWorkflowService WorkflowService, IAuthenticationRepository AuthenticationnRequestRepostory, ICertificateRepository CertificateRepository)
        {
            _AuthenticationnRequestRepostory = AuthenticationnRequestRepostory;
            _CertificateRepository = CertificateRepository;
            _WorkflowService = WorkflowService;
        }
        public async Task<BaseResponse> Handle(AuthenticatCommand request, CancellationToken cancellationToken)
        {
            var AuthentcationData = _AuthenticationnRequestRepostory.GetAll()
            .Include(x => x.Request)
            .Where(x => x.RequestId == request.RequestId).FirstOrDefault();
            if (AuthentcationData == null)
            {
                throw new Exception("no steps found");
            }
            if (AuthentcationData.Request.currentStep >= 0 && AuthentcationData.Request.currentStep < _WorkflowService.GetLastWorkflow("authentication"))
            {
                var nextStep = _WorkflowService.GetNextStep("authentication", AuthentcationData.Request.currentStep, request.IsApprove);
                AuthentcationData.Request.currentStep = nextStep;
                if (nextStep == _WorkflowService.GetLastWorkflow("authentication"))
                {
                    var certificate = await _CertificateRepository.GetAsync(AuthentcationData.CertificateId);
                    certificate.AuthenticationStatus = true;
                    await _AuthenticationnRequestRepostory.UpdateAsync(AuthentcationData, x => x.Id);
                }
            }
            else
            {
                throw new Exception("next step doesnot exist");
            }

            try
            {
                await _AuthenticationnRequestRepostory.UpdateAsync(AuthentcationData, x => x.Id);
                await _AuthenticationnRequestRepostory.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            return new BaseResponse
            {
            };
        }
    }
}