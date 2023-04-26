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
            CreateMap<Workflow, CreateWorkFlowCommand>().ReverseMap();

            CreateMap<Step, StepDTO>().ReverseMap();
        




            CreateMap<ApplicationUser, UserResponseDTO>().ReverseMap();
            CreateMap<ApplicationUser, CreateUserCommand>().ReverseMap();
            CreateMap<ApplicationUser, UpdateUserCommand>().ReverseMap();

            // CreateMap<PersonalInfo, PersonalInfoDTO>().ReverseMap();
            // CreateMap<ContactInfo, ContactInfoDTO>().ReverseMap();

            CreateMap<PersonalInfo, AddPersonalInfoRequest>().ReverseMap();
            CreateMap<ContactInfo, AddContactInfoRequest>().ReverseMap();
            CreateMap<ApplicationUser, UpdateUserCommand>().ReverseMap();

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
