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
using AppDiv.CRVS.Application.Features.Certificates.Command.Update;
using static AppDiv.CRVS.Application.Contracts.Request.AdoptionPersonalINformationRequest;
using AppDiv.CRVS.Application.Features.PaymentExamptionRequests.Command.Create;
using AppDiv.CRVS.Application.Features.CertificateStores.CertificateTransfers.Command.Create;
using AppDiv.CRVS.Application.Features.CertificateStores.CertificateTransfers.Command.Update;
using Newtonsoft.Json.Linq;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive.AdoptionArchive;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive.BirthArchive;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive.DeathArchive;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive.MarriageArchive;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive.DivorceArchive;
using AppDiv.CRVS.Application.Features.Payments.Command.Create;
using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;
using AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Update;
using AppDiv.CRVS.Application.Features.Plans.Command.Update;
using AppDiv.CRVS.Domain.Entities.Audit;
using  AppDiv.CRVS.Application.Features.Messages.Command.Create;

namespace AppDiv.CRVS.Application.Mapper
{


    internal class CRVSMappingProfile : Profile
    {
        public CRVSMappingProfile()
        {
            RecognizePostfixes("Lookup");
            // RecognizePrefixes("frm");
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());

        }


        private void ApplyMappingsFromAssembly(Assembly assembly)
        {


            CreateMap<Customer, CustomerResponseDTO>().ReverseMap();
            CreateMap<Customer, CreateCustomerCommand>().ReverseMap();
            CreateMap<Customer, EditCustomerCommand>().ReverseMap();

            CreateMap<Lookup, LookupDTO>().ReverseMap();
            CreateMap<Lookup, LookupCouchDTO>();
            CreateMap<Lookup, CreateLookupCommand>().ReverseMap();
            CreateMap<Lookup, UpdateLookupCommand>().ReverseMap();

            CreateMap<Address, AddressDTO>().ReverseMap();
            // CreateMap<AddressDTO, AddressForLookupDTO>().ReverseMap();

            CreateMap<Address, CreateAdderssCommand>().ReverseMap();
            CreateMap<Address, UpdateaddressCommand>().ReverseMap();
            CreateMap<Address, AddAddressRequest>().ReverseMap();
            CreateMap<Address, AddressCouchDTO>();


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
            CreateMap<PersonalInfo, AddAdoptionPersonalInfoRequest>().ReverseMap();
            // CreateMap<PersonalInfo, PersonalInfoByIdDTO>().ReverseMap();




            CreateMap<DeathEvent, DeathEventDTO>().ReverseMap();
            CreateMap<DeathEvent, AddDeathEventRequest>().ReverseMap();
            CreateMap<DeathEvent, CreateDeathEventCommand>().ReverseMap();
            CreateMap<DeathEvent, UpdateDeathEventCommand>()
            .ForMember(x => x.IsFromCommand, opt => opt.Ignore()).ReverseMap();
            CreateMap<BirthEvent, UpdateBirthEventCommand>()
            .ForMember(x => x.IsFromCommand, opt => opt.Ignore()).ReverseMap();
            CreateMap<UpdateBirthEventCommand, AddBirthEventRequest>().ReverseMap();
            CreateMap<UpdateDeathEventCommand, AddDeathEventRequest>().ReverseMap();

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
            CreateMap<Certificate, UpdateCertificateCommand>().ReverseMap();

            CreateMap<PaymentExamptionRequest, PaymentExamptionRequestDTO>().ReverseMap();
            CreateMap<PaymentExamptionRequest, PaymentExamptionRequestRequest>().ReverseMap();
            CreateMap<PaymentExamptionRequest, UpdatePaymentExamptionRequestCommand>().ReverseMap();
            CreateMap<PaymentExamptionRequest, CreatePaymentExamptionRequestCommand>().ReverseMap();


            CreateMap<CreateMarriageApplicationCommand, MarriageApplication>();
            CreateMap<MarriageApplication, MarriageApplicationGridDTO>();
            CreateMap<UpdateMarriageApplicationCommand, MarriageApplication>().ReverseMap();

            CreateMap<AddEventRequest, Event>().ReverseMap();
            CreateMap<AddWitnessRequest, Witness>().ReverseMap();

            CreateMap<AddRegistrarRequest, Registrar>();
            CreateMap<Registrar, RegistrarDTO>().ReverseMap();

            CreateMap<AddSupportingDocumentRequest, SupportingDocument>();
            CreateMap<AddPaymentExamptionDTO, PaymentExamption>();
            CreateMap<BirthEvent, AddBirthEventRequest>().ReverseMap();
            CreateMap<BirthEvent, BirthEventDTO>()
            .ReverseMap();

            CreateMap<BirthNotification, AddBirthNotificationRequest>().ReverseMap();
            CreateMap<BirthNotification, BirthNotificationDTO>().ReverseMap();

            CreateMap<CreateDivorceEventCommand, DivorceEvent>();
            CreateMap<UpdateDivorceEventCommand, DivorceEvent>().ReverseMap();

            CreateMap<CreateMarriageEventCommand, MarriageEvent>();
            CreateMap<UpdateMarriageEventCommand, MarriageEvent>().ReverseMap();


            CreateMap<AdoptionEvent, AddAdoptionRequest>().ReverseMap();
            CreateMap<AdoptionEvent, CreateAdoptionCommand>().ReverseMap();
            CreateMap<Event, AdoptionEventDTO>().ReverseMap();
            CreateMap<AdoptionEvent, UpdateAdoptionCommand>()
                .ForMember(dest => dest.IsFromCommand, opt => opt.Ignore())
                .ReverseMap();


            CreateMap<CourtCase, AddCourtCaseRequest>().ReverseMap();
            CreateMap<AdoptionEvent, AdoptionDTO>().ReverseMap();

            CreateMap<Court, AddCourtRequest>().ReverseMap();
            CreateMap<Court, CourtDTO>().ReverseMap();
            CreateMap<CourtCase, CourtCaseDTO>().ReverseMap();


            CreateMap<Witness, UpdateWitnessRequest>().ReverseMap();
            CreateMap<MotherInfoDTO, PersonalInfo>().ReverseMap();
            CreateMap<FatherInfoDTO, PersonalInfo>().ReverseMap();
            CreateMap<ChildInfoDTO, PersonalInfo>().ReverseMap();
            CreateMap<WitnessInfoDTO, PersonalInfo>().ReverseMap();
            CreateMap<DeadPersonalInfoDTO, PersonalInfo>().ReverseMap();
            CreateMap<RegistrarPersonalInfoDTO, PersonalInfo>().ReverseMap();
            CreateMap<BirthRegistrarPersonalInfoDTO, PersonalInfo>().ReverseMap();
            CreateMap<DivorcePartnersInfoDTO, PersonalInfo>().ReverseMap();
            CreateMap<GroomInfoDTO, PersonalInfo>().ReverseMap();

            CreateMap<BrideInfoDTO, PersonalInfo>().ReverseMap();

            CreateMap<SupportingDocument, UpdateSupportingDocumentRequest>().ReverseMap();
            CreateMap<SupportingDocument, AddSupportingDocumentRequest>().ReverseMap();
            CreateMap<PaymentExamption, UpdatePaymentExamptionRequest>().ReverseMap();
            CreateMap<Registrar, UpdateRegistrarRequest>().ReverseMap();
            CreateMap<AddEventForBirthRequest, Event>().ReverseMap();
            CreateMap<AddEventForDeathRequest, Event>().ReverseMap();
            CreateMap<AddEventForMarriageRequest, Event>().ReverseMap();
            CreateMap<AddEventForDivorceRequest, Event>().ReverseMap();
            CreateMap<RegistrarForBirthRequest, Registrar>().ReverseMap();
            CreateMap<RegistrarForDeathRequest, Registrar>().ReverseMap();
            CreateMap<RegistrarForMarriageRequest, Registrar>().ReverseMap();
            CreateMap<RegistrarForDivorceRequest, Registrar>().ReverseMap();
            CreateMap<CreatePaymentCommand, Payment>().ReverseMap();
            CreateMap<AddCertificateHistoryRequest, CertificateHistory>().ReverseMap();
            CreateMap<Request, AddRequest>().ReverseMap();
            CreateMap<CorrectionRequest, AddCorrectionRequest>().ReverseMap();
            CreateMap<Plan, AddPlanRequest>().ReverseMap();
            CreateMap<Plan, PlanDTO>().ReverseMap();
            CreateMap<Plan, UpdatePlanCommand>().ReverseMap();

            CreateMap<CertificateSerialTransfer, CertificateTransferDTO>().ReverseMap();
            CreateMap<CertificateSerialTransfer, AddCertificateTransferRequest>().ReverseMap();
            CreateMap<CertificateSerialTransfer, CreateCertificateTransferCommand>().ReverseMap();
            CreateMap<CertificateSerialTransfer, UpdateCertificateTransferCommand>().ReverseMap();



            CreateMap<Person, Officer>().ReverseMap();
            CreateMap<Person, DeceasedPerson>().ReverseMap();
            CreateMap<Person, RegistrarArchive>().ReverseMap();
            CreateMap<Person, AdoptedChild>().ReverseMap();
            CreateMap<Person, Child>().ReverseMap();
            CreateMap<Person, WitnessArchive>().ReverseMap();
            CreateMap<EventInfoArchive, AdoptionInfo>().ReverseMap();
            CreateMap<EventInfoArchive, BirthInfo>().ReverseMap();
            CreateMap<EventInfoArchive, DeathInfo>().ReverseMap();
            CreateMap<EventInfoArchive, MarriageInfo>().ReverseMap();
            CreateMap<EventInfoArchive, DivorceInfo>().ReverseMap();
            CreateMap<Transaction, TransactionRequestDTO>().ReverseMap();
            CreateMap<AuditLog, AuditLogDTO>().ReverseMap();
            CreateMap<PaymentRate , PaymentRateCouchDTO>();


            CreateMap<CreateMessageCommand , Message>().ReverseMap();   




            // CreateMap<WitnessArchive, Witness>().ReverseMap();


            CreateMap<PersonalInfoIndex, PersonalInfoSearchDTO>();
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
