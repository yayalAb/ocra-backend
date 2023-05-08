using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDiv.CRVS.Infrastructure.Migrations
{
    public partial class sprint2Tables3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvent_Addresses_BeforeAdoptionAddressId",
                table: "AdoptionEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvent_CourtCase_CourtCaseId",
                table: "AdoptionEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvent_Event_EventId",
                table: "AdoptionEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvent_PersonalInfos_AdoptiveFatherId",
                table: "AdoptionEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvent_PersonalInfos_AdoptiveMotherId",
                table: "AdoptionEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_BirthEvent_Addresses_BirthPlaceId",
                table: "BirthEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_BirthEvent_Event_EventId",
                table: "BirthEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_BirthEvent_Lookups_FacilityId",
                table: "BirthEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_BirthEvent_Lookups_FacilityTypeId",
                table: "BirthEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_BirthEvent_Lookups_TypeOfBirthId",
                table: "BirthEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_BirthEvent_PersonalInfos_FatherId",
                table: "BirthEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_BirthEvent_PersonalInfos_MotherId",
                table: "BirthEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_BirthNotification_BirthEvent_BirthEventId",
                table: "BirthNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_BirthNotification_Lookups_DeliveryTypeId",
                table: "BirthNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_BirthNotification_Lookups_SkilledProfId",
                table: "BirthNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificate_Event_EventId",
                table: "Certificate");

            migrationBuilder.DropForeignKey(
                name: "FK_Court_Addresses_AddressId",
                table: "Court");

            migrationBuilder.DropForeignKey(
                name: "FK_CourtCase_Court_CourtId",
                table: "CourtCase");

            migrationBuilder.DropForeignKey(
                name: "FK_DeathEvent_Event_EventId",
                table: "DeathEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_DeathEvent_Lookups_FacilityId",
                table: "DeathEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_DeathEvent_Lookups_FacilityTypeId",
                table: "DeathEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_DeathNotification_DeathEvent_DeathEventId",
                table: "DeathNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_DeathNotification_Lookups_CauseOfDeathInfoTypeId",
                table: "DeathNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_DivorceEvent_CourtCase_CourtCaseId",
                table: "DivorceEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_DivorceEvent_Event_EventId",
                table: "DivorceEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_DivorceEvent_PersonalInfos_DivorcedWifeId",
                table: "DivorceEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Addresses_EventAddressId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Lookups_InformantTypeLookupId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_PersonalInfos_CivilRegOfficerId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_PersonalInfos_EventOwenerId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_MarriageApplication_Addresses_ApplicationAddressId",
                table: "MarriageApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_MarriageApplication_PersonalInfos_BrideInfoId",
                table: "MarriageApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_MarriageApplication_PersonalInfos_CivilRegOfficerId",
                table: "MarriageApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_MarriageApplication_PersonalInfos_GroomInfoId",
                table: "MarriageApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_MarriageEvent_Event_EventId",
                table: "MarriageEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_MarriageEvent_Lookups_MarriageTypeId",
                table: "MarriageEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_MarriageEvent_MarriageApplication_ApplicationId",
                table: "MarriageEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_MarriageEvent_PersonalInfos_BrideInfoId",
                table: "MarriageEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Lookups_PaymentWayLookupId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_PaymentRequest_PaymentRequestId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentExamption_Event_EventId",
                table: "PaymentExamption");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentExamption_PaymentExamptionRequest_ExamptionRequestId",
                table: "PaymentExamption");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentRequest_Event_EventId",
                table: "PaymentRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentRequest_PaymentRates_PaymentRateId",
                table: "PaymentRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrar_Event_EventId",
                table: "Registrar");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrar_Lookups_RelationshipId",
                table: "Registrar");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrar_PersonalInfos_RegistrarInfoId",
                table: "Registrar");

            migrationBuilder.DropForeignKey(
                name: "FK_SupportingDocument_Event_EventId",
                table: "SupportingDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_Witness_MarriageEvent_MarriageEventId",
                table: "Witness");

            migrationBuilder.DropForeignKey(
                name: "FK_Witness_PersonalInfos_WitnessPersonalInfoId",
                table: "Witness");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Witness",
                table: "Witness");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SupportingDocument",
                table: "SupportingDocument");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Registrar",
                table: "Registrar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentRequest",
                table: "PaymentRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentExamptionRequest",
                table: "PaymentExamptionRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentExamption",
                table: "PaymentExamption");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                table: "Payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MarriageEvent",
                table: "MarriageEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MarriageApplication",
                table: "MarriageApplication");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Event",
                table: "Event");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DivorceEvent",
                table: "DivorceEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeathNotification",
                table: "DeathNotification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeathEvent",
                table: "DeathEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourtCase",
                table: "CourtCase");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Court",
                table: "Court");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Certificate",
                table: "Certificate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BirthNotification",
                table: "BirthNotification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BirthEvent",
                table: "BirthEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdoptionEvent",
                table: "AdoptionEvent");

            migrationBuilder.RenameTable(
                name: "Witness",
                newName: "Witnesses");

            migrationBuilder.RenameTable(
                name: "SupportingDocument",
                newName: "SupportingDocuments");

            migrationBuilder.RenameTable(
                name: "Registrar",
                newName: "Registrars");

            migrationBuilder.RenameTable(
                name: "PaymentRequest",
                newName: "PaymentRequests");

            migrationBuilder.RenameTable(
                name: "PaymentExamptionRequest",
                newName: "PaymentExamptionRequests");

            migrationBuilder.RenameTable(
                name: "PaymentExamption",
                newName: "PaymentExamptions");

            migrationBuilder.RenameTable(
                name: "Payment",
                newName: "Payments");

            migrationBuilder.RenameTable(
                name: "MarriageEvent",
                newName: "MarriageEvents");

            migrationBuilder.RenameTable(
                name: "MarriageApplication",
                newName: "MarriageApplications");

            migrationBuilder.RenameTable(
                name: "Event",
                newName: "Events");

            migrationBuilder.RenameTable(
                name: "DivorceEvent",
                newName: "DivorceEvents");

            migrationBuilder.RenameTable(
                name: "DeathNotification",
                newName: "DeathNotifications");

            migrationBuilder.RenameTable(
                name: "DeathEvent",
                newName: "DeathEvents");

            migrationBuilder.RenameTable(
                name: "CourtCase",
                newName: "CourtCases");

            migrationBuilder.RenameTable(
                name: "Court",
                newName: "Courts");

            migrationBuilder.RenameTable(
                name: "Certificate",
                newName: "Certificates");

            migrationBuilder.RenameTable(
                name: "BirthNotification",
                newName: "BirthNotifications");

            migrationBuilder.RenameTable(
                name: "BirthEvent",
                newName: "birthEvents");

            migrationBuilder.RenameTable(
                name: "AdoptionEvent",
                newName: "AdoptionEvents");

            migrationBuilder.RenameIndex(
                name: "IX_Witness_WitnessPersonalInfoId",
                table: "Witnesses",
                newName: "IX_Witnesses_WitnessPersonalInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_Witness_MarriageEventId",
                table: "Witnesses",
                newName: "IX_Witnesses_MarriageEventId");

            migrationBuilder.RenameIndex(
                name: "IX_SupportingDocument_EventId",
                table: "SupportingDocuments",
                newName: "IX_SupportingDocuments_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Registrar_RelationshipId",
                table: "Registrars",
                newName: "IX_Registrars_RelationshipId");

            migrationBuilder.RenameIndex(
                name: "IX_Registrar_RegistrarInfoId",
                table: "Registrars",
                newName: "IX_Registrars_RegistrarInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_Registrar_EventId",
                table: "Registrars",
                newName: "IX_Registrars_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentRequest_PaymentRateId",
                table: "PaymentRequests",
                newName: "IX_PaymentRequests_PaymentRateId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentRequest_EventId",
                table: "PaymentRequests",
                newName: "IX_PaymentRequests_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentExamption_ExamptionRequestId",
                table: "PaymentExamptions",
                newName: "IX_PaymentExamptions_ExamptionRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentExamption_EventId",
                table: "PaymentExamptions",
                newName: "IX_PaymentExamptions_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_PaymentWayLookupId",
                table: "Payments",
                newName: "IX_Payments_PaymentWayLookupId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_PaymentRequestId",
                table: "Payments",
                newName: "IX_Payments_PaymentRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_MarriageEvent_MarriageTypeId",
                table: "MarriageEvents",
                newName: "IX_MarriageEvents_MarriageTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_MarriageEvent_EventId",
                table: "MarriageEvents",
                newName: "IX_MarriageEvents_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_MarriageEvent_BrideInfoId",
                table: "MarriageEvents",
                newName: "IX_MarriageEvents_BrideInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_MarriageEvent_ApplicationId",
                table: "MarriageEvents",
                newName: "IX_MarriageEvents_ApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_MarriageApplication_GroomInfoId",
                table: "MarriageApplications",
                newName: "IX_MarriageApplications_GroomInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_MarriageApplication_CivilRegOfficerId",
                table: "MarriageApplications",
                newName: "IX_MarriageApplications_CivilRegOfficerId");

            migrationBuilder.RenameIndex(
                name: "IX_MarriageApplication_BrideInfoId",
                table: "MarriageApplications",
                newName: "IX_MarriageApplications_BrideInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_MarriageApplication_ApplicationAddressId",
                table: "MarriageApplications",
                newName: "IX_MarriageApplications_ApplicationAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_InformantTypeLookupId",
                table: "Events",
                newName: "IX_Events_InformantTypeLookupId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_EventOwenerId",
                table: "Events",
                newName: "IX_Events_EventOwenerId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_EventAddressId",
                table: "Events",
                newName: "IX_Events_EventAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_CivilRegOfficerId",
                table: "Events",
                newName: "IX_Events_CivilRegOfficerId");

            migrationBuilder.RenameIndex(
                name: "IX_DivorceEvent_EventId",
                table: "DivorceEvents",
                newName: "IX_DivorceEvents_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_DivorceEvent_DivorcedWifeId",
                table: "DivorceEvents",
                newName: "IX_DivorceEvents_DivorcedWifeId");

            migrationBuilder.RenameIndex(
                name: "IX_DivorceEvent_CourtCaseId",
                table: "DivorceEvents",
                newName: "IX_DivorceEvents_CourtCaseId");

            migrationBuilder.RenameIndex(
                name: "IX_DeathNotification_DeathEventId",
                table: "DeathNotifications",
                newName: "IX_DeathNotifications_DeathEventId");

            migrationBuilder.RenameIndex(
                name: "IX_DeathNotification_CauseOfDeathInfoTypeId",
                table: "DeathNotifications",
                newName: "IX_DeathNotifications_CauseOfDeathInfoTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_DeathEvent_FacilityTypeId",
                table: "DeathEvents",
                newName: "IX_DeathEvents_FacilityTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_DeathEvent_FacilityId",
                table: "DeathEvents",
                newName: "IX_DeathEvents_FacilityId");

            migrationBuilder.RenameIndex(
                name: "IX_DeathEvent_EventId",
                table: "DeathEvents",
                newName: "IX_DeathEvents_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_CourtCase_CourtId",
                table: "CourtCases",
                newName: "IX_CourtCases_CourtId");

            migrationBuilder.RenameIndex(
                name: "IX_Court_AddressId",
                table: "Courts",
                newName: "IX_Courts_AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Certificate_EventId",
                table: "Certificates",
                newName: "IX_Certificates_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_BirthNotification_SkilledProfId",
                table: "BirthNotifications",
                newName: "IX_BirthNotifications_SkilledProfId");

            migrationBuilder.RenameIndex(
                name: "IX_BirthNotification_DeliveryTypeId",
                table: "BirthNotifications",
                newName: "IX_BirthNotifications_DeliveryTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_BirthNotification_BirthEventId",
                table: "BirthNotifications",
                newName: "IX_BirthNotifications_BirthEventId");

            migrationBuilder.RenameIndex(
                name: "IX_BirthEvent_TypeOfBirthId",
                table: "birthEvents",
                newName: "IX_birthEvents_TypeOfBirthId");

            migrationBuilder.RenameIndex(
                name: "IX_BirthEvent_MotherId",
                table: "birthEvents",
                newName: "IX_birthEvents_MotherId");

            migrationBuilder.RenameIndex(
                name: "IX_BirthEvent_FatherId",
                table: "birthEvents",
                newName: "IX_birthEvents_FatherId");

            migrationBuilder.RenameIndex(
                name: "IX_BirthEvent_FacilityTypeId",
                table: "birthEvents",
                newName: "IX_birthEvents_FacilityTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_BirthEvent_FacilityId",
                table: "birthEvents",
                newName: "IX_birthEvents_FacilityId");

            migrationBuilder.RenameIndex(
                name: "IX_BirthEvent_EventId",
                table: "birthEvents",
                newName: "IX_birthEvents_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_BirthEvent_BirthPlaceId",
                table: "birthEvents",
                newName: "IX_birthEvents_BirthPlaceId");

            migrationBuilder.RenameIndex(
                name: "IX_AdoptionEvent_EventId",
                table: "AdoptionEvents",
                newName: "IX_AdoptionEvents_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_AdoptionEvent_CourtCaseId",
                table: "AdoptionEvents",
                newName: "IX_AdoptionEvents_CourtCaseId");

            migrationBuilder.RenameIndex(
                name: "IX_AdoptionEvent_BeforeAdoptionAddressId",
                table: "AdoptionEvents",
                newName: "IX_AdoptionEvents_BeforeAdoptionAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_AdoptionEvent_AdoptiveMotherId",
                table: "AdoptionEvents",
                newName: "IX_AdoptionEvents_AdoptiveMotherId");

            migrationBuilder.RenameIndex(
                name: "IX_AdoptionEvent_AdoptiveFatherId",
                table: "AdoptionEvents",
                newName: "IX_AdoptionEvents_AdoptiveFatherId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Witnesses",
                table: "Witnesses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupportingDocuments",
                table: "SupportingDocuments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Registrars",
                table: "Registrars",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentRequests",
                table: "PaymentRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentExamptionRequests",
                table: "PaymentExamptionRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentExamptions",
                table: "PaymentExamptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarriageEvents",
                table: "MarriageEvents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarriageApplications",
                table: "MarriageApplications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DivorceEvents",
                table: "DivorceEvents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeathNotifications",
                table: "DeathNotifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeathEvents",
                table: "DeathEvents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourtCases",
                table: "CourtCases",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Courts",
                table: "Courts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Certificates",
                table: "Certificates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BirthNotifications",
                table: "BirthNotifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_birthEvents",
                table: "birthEvents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdoptionEvents",
                table: "AdoptionEvents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvents_Addresses_BeforeAdoptionAddressId",
                table: "AdoptionEvents",
                column: "BeforeAdoptionAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvents_CourtCases_CourtCaseId",
                table: "AdoptionEvents",
                column: "CourtCaseId",
                principalTable: "CourtCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvents_Events_EventId",
                table: "AdoptionEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvents_PersonalInfos_AdoptiveFatherId",
                table: "AdoptionEvents",
                column: "AdoptiveFatherId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvents_PersonalInfos_AdoptiveMotherId",
                table: "AdoptionEvents",
                column: "AdoptiveMotherId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_birthEvents_Addresses_BirthPlaceId",
                table: "birthEvents",
                column: "BirthPlaceId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_birthEvents_Events_EventId",
                table: "birthEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_birthEvents_Lookups_FacilityId",
                table: "birthEvents",
                column: "FacilityId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_birthEvents_Lookups_FacilityTypeId",
                table: "birthEvents",
                column: "FacilityTypeId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_birthEvents_Lookups_TypeOfBirthId",
                table: "birthEvents",
                column: "TypeOfBirthId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_birthEvents_PersonalInfos_FatherId",
                table: "birthEvents",
                column: "FatherId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_birthEvents_PersonalInfos_MotherId",
                table: "birthEvents",
                column: "MotherId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BirthNotifications_birthEvents_BirthEventId",
                table: "BirthNotifications",
                column: "BirthEventId",
                principalTable: "birthEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BirthNotifications_Lookups_DeliveryTypeId",
                table: "BirthNotifications",
                column: "DeliveryTypeId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BirthNotifications_Lookups_SkilledProfId",
                table: "BirthNotifications",
                column: "SkilledProfId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Events_EventId",
                table: "Certificates",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtCases_Courts_CourtId",
                table: "CourtCases",
                column: "CourtId",
                principalTable: "Courts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courts_Addresses_AddressId",
                table: "Courts",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeathEvents_Events_EventId",
                table: "DeathEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeathEvents_Lookups_FacilityId",
                table: "DeathEvents",
                column: "FacilityId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeathEvents_Lookups_FacilityTypeId",
                table: "DeathEvents",
                column: "FacilityTypeId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeathNotifications_DeathEvents_DeathEventId",
                table: "DeathNotifications",
                column: "DeathEventId",
                principalTable: "DeathEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeathNotifications_Lookups_CauseOfDeathInfoTypeId",
                table: "DeathNotifications",
                column: "CauseOfDeathInfoTypeId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DivorceEvents_CourtCases_CourtCaseId",
                table: "DivorceEvents",
                column: "CourtCaseId",
                principalTable: "CourtCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DivorceEvents_Events_EventId",
                table: "DivorceEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DivorceEvents_PersonalInfos_DivorcedWifeId",
                table: "DivorceEvents",
                column: "DivorcedWifeId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Addresses_EventAddressId",
                table: "Events",
                column: "EventAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Lookups_InformantTypeLookupId",
                table: "Events",
                column: "InformantTypeLookupId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_PersonalInfos_CivilRegOfficerId",
                table: "Events",
                column: "CivilRegOfficerId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_PersonalInfos_EventOwenerId",
                table: "Events",
                column: "EventOwenerId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarriageApplications_Addresses_ApplicationAddressId",
                table: "MarriageApplications",
                column: "ApplicationAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarriageApplications_PersonalInfos_BrideInfoId",
                table: "MarriageApplications",
                column: "BrideInfoId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarriageApplications_PersonalInfos_CivilRegOfficerId",
                table: "MarriageApplications",
                column: "CivilRegOfficerId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarriageApplications_PersonalInfos_GroomInfoId",
                table: "MarriageApplications",
                column: "GroomInfoId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarriageEvents_Events_EventId",
                table: "MarriageEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarriageEvents_Lookups_MarriageTypeId",
                table: "MarriageEvents",
                column: "MarriageTypeId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarriageEvents_MarriageApplications_ApplicationId",
                table: "MarriageEvents",
                column: "ApplicationId",
                principalTable: "MarriageApplications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarriageEvents_PersonalInfos_BrideInfoId",
                table: "MarriageEvents",
                column: "BrideInfoId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentExamptions_Events_EventId",
                table: "PaymentExamptions",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentExamptions_PaymentExamptionRequests_ExamptionRequestId",
                table: "PaymentExamptions",
                column: "ExamptionRequestId",
                principalTable: "PaymentExamptionRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRequests_Events_EventId",
                table: "PaymentRequests",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRequests_PaymentRates_PaymentRateId",
                table: "PaymentRequests",
                column: "PaymentRateId",
                principalTable: "PaymentRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Lookups_PaymentWayLookupId",
                table: "Payments",
                column: "PaymentWayLookupId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentRequests_PaymentRequestId",
                table: "Payments",
                column: "PaymentRequestId",
                principalTable: "PaymentRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrars_Events_EventId",
                table: "Registrars",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrars_Lookups_RelationshipId",
                table: "Registrars",
                column: "RelationshipId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrars_PersonalInfos_RegistrarInfoId",
                table: "Registrars",
                column: "RegistrarInfoId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportingDocuments_Events_EventId",
                table: "SupportingDocuments",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Witnesses_MarriageEvents_MarriageEventId",
                table: "Witnesses",
                column: "MarriageEventId",
                principalTable: "MarriageEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Witnesses_PersonalInfos_WitnessPersonalInfoId",
                table: "Witnesses",
                column: "WitnessPersonalInfoId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvents_Addresses_BeforeAdoptionAddressId",
                table: "AdoptionEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvents_CourtCases_CourtCaseId",
                table: "AdoptionEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvents_Events_EventId",
                table: "AdoptionEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvents_PersonalInfos_AdoptiveFatherId",
                table: "AdoptionEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionEvents_PersonalInfos_AdoptiveMotherId",
                table: "AdoptionEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_birthEvents_Addresses_BirthPlaceId",
                table: "birthEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_birthEvents_Events_EventId",
                table: "birthEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_birthEvents_Lookups_FacilityId",
                table: "birthEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_birthEvents_Lookups_FacilityTypeId",
                table: "birthEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_birthEvents_Lookups_TypeOfBirthId",
                table: "birthEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_birthEvents_PersonalInfos_FatherId",
                table: "birthEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_birthEvents_PersonalInfos_MotherId",
                table: "birthEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_BirthNotifications_birthEvents_BirthEventId",
                table: "BirthNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_BirthNotifications_Lookups_DeliveryTypeId",
                table: "BirthNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_BirthNotifications_Lookups_SkilledProfId",
                table: "BirthNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Events_EventId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_CourtCases_Courts_CourtId",
                table: "CourtCases");

            migrationBuilder.DropForeignKey(
                name: "FK_Courts_Addresses_AddressId",
                table: "Courts");

            migrationBuilder.DropForeignKey(
                name: "FK_DeathEvents_Events_EventId",
                table: "DeathEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_DeathEvents_Lookups_FacilityId",
                table: "DeathEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_DeathEvents_Lookups_FacilityTypeId",
                table: "DeathEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_DeathNotifications_DeathEvents_DeathEventId",
                table: "DeathNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_DeathNotifications_Lookups_CauseOfDeathInfoTypeId",
                table: "DeathNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_DivorceEvents_CourtCases_CourtCaseId",
                table: "DivorceEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_DivorceEvents_Events_EventId",
                table: "DivorceEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_DivorceEvents_PersonalInfos_DivorcedWifeId",
                table: "DivorceEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Addresses_EventAddressId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Lookups_InformantTypeLookupId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_PersonalInfos_CivilRegOfficerId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_PersonalInfos_EventOwenerId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_MarriageApplications_Addresses_ApplicationAddressId",
                table: "MarriageApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_MarriageApplications_PersonalInfos_BrideInfoId",
                table: "MarriageApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_MarriageApplications_PersonalInfos_CivilRegOfficerId",
                table: "MarriageApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_MarriageApplications_PersonalInfos_GroomInfoId",
                table: "MarriageApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_MarriageEvents_Events_EventId",
                table: "MarriageEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_MarriageEvents_Lookups_MarriageTypeId",
                table: "MarriageEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_MarriageEvents_MarriageApplications_ApplicationId",
                table: "MarriageEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_MarriageEvents_PersonalInfos_BrideInfoId",
                table: "MarriageEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentExamptions_Events_EventId",
                table: "PaymentExamptions");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentExamptions_PaymentExamptionRequests_ExamptionRequestId",
                table: "PaymentExamptions");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentRequests_Events_EventId",
                table: "PaymentRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentRequests_PaymentRates_PaymentRateId",
                table: "PaymentRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Lookups_PaymentWayLookupId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentRequests_PaymentRequestId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrars_Events_EventId",
                table: "Registrars");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrars_Lookups_RelationshipId",
                table: "Registrars");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrars_PersonalInfos_RegistrarInfoId",
                table: "Registrars");

            migrationBuilder.DropForeignKey(
                name: "FK_SupportingDocuments_Events_EventId",
                table: "SupportingDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_Witnesses_MarriageEvents_MarriageEventId",
                table: "Witnesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Witnesses_PersonalInfos_WitnessPersonalInfoId",
                table: "Witnesses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Witnesses",
                table: "Witnesses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SupportingDocuments",
                table: "SupportingDocuments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Registrars",
                table: "Registrars");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentRequests",
                table: "PaymentRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentExamptions",
                table: "PaymentExamptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentExamptionRequests",
                table: "PaymentExamptionRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MarriageEvents",
                table: "MarriageEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MarriageApplications",
                table: "MarriageApplications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DivorceEvents",
                table: "DivorceEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeathNotifications",
                table: "DeathNotifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeathEvents",
                table: "DeathEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Courts",
                table: "Courts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourtCases",
                table: "CourtCases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Certificates",
                table: "Certificates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BirthNotifications",
                table: "BirthNotifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_birthEvents",
                table: "birthEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdoptionEvents",
                table: "AdoptionEvents");

            migrationBuilder.RenameTable(
                name: "Witnesses",
                newName: "Witness");

            migrationBuilder.RenameTable(
                name: "SupportingDocuments",
                newName: "SupportingDocument");

            migrationBuilder.RenameTable(
                name: "Registrars",
                newName: "Registrar");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payment");

            migrationBuilder.RenameTable(
                name: "PaymentRequests",
                newName: "PaymentRequest");

            migrationBuilder.RenameTable(
                name: "PaymentExamptions",
                newName: "PaymentExamption");

            migrationBuilder.RenameTable(
                name: "PaymentExamptionRequests",
                newName: "PaymentExamptionRequest");

            migrationBuilder.RenameTable(
                name: "MarriageEvents",
                newName: "MarriageEvent");

            migrationBuilder.RenameTable(
                name: "MarriageApplications",
                newName: "MarriageApplication");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "Event");

            migrationBuilder.RenameTable(
                name: "DivorceEvents",
                newName: "DivorceEvent");

            migrationBuilder.RenameTable(
                name: "DeathNotifications",
                newName: "DeathNotification");

            migrationBuilder.RenameTable(
                name: "DeathEvents",
                newName: "DeathEvent");

            migrationBuilder.RenameTable(
                name: "Courts",
                newName: "Court");

            migrationBuilder.RenameTable(
                name: "CourtCases",
                newName: "CourtCase");

            migrationBuilder.RenameTable(
                name: "Certificates",
                newName: "Certificate");

            migrationBuilder.RenameTable(
                name: "BirthNotifications",
                newName: "BirthNotification");

            migrationBuilder.RenameTable(
                name: "birthEvents",
                newName: "BirthEvent");

            migrationBuilder.RenameTable(
                name: "AdoptionEvents",
                newName: "AdoptionEvent");

            migrationBuilder.RenameIndex(
                name: "IX_Witnesses_WitnessPersonalInfoId",
                table: "Witness",
                newName: "IX_Witness_WitnessPersonalInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_Witnesses_MarriageEventId",
                table: "Witness",
                newName: "IX_Witness_MarriageEventId");

            migrationBuilder.RenameIndex(
                name: "IX_SupportingDocuments_EventId",
                table: "SupportingDocument",
                newName: "IX_SupportingDocument_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Registrars_RelationshipId",
                table: "Registrar",
                newName: "IX_Registrar_RelationshipId");

            migrationBuilder.RenameIndex(
                name: "IX_Registrars_RegistrarInfoId",
                table: "Registrar",
                newName: "IX_Registrar_RegistrarInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_Registrars_EventId",
                table: "Registrar",
                newName: "IX_Registrar_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_PaymentWayLookupId",
                table: "Payment",
                newName: "IX_Payment_PaymentWayLookupId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_PaymentRequestId",
                table: "Payment",
                newName: "IX_Payment_PaymentRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentRequests_PaymentRateId",
                table: "PaymentRequest",
                newName: "IX_PaymentRequest_PaymentRateId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentRequests_EventId",
                table: "PaymentRequest",
                newName: "IX_PaymentRequest_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentExamptions_ExamptionRequestId",
                table: "PaymentExamption",
                newName: "IX_PaymentExamption_ExamptionRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentExamptions_EventId",
                table: "PaymentExamption",
                newName: "IX_PaymentExamption_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_MarriageEvents_MarriageTypeId",
                table: "MarriageEvent",
                newName: "IX_MarriageEvent_MarriageTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_MarriageEvents_EventId",
                table: "MarriageEvent",
                newName: "IX_MarriageEvent_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_MarriageEvents_BrideInfoId",
                table: "MarriageEvent",
                newName: "IX_MarriageEvent_BrideInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_MarriageEvents_ApplicationId",
                table: "MarriageEvent",
                newName: "IX_MarriageEvent_ApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_MarriageApplications_GroomInfoId",
                table: "MarriageApplication",
                newName: "IX_MarriageApplication_GroomInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_MarriageApplications_CivilRegOfficerId",
                table: "MarriageApplication",
                newName: "IX_MarriageApplication_CivilRegOfficerId");

            migrationBuilder.RenameIndex(
                name: "IX_MarriageApplications_BrideInfoId",
                table: "MarriageApplication",
                newName: "IX_MarriageApplication_BrideInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_MarriageApplications_ApplicationAddressId",
                table: "MarriageApplication",
                newName: "IX_MarriageApplication_ApplicationAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_InformantTypeLookupId",
                table: "Event",
                newName: "IX_Event_InformantTypeLookupId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_EventOwenerId",
                table: "Event",
                newName: "IX_Event_EventOwenerId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_EventAddressId",
                table: "Event",
                newName: "IX_Event_EventAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_CivilRegOfficerId",
                table: "Event",
                newName: "IX_Event_CivilRegOfficerId");

            migrationBuilder.RenameIndex(
                name: "IX_DivorceEvents_EventId",
                table: "DivorceEvent",
                newName: "IX_DivorceEvent_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_DivorceEvents_DivorcedWifeId",
                table: "DivorceEvent",
                newName: "IX_DivorceEvent_DivorcedWifeId");

            migrationBuilder.RenameIndex(
                name: "IX_DivorceEvents_CourtCaseId",
                table: "DivorceEvent",
                newName: "IX_DivorceEvent_CourtCaseId");

            migrationBuilder.RenameIndex(
                name: "IX_DeathNotifications_DeathEventId",
                table: "DeathNotification",
                newName: "IX_DeathNotification_DeathEventId");

            migrationBuilder.RenameIndex(
                name: "IX_DeathNotifications_CauseOfDeathInfoTypeId",
                table: "DeathNotification",
                newName: "IX_DeathNotification_CauseOfDeathInfoTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_DeathEvents_FacilityTypeId",
                table: "DeathEvent",
                newName: "IX_DeathEvent_FacilityTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_DeathEvents_FacilityId",
                table: "DeathEvent",
                newName: "IX_DeathEvent_FacilityId");

            migrationBuilder.RenameIndex(
                name: "IX_DeathEvents_EventId",
                table: "DeathEvent",
                newName: "IX_DeathEvent_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Courts_AddressId",
                table: "Court",
                newName: "IX_Court_AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_CourtCases_CourtId",
                table: "CourtCase",
                newName: "IX_CourtCase_CourtId");

            migrationBuilder.RenameIndex(
                name: "IX_Certificates_EventId",
                table: "Certificate",
                newName: "IX_Certificate_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_BirthNotifications_SkilledProfId",
                table: "BirthNotification",
                newName: "IX_BirthNotification_SkilledProfId");

            migrationBuilder.RenameIndex(
                name: "IX_BirthNotifications_DeliveryTypeId",
                table: "BirthNotification",
                newName: "IX_BirthNotification_DeliveryTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_BirthNotifications_BirthEventId",
                table: "BirthNotification",
                newName: "IX_BirthNotification_BirthEventId");

            migrationBuilder.RenameIndex(
                name: "IX_birthEvents_TypeOfBirthId",
                table: "BirthEvent",
                newName: "IX_BirthEvent_TypeOfBirthId");

            migrationBuilder.RenameIndex(
                name: "IX_birthEvents_MotherId",
                table: "BirthEvent",
                newName: "IX_BirthEvent_MotherId");

            migrationBuilder.RenameIndex(
                name: "IX_birthEvents_FatherId",
                table: "BirthEvent",
                newName: "IX_BirthEvent_FatherId");

            migrationBuilder.RenameIndex(
                name: "IX_birthEvents_FacilityTypeId",
                table: "BirthEvent",
                newName: "IX_BirthEvent_FacilityTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_birthEvents_FacilityId",
                table: "BirthEvent",
                newName: "IX_BirthEvent_FacilityId");

            migrationBuilder.RenameIndex(
                name: "IX_birthEvents_EventId",
                table: "BirthEvent",
                newName: "IX_BirthEvent_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_birthEvents_BirthPlaceId",
                table: "BirthEvent",
                newName: "IX_BirthEvent_BirthPlaceId");

            migrationBuilder.RenameIndex(
                name: "IX_AdoptionEvents_EventId",
                table: "AdoptionEvent",
                newName: "IX_AdoptionEvent_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_AdoptionEvents_CourtCaseId",
                table: "AdoptionEvent",
                newName: "IX_AdoptionEvent_CourtCaseId");

            migrationBuilder.RenameIndex(
                name: "IX_AdoptionEvents_BeforeAdoptionAddressId",
                table: "AdoptionEvent",
                newName: "IX_AdoptionEvent_BeforeAdoptionAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_AdoptionEvents_AdoptiveMotherId",
                table: "AdoptionEvent",
                newName: "IX_AdoptionEvent_AdoptiveMotherId");

            migrationBuilder.RenameIndex(
                name: "IX_AdoptionEvents_AdoptiveFatherId",
                table: "AdoptionEvent",
                newName: "IX_AdoptionEvent_AdoptiveFatherId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Witness",
                table: "Witness",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupportingDocument",
                table: "SupportingDocument",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Registrar",
                table: "Registrar",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                table: "Payment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentRequest",
                table: "PaymentRequest",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentExamption",
                table: "PaymentExamption",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentExamptionRequest",
                table: "PaymentExamptionRequest",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarriageEvent",
                table: "MarriageEvent",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarriageApplication",
                table: "MarriageApplication",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Event",
                table: "Event",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DivorceEvent",
                table: "DivorceEvent",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeathNotification",
                table: "DeathNotification",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeathEvent",
                table: "DeathEvent",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Court",
                table: "Court",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourtCase",
                table: "CourtCase",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Certificate",
                table: "Certificate",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BirthNotification",
                table: "BirthNotification",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BirthEvent",
                table: "BirthEvent",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdoptionEvent",
                table: "AdoptionEvent",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvent_Addresses_BeforeAdoptionAddressId",
                table: "AdoptionEvent",
                column: "BeforeAdoptionAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvent_CourtCase_CourtCaseId",
                table: "AdoptionEvent",
                column: "CourtCaseId",
                principalTable: "CourtCase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvent_Event_EventId",
                table: "AdoptionEvent",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvent_PersonalInfos_AdoptiveFatherId",
                table: "AdoptionEvent",
                column: "AdoptiveFatherId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionEvent_PersonalInfos_AdoptiveMotherId",
                table: "AdoptionEvent",
                column: "AdoptiveMotherId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BirthEvent_Addresses_BirthPlaceId",
                table: "BirthEvent",
                column: "BirthPlaceId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BirthEvent_Event_EventId",
                table: "BirthEvent",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BirthEvent_Lookups_FacilityId",
                table: "BirthEvent",
                column: "FacilityId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BirthEvent_Lookups_FacilityTypeId",
                table: "BirthEvent",
                column: "FacilityTypeId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BirthEvent_Lookups_TypeOfBirthId",
                table: "BirthEvent",
                column: "TypeOfBirthId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BirthEvent_PersonalInfos_FatherId",
                table: "BirthEvent",
                column: "FatherId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BirthEvent_PersonalInfos_MotherId",
                table: "BirthEvent",
                column: "MotherId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BirthNotification_BirthEvent_BirthEventId",
                table: "BirthNotification",
                column: "BirthEventId",
                principalTable: "BirthEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BirthNotification_Lookups_DeliveryTypeId",
                table: "BirthNotification",
                column: "DeliveryTypeId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BirthNotification_Lookups_SkilledProfId",
                table: "BirthNotification",
                column: "SkilledProfId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificate_Event_EventId",
                table: "Certificate",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Court_Addresses_AddressId",
                table: "Court",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourtCase_Court_CourtId",
                table: "CourtCase",
                column: "CourtId",
                principalTable: "Court",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeathEvent_Event_EventId",
                table: "DeathEvent",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeathEvent_Lookups_FacilityId",
                table: "DeathEvent",
                column: "FacilityId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeathEvent_Lookups_FacilityTypeId",
                table: "DeathEvent",
                column: "FacilityTypeId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeathNotification_DeathEvent_DeathEventId",
                table: "DeathNotification",
                column: "DeathEventId",
                principalTable: "DeathEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeathNotification_Lookups_CauseOfDeathInfoTypeId",
                table: "DeathNotification",
                column: "CauseOfDeathInfoTypeId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DivorceEvent_CourtCase_CourtCaseId",
                table: "DivorceEvent",
                column: "CourtCaseId",
                principalTable: "CourtCase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DivorceEvent_Event_EventId",
                table: "DivorceEvent",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DivorceEvent_PersonalInfos_DivorcedWifeId",
                table: "DivorceEvent",
                column: "DivorcedWifeId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Addresses_EventAddressId",
                table: "Event",
                column: "EventAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Lookups_InformantTypeLookupId",
                table: "Event",
                column: "InformantTypeLookupId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_PersonalInfos_CivilRegOfficerId",
                table: "Event",
                column: "CivilRegOfficerId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_PersonalInfos_EventOwenerId",
                table: "Event",
                column: "EventOwenerId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarriageApplication_Addresses_ApplicationAddressId",
                table: "MarriageApplication",
                column: "ApplicationAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarriageApplication_PersonalInfos_BrideInfoId",
                table: "MarriageApplication",
                column: "BrideInfoId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarriageApplication_PersonalInfos_CivilRegOfficerId",
                table: "MarriageApplication",
                column: "CivilRegOfficerId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarriageApplication_PersonalInfos_GroomInfoId",
                table: "MarriageApplication",
                column: "GroomInfoId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarriageEvent_Event_EventId",
                table: "MarriageEvent",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarriageEvent_Lookups_MarriageTypeId",
                table: "MarriageEvent",
                column: "MarriageTypeId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarriageEvent_MarriageApplication_ApplicationId",
                table: "MarriageEvent",
                column: "ApplicationId",
                principalTable: "MarriageApplication",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarriageEvent_PersonalInfos_BrideInfoId",
                table: "MarriageEvent",
                column: "BrideInfoId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Lookups_PaymentWayLookupId",
                table: "Payment",
                column: "PaymentWayLookupId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_PaymentRequest_PaymentRequestId",
                table: "Payment",
                column: "PaymentRequestId",
                principalTable: "PaymentRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentExamption_Event_EventId",
                table: "PaymentExamption",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentExamption_PaymentExamptionRequest_ExamptionRequestId",
                table: "PaymentExamption",
                column: "ExamptionRequestId",
                principalTable: "PaymentExamptionRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRequest_Event_EventId",
                table: "PaymentRequest",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRequest_PaymentRates_PaymentRateId",
                table: "PaymentRequest",
                column: "PaymentRateId",
                principalTable: "PaymentRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrar_Event_EventId",
                table: "Registrar",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrar_Lookups_RelationshipId",
                table: "Registrar",
                column: "RelationshipId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrar_PersonalInfos_RegistrarInfoId",
                table: "Registrar",
                column: "RegistrarInfoId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportingDocument_Event_EventId",
                table: "SupportingDocument",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Witness_MarriageEvent_MarriageEventId",
                table: "Witness",
                column: "MarriageEventId",
                principalTable: "MarriageEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Witness_PersonalInfos_WitnessPersonalInfoId",
                table: "Witness",
                column: "WitnessPersonalInfoId",
                principalTable: "PersonalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
