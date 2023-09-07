using System;
using Ab.Domain.Configuration.Settings;
using AppDiv.CRVS.Domain.Configuration.Settings;
using AppDiv.CRVS.Domain.Entities.Audit;
using AppDiv.CRVS.Domain.Entities.Settings;
using AppDiv.CRVS.Infrastructure.Context;
using Audit.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Newtonsoft.Json;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Infrastructure.Seed;
using Audit.EntityFramework;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using AppDiv.CRVS.Domain.Configuration;
using AppDiv.CRVS.Domain.Configurations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppDiv.CRVS.Infrastructure
{
    public class CRVSDbContext : AuditIdentityDbContext<ApplicationUser>, ICRVSDbContext
    {
        private readonly IUserResolverService userResolverService;

        public DbSet<Lookup> Lookups { get; set; }
        public DbSet<PersonalInfo> PersonalInfos { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<CertificateTemplate> CertificateTemplates { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }

        public DbSet<AdoptionEvent> AdoptionEvents { get; set; }
        public DbSet<BirthEvent> BirthEvents { get; set; }
        public DbSet<BirthNotification> BirthNotifications { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        // public DbSet<Court> Courts { get; set; }
        public DbSet<CourtCase> CourtCases { get; set; }
        public DbSet<DeathEvent> DeathEvents { get; set; }
        public DbSet<DeathNotification> DeathNotifications { get; set; }
        public DbSet<DivorceEvent> DivorceEvents { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<PaymentRate> PaymentRates { get; set; }
        public DbSet<MarriageEvent> MarriageEvents { get; set; }
        public DbSet<MarriageApplication> MarriageApplications { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentExamption> PaymentExamptions { get; set; }
        public DbSet<PaymentExamptionRequest> PaymentExamptionRequests { get; set; }
        public DbSet<PaymentRequest> PaymentRequests { get; set; }
        public DbSet<Registrar> Registrars { get; set; }
        public DbSet<SupportingDocument> SupportingDocuments { get; set; }
        public DbSet<Witness> Witnesses { get; set; }
        public DbSet<CertificateHistory> CertificateHistories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<CertificateSerialRange> CertificateSerialRanges { get; set; }
        public DbSet<CertificateSerialTransfer> CertificateSerialTransfers { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<LoginHistory> LoginHistorys { get; set; }
        public DbSet<VerficationRequest> VerficationRequests { get; set; }
        public DbSet<WorkHistory> WorkHistories { get; set; }
        public DbSet<RevocationToken> RevocationTokens { get; set; }

        public DbSet<Message> Messages { get; set; }
        public DbSet<OnlineUser> OnlineUsers { get; set; }
        public DbSet<MyReports> MyReports { get; set; }
        public DbSet<ReportStore> ReportStores { get; set; }
        public DbSet<FingerprintApiKey> FingerprintApiKeys { get; set; }
        public DbSet<EventDuplicate> EventDuplicates { get; set; }
        public DbSet<PersonDuplicate> PersonDuplicates { get; set; }
        public CRVSDbContext(DbContextOptions<CRVSDbContext> options, IUserResolverService userResolverService) : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
            this.userResolverService = userResolverService;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            // To run sql scripts, example alter database to set collation, create stored procedure, function, view ....
            // optionsBuilder.ReplaceService<IMigrationsSqlGenerator, CustomSqlServerMigrationsSqlGenerator>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            #region Entity Configuration
            {
                modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
                modelBuilder.ApplyConfiguration(new UserGroupEntityConfiguration());
                // modelBuilder.ApplyConfiguration(new WorkHistoryEntityConfiguration());
                modelBuilder.ApplyConfiguration(new PersonalInfoEntityConfiguration());
                modelBuilder.ApplyConfiguration(new LookupEntityConfiguration());
                modelBuilder.ApplyConfiguration(new AddressEntityConfiguration());

                modelBuilder.ApplyConfiguration(new PaymentEntityConfiguration());
                modelBuilder.ApplyConfiguration(new PaymentExamptionEntityConfiguration());
                modelBuilder.ApplyConfiguration(new PaymentExamptionRequestEntityConfiguration());
                modelBuilder.ApplyConfiguration(new PaymentRequestEntityConfiguration());

                modelBuilder.ApplyConfiguration(new SupportingDocumentEntityConfiguration());
                modelBuilder.ApplyConfiguration(new CertificateEntityConfiguration());
                modelBuilder.ApplyConfiguration(new RegistrarEntityConfiguration());

                modelBuilder.ApplyConfiguration(new EventEntityConfiguration());

                modelBuilder.ApplyConfiguration(new MarriageEventEntityConfiguration());
                modelBuilder.ApplyConfiguration(new MarriageApplicationEntityConfiguration());
                modelBuilder.ApplyConfiguration(new WitnessEntityConfiguration());

                modelBuilder.ApplyConfiguration(new AdoptionEventEntityConfiguration());
                modelBuilder.ApplyConfiguration(new DivorceEventEntityConfiguration());

                modelBuilder.ApplyConfiguration(new BirthEventEntityConfiguration());
                modelBuilder.ApplyConfiguration(new BirthNotficationEntityConfiguration());

                modelBuilder.ApplyConfiguration(new DeathEventEntityConfiguration());
                modelBuilder.ApplyConfiguration(new DeathNotificationEntityConfiguration());

                modelBuilder.ApplyConfiguration(new TransactionEntityConfiguration());
                modelBuilder.ApplyConfiguration(new RequestEntityConfiguration());
                modelBuilder.ApplyConfiguration(new CertificateSerialTransferEntityConfiguration());
                modelBuilder.ApplyConfiguration(new AuthenticationRequestConfiguration());
                modelBuilder.ApplyConfiguration(new PlanEntityConfiguration());
                modelBuilder.ApplyConfiguration(new MessageEntityConfiguration());





            }
            #endregion
            base.OnModelCreating(modelBuilder);

            // SeedData.SeedRoles(modelBuilder);
            // SeedData.SeedUsers(modelBuilder);
            // SeedData.SeedUserRoles(modelBuilder);
            // SeedData.SeedGender(modelBuilder);
            // SeedData.SeedSuffix(modelBuilder);


            #region Audit Config
            Audit.Core.Configuration.Setup()
                .UseEntityFramework(config => config
                .AuditTypeMapper(t => typeof(AuditLog))
                .AuditEntityAction<AuditLog>((auditEvent, auditedEntity, auditEntity) =>
                {
                    auditEntity.AuditData = JsonConvert.SerializeObject(auditedEntity, GetJsonSerializerSettings());
                    auditEntity.EntityType = auditedEntity.EntityType.Name;
                    auditEntity.AuditDate = DateTime.Now;
                    auditEntity.AuditUserId = string.Empty;
                    auditEntity.AddressId = Guid.NewGuid();
                    if (userResolverService != null)
                    {
                        var userId = userResolverService.GetUserId();
                        var workingAddressId = userResolverService.GetWorkingAddressId();

                        auditEntity.AuditUserId = userId is not null
                                ? userId
                                : string.Empty;
                        auditEntity.AddressId = workingAddressId;
                    }

                    auditEntity.Action = auditedEntity.Action;
                    auditEntity.Enviroment = JsonConvert.SerializeObject(new AuditEventEnvironment
                    {
                        UserName = auditEvent.Environment.UserName,
                        MachineName = auditEvent.Environment.MachineName,
                        DomainName = auditEvent.Environment.DomainName,
                        CallingMethodName = auditEvent.Environment.CallingMethodName,
                        Exception = auditEvent.Environment.Exception,
                        Culture = auditEvent.Environment.Culture
                    }, GetJsonSerializerSettings());
                    /*if (auditedEntity.EntityType == typeof(test))
                    {
                        var json = auditEntity.AuditData.Replace("'", "\"");
                        var jo = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);
                        var referenceNo = jo["ColumnValues"]["ReferenceNumber"];
                        auditEntity.TablePk = jo["ColumnValues"]["ReferenceNumber"].ToString();
                    }*/
                    //else
                    {
                        auditEntity.TablePk = auditedEntity.PrimaryKey.First().Value.ToString();
                    }
                }).IgnoreMatchedProperties(true));
            #endregion
        }

        public static JsonSerializerSettings GetJsonSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatString = "yyyy-MM-dd hh:mm:ss",
                StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                Formatting = Newtonsoft.Json.Formatting.Indented
            };
        }

        public Guid GetCurrentUserId()
        {
            var userId = userResolverService.GetUserId();
            return userId != null ? new Guid(userId) : Guid.Empty;

        }
    }
}
