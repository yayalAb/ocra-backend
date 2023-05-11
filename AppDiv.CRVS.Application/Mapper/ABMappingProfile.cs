using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Features.AddressLookup.Commands.Create;
using AppDiv.CRVS.Application.Features.AddressLookup.Commands.Update;
using AppDiv.CRVS.Application.Features.Customers.Command.Create;
using AppDiv.CRVS.Application.Features.Customers.Command.Update;
using AppDiv.CRVS.Application.Features.Groups.Commands.Create;
using AppDiv.CRVS.Application.Features.Lookups.Command.Create;
using AppDiv.CRVS.Application.Features.Lookups.Command.Update;
using AppDiv.CRVS.Application.Features.Settings.Commands.create;
using AppDiv.CRVS.Application.Features.Settings.Commands.Update;
using AppDiv.CRVS.Application.Features.WorkFlows.Commands.Create;
using AppDiv.CRVS.Application.Features.User.Command.Create;
using AppDiv.CRVS.Application.Features.User.Command.Update;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain.Entities;
using Application.Common.Mappings;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Features.PaymentRates.Command.Create;
using AppDiv.CRVS.Application.Features.PaymentRates.Command.Update;
using AppDiv.CRVS.Application.Features.MarriageApplications.Command.Create;
using AppDiv.CRVS.Application.Features.MarriageApplications.Command.Update;
using AppDiv.CRVS.Application.Features.DeathEvents.Command.Create;
using AppDiv.CRVS.Application.Features.DeathEvents.Command.Update;
using AppDiv.CRVS.Application.Features.DivorceEvents.Command.Create;
using AppDiv.CRVS.Application.Features.DivorceEvents.Command.Update;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Create;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Update;
using AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Create;
using AppDiv.CRVS.Application.Features.BirthEvents.Command.Update;
using AppDiv.CRVS.Application.Features.PaymentExamptionRequests.Command.Update;

namespace AppDiv.CRVS.Application.Mapper
{
    internal class CRVSMappingProfile : Profile
    {
        public CRVSMappingProfile()
        {

            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());



        }
        private void ApplyMappingsFromAssembly(Assembly assembly)
        {

            CreateMap<Customer, CustomerResponseDTO>().ReverseMap();
            CreateMap<Customer, CreateCustomerCommand>().ReverseMap();
            CreateMap<Customer, EditCustomerCommand>().ReverseMap();

            CreateMap<Lookup, LookupDTO>().ReverseMap();
            CreateMap<Lookup, CreateLookupCommand>().ReverseMap();
            CreateMap<Lookup, UpdateLookupCommand>().ReverseMap();

            CreateMap<Address, AddressDTO>().ReverseMap();
            // CreateMap<AddressDTO, AddressForLookupDTO>().ReverseMap();

            CreateMap<Address, CreateAdderssCommand>().ReverseMap();
            CreateMap<Address, UpdateaddressCommand>().ReverseMap();

            CreateMap<Setting, SettingDTO>().ReverseMap();
            CreateMap<Setting, createSettingCommand>().ReverseMap();
            CreateMap<Setting, UpdateSettingCommand>().ReverseMap();

            CreateMap<UserGroup, GroupDTO>().ReverseMap();
            CreateMap<UserGroup, CreateGroupCommand>().ReverseMap();

            CreateMap<Workflow, WorkflowDTO>().ReverseMap();
            CreateMap<Workflow, GetAllWorkFlowDTO>().ReverseMap();
            CreateMap<Workflow, CreateWorkFlowCommand>().ReverseMap();


            CreateMap<Step, StepDTO>().ReverseMap();
            CreateMap<PaymentRate, PaymentRateDTO>().ReverseMap();
            CreateMap<PaymentRate, AddPaymentRateRequest>().ReverseMap();
            CreateMap<PaymentRate, CreatePaymentRateCommand>().ReverseMap();
            CreateMap<PaymentRate, UpdatePaymentRateCommand>().ReverseMap();





            CreateMap<ApplicationUser, UserResponseDTO>().ReverseMap();
            CreateMap<CreateUserCommand, ApplicationUser>()
            .ForMember(x => x.UserGroups, opt => opt.Ignore())
            .ReverseMap();
            CreateMap<ApplicationUser, UpdateUserCommand>().ReverseMap();

            // CreateMap<ApplicationUser, FetchSingleUserResponseDTO>().ReverseMap();
            CreateMap<ContactInfo, ContactInfoDTO>().ReverseMap();

            CreateMap<PersonalInfo, AddPersonalInfoRequest>().ReverseMap();
            CreateMap<ContactInfo, AddContactInfoRequest>().ReverseMap();
            CreateMap<PersonalInfo, UpdatePersonalInfoRequest>().ReverseMap();
            // CreateMap<PersonalInfo, PersonalInfoDTO>().ReverseMap();
            CreateMap<ContactInfo, UpdateContactInfoRequest>().ReverseMap();
            CreateMap<ApplicationUser, UpdateUserCommand>().ReverseMap();
            CreateMap<PersonalInfo, AdoptionEventPersonalInfoDTO>().ReverseMap();


            CreateMap<DeathEvent, DeathEventDTO>().ReverseMap();
            CreateMap<DeathEvent, AddDeathEventRequest>().ReverseMap();
            CreateMap<DeathEvent, CreateDeathEventCommand>().ReverseMap();
            CreateMap<DeathEvent, UpdateDeathEventCommand>().ReverseMap();
            CreateMap<BirthEvent, UpdateBirthEventCommand>().ReverseMap();

            CreateMap<DeathNotification, AddDeathNotificationRequest>().ReverseMap();
            CreateMap<DeathNotification, DeathNotificationDTO>().ReverseMap();
            CreateMap<DeathNotification, UpdateDeathNotificationRequest>().ReverseMap();
            CreateMap<BirthNotification, UpdateBirthNotificationRequest>().ReverseMap();

            CreateMap<Event, EventDTO>().ReverseMap();
            CreateMap<Event, AddEventRequest>().ReverseMap();
            CreateMap<Event, AddAdoptionEventRequest>().ReverseMap();

            CreateMap<Event, UpdateEventRequest>().ReverseMap();

            CreateMap<PaymentExamption, PaymentExamptionDTO>().ReverseMap();
            CreateMap<PaymentExamption, AddPaymentExamptionRequest>().ReverseMap();

            CreateMap<SupportingDocument, SupportingDocumentDTO>().ReverseMap();
            CreateMap<SupportingDocument, SupportingDocumentRequest>().ReverseMap();

            CreateMap<Certificate, CertificateDTO>().ReverseMap();
            CreateMap<Certificate, CertificateRequest>().ReverseMap();

            CreateMap<PaymentExamptionRequest, PaymentExamptionRequestDTO>().ReverseMap();
            CreateMap<PaymentExamptionRequest, PaymentExamptionRequestRequest>().ReverseMap();
            CreateMap<PaymentExamptionRequest, UpdatePaymentExamptionRequestCommand>().ReverseMap();

            CreateMap<CreateMarriageApplicationCommand, MarriageApplication>();
            CreateMap<MarriageApplication, MarriageApplicationGridDTO>();
            CreateMap<UpdateMarriageApplicationCommand, MarriageApplication>().ReverseMap();

            CreateMap<AddEventRequest, Event>();
            CreateMap<AddWitnessRequest, Witness>();

            CreateMap<AddRegistrarRequest, Registrar>();
            CreateMap<Registrar, RegistrarDTO>().ReverseMap();

            CreateMap<AddSupportingDocumentRequest, SupportingDocument>();
            CreateMap<AddPaymentExamptionDTO, PaymentExamption>();
            CreateMap<BirthEvent, AddBirthEventRequest>().ReverseMap();
            CreateMap<BirthEvent, BirthEventDTO>().ReverseMap();

            CreateMap<BirthNotification, AddBirthNotificationRequest>().ReverseMap();
            CreateMap<BirthNotification, BirthNotificationDTO>().ReverseMap();

            CreateMap<CreateDivorceEventCommand, DivorceEvent>();
            CreateMap<UpdateDivorceEventCommand, DivorceEvent>().ReverseMap();

            CreateMap<CreateMarriageEventCommand, MarriageEvent>();
            CreateMap<UpdateMarriageEventCommand, MarriageEvent>().ReverseMap();


            CreateMap<AdoptionEvent, AddAdoptionRequest>().ReverseMap();
            CreateMap<AdoptionEvent, CreateAdoptionCommand>().ReverseMap();

            CreateMap<CourtCase, AddCourtCaseRequest>().ReverseMap();
            CreateMap<AdoptionEvent, AdoptionDTO>().ReverseMap();



            CreateMap<SupportingDocument, UpdateSupportingDocumentRequest>().ReverseMap();
            CreateMap<PaymentExamption, UpdatePaymentExamptionRequest>().ReverseMap();
            CreateMap<Registrar, UpdateRegistrarRequest>().ReverseMap();








            // CreateMap<List<ApplicationUser>, List<UserResponseDTO>>().ReverseMap();

            var mapFromType = typeof(IMapFrom<>);

            var mappingMethodName = nameof(IMapFrom<object>.Mapping);

            bool HasInterface(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == mapFromType;

            var types = assembly.GetExportedTypes().Where(t => t.GetInterfaces().Any(HasInterface)).ToList();

            var argumentTypes = new Type[] { typeof(Profile) };

            foreach (var type in types)
            {

                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod(mappingMethodName);

                if (methodInfo != null)
                {
                    methodInfo.Invoke(instance, new object[] { this });
                }
                else
                {
                    var interfaces = type.GetInterfaces().Where(HasInterface).ToList();

                    if (interfaces.Count > 0)
                    {
                        foreach (var @interface in interfaces)
                        {
                            var interfaceMethodInfo = @interface.GetMethod(mappingMethodName, argumentTypes);

                            interfaceMethodInfo?.Invoke(instance, new object[] { this });
                        }
                    }
                }

            }
        }
    }
}
