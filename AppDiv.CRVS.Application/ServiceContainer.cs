using Microsoft.Extensions.DependencyInjection;
using AppDiv.CRVS.Application.Interfaces;
using System.Reflection;
using MediatR;
using AppDiv.CRVS.Application.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using AppDiv.CRVS.Application.Common.Behaviours;
using FluentValidation;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces.Archive;
using AppDiv.CRVS.Application.Service.ArchiveService;

namespace AppDiv.CRVS.Application
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, ConfigurationManager configuration)
        {



            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient);

            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IEventDocumentService, EventDocumentService>();
            services.AddScoped<IEventPaymentRequestService, EventPaymentRequestService>();
            services.AddScoped<IReturnAdoptionCertfcate, ReturnAdoptionCertfcate>();
            services.AddScoped<IReturnDeathCertificate, ReturnDeathCertificate>();
            services.AddScoped<IReturnBirthCertificate, ReturnBirthCertificate>();
            services.AddScoped<IReturnMarriageCertificate, ReturnMarriageCertificate>();
            services.AddScoped<IReturnDivorceCertificate, ReturnDivorceCertificate>();

            services.AddScoped<IReturnAdoptionArchive, ReturnAdoptionArchive>();
            services.AddScoped<IReturnDeathArchive, ReturnDeathArchive>();
            services.AddScoped<IReturnBirthArchive, ReturnBirthArchive>();
            services.AddScoped<IReturnMarriageArchive, ReturnMarriageArchive>();
            services.AddScoped<IReturnDivorceArchive, ReturnDivorceArchive>();
            services.AddScoped<IWorkflowService, WorkflowService>();
            services.AddScoped<IReturnVerficationList, ReturnVerficationList>();
            services.AddScoped<IMergeAndSplitAddressService, MergeAndSplitAddressService>();

            services.AddScoped<IContentValidator, ContentValidator>();
            services.AddScoped<IWorkHistoryTracker, WorkHistoryTracker>();
            services.AddScoped<HelperService>();


            return services;
        }
    }
}
