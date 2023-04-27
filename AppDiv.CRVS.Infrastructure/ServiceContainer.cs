using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Utility.Config;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Infrastructure.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Infrastructure.Services;
using AppDiv.CRVS.Utility.Services;
using Twilio.Clients;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Infrastructure
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
        {
            // services.AddDbContext<CRVSDbContext>(
            //     options => options.UseSqlServer(
            //         configuration.GetConnectionString("CRVSConnectionString"),
            //         o => o.MigrationsAssembly(typeof(ServiceContainer).Assembly.FullName)
            //     ).EnableSensitiveDataLogging()
            // );
            services.AddDbContext<CRVSDbContext>(
                options =>
            options.UseMySql(configuration.GetConnectionString("CRVSConnectionString"),
                  Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.29-mysql"),
                  mySqlOptions => mySqlOptions.EnableRetryOnFailure()));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                      .AddEntityFrameworkStores<CRVSDbContext>()
                      .AddDefaultTokenProviders();


            services.Configure<IdentityOptions>(options =>
            {
                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                // Default Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false; // For special character
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 0;
                // Default SignIn settings.
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.User.RequireUniqueEmail = true;
            });

            // services.Configure<RabbitMQConfiguration>(configuration.GetSection(RabbitMQConfiguration.CONFIGURATION_SECTION));
            services.Configure<SMTPServerConfiguration>(configuration.GetSection(SMTPServerConfiguration.CONFIGURATION_SECTION));
            services.Configure<TwilioConfiguration>(configuration.GetSection(TwilioConfiguration.CONFIGURATION_SECTION));




            services.AddSingleton<IUserResolverService, UserResolverService>();
            services.AddSingleton<IFileService, FileService>();

            services.AddSingleton<IMailService, MailKitService>();
            services.AddSingleton<ISmsService, TwilioService>();



            #region Repositories DI         

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ILookupRepository, LookupRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddTransient<IPersonalInfoRepository, PersonalInfoRepository>();
            services.AddTransient<IContactInfoRepository, ContactInfoRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IWorkflowRepository, WorkflowRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IStepRepository, StepRepository>();


            services.AddTransient<ICertificateTemplateRepository, CertificateTemplateRepository>();
            services.AddTransient<IPaymentRateRepository, PaymentRateRepository>();

            services.AddScoped<CRVSDbContextInitializer>(); services.AddScoped<IAddressLookupRepository, AddressLookupRepository>();
            services.AddHttpClient<ITwilioRestClient, TwilioClient>();
            #endregion Repositories DI

            return services;
        }
    }
}
