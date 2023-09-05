﻿using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Features.Certificates.Command.Create;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Application.Exceptions;

namespace AppDiv.CRVS.Application.Features.Archives.Query
{
    public class ArchiveResponseDTO
    {
        public JObject Content { get; set; }
        public Guid? TemplateId { get; set; }
         public bool? IsAuthenticated { get; set; } = false;

    }

    // Customer GenerateCustomerQuery with Customer response
    public class GenerateArchiveQuery : IRequest<object>
    {
        public Guid Id { get; set; }
        public string? CertificateSerialNumber { get; set; }
        public bool IsPrint { get; set; } = false;


    }

    public class GenerateArchiveHandler : IRequestHandler<GenerateArchiveQuery, object>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserResolverService _userResolverService;
        private readonly IBirthEventRepository _IBirthEventRepository;
        private readonly ICertificateTemplateRepository _ICertificateTemplateRepository;
        private readonly IArchiveGenerator _archiveGenerator;


        public GenerateArchiveHandler(
                                        IArchiveGenerator archiveGenerator,
                                        IBirthEventRepository IBirthEventRepository,
                                        ICertificateTemplateRepository ICertificateTemplateRepository,
                                        ICertificateRepository CertificateRepository,
                                        IEventRepository eventRepository,
                                        IUserRepository userRepository,
                                        IUserResolverService userResolverService)
        {
            _certificateRepository = CertificateRepository;
            _eventRepository = eventRepository;
            _ICertificateTemplateRepository = ICertificateTemplateRepository;
            _IBirthEventRepository = IBirthEventRepository;
            _archiveGenerator = archiveGenerator;
            _userRepository = userRepository;
            _userResolverService = userResolverService;
        }
        public async Task<object> Handle(GenerateArchiveQuery request, CancellationToken cancellationToken)
        {
            bool Authenticated=true;

            var selectedEvent = await _eventRepository.GetByIdAsync(request.Id);
            if (selectedEvent == null)
            {
                throw new NotFoundException("Event with the given Id  Does not Found");
            }
            Guid PersonalInfoId = _userResolverService.GetUserPersonalId();
            if (PersonalInfoId == null || PersonalInfoId == Guid.Empty)
            {
                throw new NotFoundException("User Not Found Please Logout and Login Once");
            }
            var userInfo = _userRepository.GetAll()
            .Include(x => x.Address)
            .Where(x => x.PersonalInfoId == PersonalInfoId).FirstOrDefault();
            if (userInfo == null)
            {
                throw new NotFoundException("User Not Found Please Logout and Login Once");

            }
            // if (userInfo.AddressId != selectedEvent.EventRegisteredAddressId)
            // {
            //     throw new NotFoundException("You Are Not Allowed to See This Event Detail");
            // }
            var birthCertificateNo = _IBirthEventRepository.GetAll().Where(x => x.Event.EventOwenerId == selectedEvent.EventOwenerId).FirstOrDefault();
            
            var content = await _eventRepository.GetArchive(request.Id);

            var certificate = _archiveGenerator.GetArchive(request, content, birthCertificateNo?.Event?.CertificateId);
          
            var certificateTemplateId = _ICertificateTemplateRepository.GetAll().Where(c => c.CertificateType == selectedEvent.EventType + " " + "Archive").FirstOrDefault();
           
           var certficatesList= _certificateRepository.GetAll().Where(x => x.EventId == request.Id && x.Status);
          
           foreach(var xx in certficatesList){
               if(!xx.AuthenticationStatus){
                  Authenticated=false;
                  break;
               }
           }
            var response = new ArchiveResponseDTO();
            response.Content = certificate;
            response.TemplateId = certificateTemplateId?.Id;
            response.IsAuthenticated=Authenticated;
            return response;
        }

    }
}
