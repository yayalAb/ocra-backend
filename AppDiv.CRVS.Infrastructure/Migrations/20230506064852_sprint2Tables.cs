using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDiv.CRVS.Infrastructure.Migrations
{
    public partial class sprint2Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Court",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NameStr = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AddressId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DescriptionStr = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModifiedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Court", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Court_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EventType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EventOwenerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EventDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EventRegDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EventAddressId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InformantTypeLookupId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CivilRegOfficerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsExampted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsCertified = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModifiedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_Addresses_EventAddressId",
                        column: x => x.EventAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Event_Lookups_InformantTypeLookupId",
                        column: x => x.InformantTypeLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Event_PersonalInfos_CivilRegOfficerId",
                        column: x => x.CivilRegOfficerId,
                        principalTable: "PersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Event_PersonalInfos_EventOwenerId",
                        column: x => x.EventOwenerId,
                        principalTable: "PersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MarriageApplication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ApplicationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ApplicationAddressId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    BrideInfoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GroomInfoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CivilRegOfficerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModifiedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarriageApplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarriageApplication_Addresses_ApplicationAddressId",
                        column: x => x.ApplicationAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarriageApplication_PersonalInfos_BrideInfoId",
                        column: x => x.BrideInfoId,
                        principalTable: "PersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarriageApplication_PersonalInfos_CivilRegOfficerId",
                        column: x => x.CivilRegOfficerId,
                        principalTable: "PersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarriageApplication_PersonalInfos_GroomInfoId",
                        column: x => x.GroomInfoId,
                        principalTable: "PersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PaymentExamptionRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ReasonStr = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExamptedClientId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExamptedClientFullNAme = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExamptedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ExamptedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumberOfClient = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModifiedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentExamptionRequest", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CourtCase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CourtId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CourtCaseNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConfirmedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModifiedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtCase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourtCase_Court_CourtId",
                        column: x => x.CourtId,
                        principalTable: "Court",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BirthEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FatherId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MotherId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FacilityTypeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FacilityId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    BirthPlaceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TypeOfBirthId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EventId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModifiedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BirthEvent_Addresses_BirthPlaceId",
                        column: x => x.BirthPlaceId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BirthEvent_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BirthEvent_Lookups_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BirthEvent_Lookups_FacilityTypeId",
                        column: x => x.FacilityTypeId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BirthEvent_Lookups_TypeOfBirthId",
                        column: x => x.TypeOfBirthId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BirthEvent_PersonalInfos_FatherId",
                        column: x => x.FatherId,
                        principalTable: "PersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BirthEvent_PersonalInfos_MotherId",
                        column: x => x.MotherId,
                        principalTable: "PersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Certificate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EventId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ContentStr = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AuthenticationStatus = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PrintCont = table.Column<int>(type: "int", nullable: false),
                    CertificateSerialNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModifiedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificate_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DeathEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FacilityTypeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FacilityId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DuringDeath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PlaceOfFuneral = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EventId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModifiedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeathEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeathEvent_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeathEvent_Lookups_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeathEvent_Lookups_FacilityTypeId",
                        column: x => x.FacilityTypeId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PaymentRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ReasonStr = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EventId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Amount = table.Column<float>(type: "float", nullable: false),
                    status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PaymentRateId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModifiedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentRequest_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentRequest_PaymentRates_PaymentRateId",
                        column: x => x.PaymentRateId,
                        principalTable: "PaymentRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Registrar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RelationshipId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RegistrarInfoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EventId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModifiedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Registrar_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Registrar_Lookups_RelationshipId",
                        column: x => x.RelationshipId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Registrar_PersonalInfos_RegistrarInfoId",
                        column: x => x.RegistrarInfoId,
                        principalTable: "PersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MarriageEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    BrideInfoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MarriageTypeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ApplicationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EventId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModifiedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarriageEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarriageEvent_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarriageEvent_Lookups_MarriageTypeId",
                        column: x => x.MarriageTypeId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarriageEvent_MarriageApplication_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "MarriageApplication",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MarriageEvent_PersonalInfos_BrideInfoId",
                        column: x => x.BrideInfoId,
                        principalTable: "PersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PaymentExamption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ExamptionRequestId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EventId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModifiedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentExamption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentExamption_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentExamption_PaymentExamptionRequest_ExamptionRequestId",
                        column: x => x.ExamptionRequestId,
                        principalTable: "PaymentExamptionRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AdoptionEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    BeforeAdoptionAddressId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AdoptiveMotherId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AdoptiveFatherId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CourtCaseId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EventId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ApprovedNameStr = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReasonStr = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModifiedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoptionEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdoptionEvent_Addresses_BeforeAdoptionAddressId",
                        column: x => x.BeforeAdoptionAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdoptionEvent_CourtCase_CourtCaseId",
                        column: x => x.CourtCaseId,
                        principalTable: "CourtCase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdoptionEvent_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdoptionEvent_PersonalInfos_AdoptiveFatherId",
                        column: x => x.AdoptiveFatherId,
                        principalTable: "PersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdoptionEvent_PersonalInfos_AdoptiveMotherId",
                        column: x => x.AdoptiveMotherId,
                        principalTable: "PersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DivorceEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DivorcedWifeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DataOfMarriage = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DivorceDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DivorceReasonStr = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CourtCaseId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NumberChildren = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModifiedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DivorceEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DivorceEvent_CourtCase_CourtCaseId",
                        column: x => x.CourtCaseId,
                        principalTable: "CourtCase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DivorceEvent_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DivorceEvent_PersonalInfos_DivorcedWifeId",
                        column: x => x.DivorcedWifeId,
                        principalTable: "PersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BirthNotification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    BirthEventId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DeliveryTypeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    WeghtAtBirth = table.Column<float>(type: "float", nullable: false),
                    SkilledProfId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NotficationSerialNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModifiedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BirthNotification_BirthEvent_BirthEventId",
                        column: x => x.BirthEventId,
                        principalTable: "BirthEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BirthNotification_Lookups_DeliveryTypeId",
                        column: x => x.DeliveryTypeId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BirthNotification_Lookups_SkilledProfId",
                        column: x => x.SkilledProfId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DeathNotification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CauseOfDeath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CauseOfDeathInfoTypeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DeathNotificationSerialNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeathEventId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModifiedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeathNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeathNotification_DeathEvent_DeathEventId",
                        column: x => x.DeathEventId,
                        principalTable: "DeathEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeathNotification_Lookups_CauseOfDeathInfoTypeId",
                        column: x => x.CauseOfDeathInfoTypeId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PaymentRequestId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PaymentWayLookupId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    BillNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModifiedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Lookups_PaymentWayLookupId",
                        column: x => x.PaymentWayLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_PaymentRequest_PaymentRequestId",
                        column: x => x.PaymentRequestId,
                        principalTable: "PaymentRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Witness",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    WitnessPersonalInfoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    WitnessFor = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MarriageEventId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ModifiedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Witness", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Witness_MarriageEvent_MarriageEventId",
                        column: x => x.MarriageEventId,
                        principalTable: "MarriageEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Witness_PersonalInfos_WitnessPersonalInfoId",
                        column: x => x.WitnessPersonalInfoId,
                        principalTable: "PersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionEvent_AdoptiveFatherId",
                table: "AdoptionEvent",
                column: "AdoptiveFatherId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionEvent_AdoptiveMotherId",
                table: "AdoptionEvent",
                column: "AdoptiveMotherId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionEvent_BeforeAdoptionAddressId",
                table: "AdoptionEvent",
                column: "BeforeAdoptionAddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionEvent_CourtCaseId",
                table: "AdoptionEvent",
                column: "CourtCaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionEvent_EventId",
                table: "AdoptionEvent",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BirthEvent_BirthPlaceId",
                table: "BirthEvent",
                column: "BirthPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_BirthEvent_EventId",
                table: "BirthEvent",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BirthEvent_FacilityId",
                table: "BirthEvent",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_BirthEvent_FacilityTypeId",
                table: "BirthEvent",
                column: "FacilityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BirthEvent_FatherId",
                table: "BirthEvent",
                column: "FatherId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BirthEvent_MotherId",
                table: "BirthEvent",
                column: "MotherId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BirthEvent_TypeOfBirthId",
                table: "BirthEvent",
                column: "TypeOfBirthId");

            migrationBuilder.CreateIndex(
                name: "IX_BirthNotification_BirthEventId",
                table: "BirthNotification",
                column: "BirthEventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BirthNotification_DeliveryTypeId",
                table: "BirthNotification",
                column: "DeliveryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BirthNotification_SkilledProfId",
                table: "BirthNotification",
                column: "SkilledProfId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_EventId",
                table: "Certificate",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Court_AddressId",
                table: "Court",
                column: "AddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourtCase_CourtId",
                table: "CourtCase",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_DeathEvent_EventId",
                table: "DeathEvent",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeathEvent_FacilityId",
                table: "DeathEvent",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_DeathEvent_FacilityTypeId",
                table: "DeathEvent",
                column: "FacilityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DeathNotification_CauseOfDeathInfoTypeId",
                table: "DeathNotification",
                column: "CauseOfDeathInfoTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DeathNotification_DeathEventId",
                table: "DeathNotification",
                column: "DeathEventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DivorceEvent_CourtCaseId",
                table: "DivorceEvent",
                column: "CourtCaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DivorceEvent_DivorcedWifeId",
                table: "DivorceEvent",
                column: "DivorcedWifeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DivorceEvent_EventId",
                table: "DivorceEvent",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_CivilRegOfficerId",
                table: "Event",
                column: "CivilRegOfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventAddressId",
                table: "Event",
                column: "EventAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventOwenerId",
                table: "Event",
                column: "EventOwenerId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_InformantTypeLookupId",
                table: "Event",
                column: "InformantTypeLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_MarriageApplication_ApplicationAddressId",
                table: "MarriageApplication",
                column: "ApplicationAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_MarriageApplication_BrideInfoId",
                table: "MarriageApplication",
                column: "BrideInfoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarriageApplication_CivilRegOfficerId",
                table: "MarriageApplication",
                column: "CivilRegOfficerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarriageApplication_GroomInfoId",
                table: "MarriageApplication",
                column: "GroomInfoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarriageEvent_ApplicationId",
                table: "MarriageEvent",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarriageEvent_BrideInfoId",
                table: "MarriageEvent",
                column: "BrideInfoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarriageEvent_EventId",
                table: "MarriageEvent",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarriageEvent_MarriageTypeId",
                table: "MarriageEvent",
                column: "MarriageTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PaymentRequestId",
                table: "Payment",
                column: "PaymentRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PaymentWayLookupId",
                table: "Payment",
                column: "PaymentWayLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentExamption_EventId",
                table: "PaymentExamption",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentExamption_ExamptionRequestId",
                table: "PaymentExamption",
                column: "ExamptionRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequest_EventId",
                table: "PaymentRequest",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequest_PaymentRateId",
                table: "PaymentRequest",
                column: "PaymentRateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registrar_EventId",
                table: "Registrar",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registrar_RegistrarInfoId",
                table: "Registrar",
                column: "RegistrarInfoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registrar_RelationshipId",
                table: "Registrar",
                column: "RelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_Witness_MarriageEventId",
                table: "Witness",
                column: "MarriageEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Witness_WitnessPersonalInfoId",
                table: "Witness",
                column: "WitnessPersonalInfoId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdoptionEvent");

            migrationBuilder.DropTable(
                name: "BirthNotification");

            migrationBuilder.DropTable(
                name: "Certificate");

            migrationBuilder.DropTable(
                name: "DeathNotification");

            migrationBuilder.DropTable(
                name: "DivorceEvent");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "PaymentExamption");

            migrationBuilder.DropTable(
                name: "Registrar");

            migrationBuilder.DropTable(
                name: "Witness");

            migrationBuilder.DropTable(
                name: "BirthEvent");

            migrationBuilder.DropTable(
                name: "DeathEvent");

            migrationBuilder.DropTable(
                name: "CourtCase");

            migrationBuilder.DropTable(
                name: "PaymentRequest");

            migrationBuilder.DropTable(
                name: "PaymentExamptionRequest");

            migrationBuilder.DropTable(
                name: "MarriageEvent");

            migrationBuilder.DropTable(
                name: "Court");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "MarriageApplication");
        }
    }
}
