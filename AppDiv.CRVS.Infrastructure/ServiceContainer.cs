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
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Infrastructure.Extensions;
using AppDiv.CRVS.Infrastructure.Service;
using CouchDB.Driver.DependencyInjection;
using AppDiv.CRVS.Infrastructure.Context;
using AppDiv.CRVS.Application.Persistence.Couch;
using AppDiv.CRVS.Infrastructure.Persistence.Couch;
using AppDiv.CRVS.Application.Interfaces.Persistence.Couch;
using Hangfire;
// using Hangfire.MySql.Core;
// using Hangfire.MySql.Core;
using System.Data;
using Hangfire.MySql;
// using System.Data;
// using Hangfire.MySqlStorage;
// using Hangfire.Core;
// using Hangfire.MySql.Core_MySql.Data;
// using AppDiv.CRVS.Infrastructure.Extensions;

namespace AppDiv.CRVS.Infrastructure
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
        {
            #region  db configuration


            // services.AddDbContext<CRVSDbContext>(
            //     options => options.UseSqlServer(
            //         configuration.GetConnectionString("CRVSConnectionString"),
            //         o => o.MigrationsAssembly(typeof(ServiceContainer).Assembly.FullName)
            //     ).EnableSensitiveDataLogging()
            // );
            services.AddDbContext<CRVSDbContext>(
                options =>
                {
                    // options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                    options.UseMySql(configuration.GetConnectionString("CRVSConnectionString"),
                        Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.29-mysql"),
                        mySqlOptions => mySqlOptions.EnableRetryOnFailure());
                    // .EnableSensitiveDataLogging();
                });

            #endregion db configuration

            // #region  elasticSearch    

            services.AddElasticSearch(configuration);
            // #endregion elasticSearch
            #region  couch db config

            var couchUrl = configuration["CouchDB:URL"];
            var couchDb = configuration["CouchDB:DbName"];
            var couchUser = configuration["CouchDB:UserName"];
            var couchPassword = configuration["CouchDB:Password"];
            services.AddCouchContext<CRVSCouchDbContext>(builder => builder
                .UseEndpoint(couchUrl)
                .UseBasicAuthentication(username: couchUser, password: couchPassword));

            #endregion couch db config

            #region  identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                      .AddEntityFrameworkStores<CRVSDbContext>()
                      .AddDefaultTokenProviders();


            services.Configure<IdentityOptions>(options =>
               {
                   // Default Lockout settings.
                   options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(99999);
                   options.Lockout.MaxFailedAccessAttempts = 5;
                   options.Lockout.AllowedForNewUsers = true;
                   // Default Password settings.
                   options.Password.RequireDigit = false;
                   options.Password.RequireLowercase = false;
                   options.Password.RequireNonAlphanumeric = false; // For special character
                   options.Password.RequireUppercase = false;
                   options.Password.RequiredLength = 3;
                   options.Password.RequiredUniqueChars = 0;

                   // //TODO:add policy object names
                   // options.Password.RequireDigit = passwordpolicy.Value<bool>("number");
                   // options.Password.RequireLowercase = passwordpolicy.Value<bool>("lowerCase");
                   // options.Password.RequireNonAlphanumeric = passwordpolicy.Value<bool>("otherCharacter"); // For special character
                   // options.Password.RequireUppercase = passwordpolicy.Value<bool>("upperCase");
                   // options.Password.RequiredLength = passwordpolicy.Value<int>("minLength");
                   // options.Password.RequiredUniqueChars = 0;
                   // Default SignIn settings.
                   options.SignIn.RequireConfirmedEmail = false;
                   options.SignIn.RequireConfirmedPhoneNumber = false;
                   options.User.RequireUniqueEmail = true;
               });
            #endregion identity

            #region hangfire
            string hangfireConnectionString = configuration.GetConnectionString("HangFireConnectionString");
            services.AddHangfire(configuration => configuration
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseStorage(
                new MySqlStorage(
                    hangfireConnectionString,
                    new MySqlStorageOptions
                    {
                        TransactionIsolationLevel = (System.Transactions.IsolationLevel?)IsolationLevel.ReadCommitted,
                        QueuePollInterval = TimeSpan.FromSeconds(15),
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),
                        CountersAggregateInterval = TimeSpan.FromSeconds(5),//TODO: option config/
                        PrepareSchemaIfNecessary = true,
                        DashboardJobListLimit = 50000,

                    }
                ))
            );
            // Add the processing server as IHostedService
            services.AddHangfireServer();
            #endregion hangfire


            // services.Configure<RabbitMQConfiguration>(configuration.GetSection(RabbitMQConfiguration.CONFIGURATION_SECTION));
            services.Configure<SMTPServerConfiguration>(configuration.GetSection(SMTPServerConfiguration.CONFIGURATION_SECTION));
            services.Configure<TwilioConfiguration>(configuration.GetSection(TwilioConfiguration.CONFIGURATION_SECTION));
            services.Configure<AfroMessageConfiguration>(configuration.GetSection(AfroMessageConfiguration.CONFIGURATION_SECTION));



            services.AddSingleton<IUserResolverService, UserResolverService>();
            services.AddSingleton<IFileService, FileService>();

            services.AddSingleton<IMailService, MailKitService>();
            services.AddSingleton<ISmsService, TwilioService>();
            services.AddSingleton<ISmsService, AfroMessageService>();




            #region Repositories DI         

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ILookupRepository, LookupRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IPersonalInfoRepository, PersonalInfoRepository>();
            services.AddScoped<IContactInfoRepository, ContactInfoRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IWorkflowRepository, WorkflowRepository>();
            services.AddScoped<ICourtRepository, CourtRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IStepRepository, StepRepository>();
            services.AddTransient<ICertificateRepository, CertificateRepository>();
            services.AddTransient<IPaymentExamptionRequestRepository, PaymentExamptionRequestRepository>();

            services.AddTransient<IMarriageApplicationRepository, MarriageApplicationRepository>();
            services.AddTransient<IMarriageEventRepository, MarriageEventRepository>();
            services.AddTransient<IAdoptionEventRepository, AdoptionEventRepository>();
            services.AddScoped<IDeathEventRepository, DeathEventRepository>();
            services.AddScoped<IBirthEventRepository, BirthEventRepository>();
            services.AddTransient<IDivorceEventRepository, DivorceEventRepository>();

            services.AddTransient<ICertificateTemplateRepository, CertificateTemplateRepository>();
            services.AddTransient<IPaymentRateRepository, PaymentRateRepository>();
            services.AddTransient<IPaymentRepository, PaymentRepository>();
            services.AddTransient<IPaymentRequestRepository, PaymentRequestRepository>();
            services.AddTransient<ISupportingDocumentRepository, SupportingDocumentRepository>();

            services.AddTransient<IEventRepository, EventRepository>();
            services.AddTransient<IPlanRepository, PlanRepository>();
            services.AddTransient<IReportRepostory, ReportRepostory>();
            services.AddTransient<IReportStoreRepostory, ReportStoreRepostory>();
            services.AddTransient<IReportStoreRepostory, ReportStoreRepostory>();
            services.AddTransient<IFingerprintApiKeyRepostory, FingerprintApiKeyRepostory>();


            services.AddScoped<CRVSDbContextInitializer>();
            services.AddScoped<IAddressLookupRepository, AddressLookupRepository>();
            services.AddHttpClient<ITwilioRestClient, TwilioClient>();
            services.AddScoped<IDateAndAddressService, DateAndAddressService>();
            services.AddScoped<IDateAndAddressService, DateAndAddressService>();
            services.AddScoped<ILookupFromId, LookupFromId>();
            services.AddScoped<ICertificateGenerator, CertificateGenerator>();
            services.AddScoped<IArchiveGenerator, ArchiveGenerator>();
            services.AddScoped<IAddressLookupRepository, AddressLookupRepository>();
            services.AddScoped<ICertificateHistoryRepository, CertificateHistoryRepository>();
            services.AddScoped<ICertificateTransferRepository, CertificateTransferRepository>();
            services.AddScoped<ICertificateRangeRepository, CertificateRangeRepository>();
            services.AddScoped<ICorrectionRequestRepostory, CorrectionRequestRepostory>();
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            services.AddScoped<IRequestRepostory, RequestRepostory>();
            services.AddScoped<ILookupCouchRepository, LookupCouchRepository>();
            services.AddScoped<IAddressLookupCouchRepository, AddressLookupCouchRepository>();
            services.AddScoped<ISettingCouchRepository, SettingCouchRepository>();
            services.AddScoped<IPaymentRateCouchRepository, PaymentRateCouchRepository>();

            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IOnlineUserRepository, OnlineUserRepository>();
            services.AddScoped<IMyReportRepository, MyReportRepository>();
            services.AddScoped<IReportStoreRepostory, ReportStoreRepostory>();




            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IUpdateEventPaymetnService, UpdateEventPaymetnService>();
            services.AddTransient<ILoginHistoryRepository, LoginHistoryRepository>();
            services.AddTransient<IAuditLogRepository, AuditLogRepository>();
            services.AddTransient<IRevocationTokenRepository, RevocationTokenRepository>();
            services.AddTransient<IVerficationRequestRepository, VerficationRequestRepository>();
            services.AddScoped<IWorkHistoryRepository, WorkHistoryRepository>();
            services.AddScoped<ITokenValidatorService, TokenValidatorService>();
            services.AddScoped<IBackgroundJobs, BackgroundJobs>();
            services.AddScoped<IPersonDuplicateRepository, PersonDuplicateRepository>();
            services.AddScoped<IEventDuplicateRepository, EventDuplicateRepository>();



            // services.AddScoped<IReturnAdoptionCertfcate, ReturnAdoptionCertfcate>();
            #endregion Repositories DI

            return services;
        }
    }
}
