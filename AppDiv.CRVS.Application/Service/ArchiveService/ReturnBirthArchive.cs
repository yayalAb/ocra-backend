using AppDiv.CRVS.Application.Contracts.DTOs.Archive;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive.BirthArchive;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Services;
using AppDiv.CRVS.Application.Interfaces.Archive;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AutoMapper.QueryableExtensions;
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public class ReturnBirthArchive : IReturnBirthArchive
    {
        private readonly IDateAndAddressService _dateAndAddressService;
        private readonly ILookupFromId _lookupService;
        private readonly IPersonalInfoRepository _person;
        private readonly ISupportingDocumentRepository _supportingDocument;
        private readonly IReportRepostory _reportRepostory;
        public ReturnBirthArchive(IDateAndAddressService DateAndAddressService,
                                ILookupFromId lookupService,
                                IPersonalInfoRepository person,
                                ISupportingDocumentRepository supportingDocument,
                                IReportRepostory reportRepostory)
        {
            _dateAndAddressService = DateAndAddressService;
            _lookupService = lookupService;
            _supportingDocument = supportingDocument;
            _person = person;
            _reportRepostory=reportRepostory;
            // _convertor = new CustomDateConverter();
        }

        private BirthInfo GetEventInfo(Event? birth)
        {
            BirthInfo birthInfo = CustomMapper.Mapper.Map<BirthInfo>(ReturnPerson.GetEventInfo(birth, _dateAndAddressService, _reportRepostory));
            var typeOfBirth = _lookupService.GetLookup(birth?.BirthEvent?.TypeOfBirthLookupId);
            birthInfo.TypeOfBirthOr = birth?.BirthEvent?.TypeOfBirthLookup?.Value?.Value<string>("or") ?? typeOfBirth?.Value?.Value<string>("or");
            birthInfo.TypeOfBirthAm = birth?.BirthEvent?.TypeOfBirthLookup?.Value?.Value<string>("am") ?? typeOfBirth?.Value?.Value<string>("am");
            var birthPlace = _lookupService.GetLookup(birth?.BirthEvent?.BirthPlaceId);
            birthInfo.BirthPlaceOr = birth?.BirthEvent?.BirthPlace?.Value?.Value<string>("or") ?? birthPlace?.Value?.Value<string>("or");
            birthInfo.BirthPlaceAm = birth?.BirthEvent?.BirthPlace?.Value?.Value<string>("am") ?? birthPlace?.Value?.Value<string>("am");
            return birthInfo;
        }
        private BirthNotificationArchive GetNotification(BirthNotification? notification)
        {
            var deliveryType = _lookupService.GetLookup(notification?.DeliveryTypeLookupId);
            var skilledProfessional = _lookupService.GetLookup(notification?.SkilledProfLookupId);
            return new BirthNotificationArchive
            {
                WeightAtBirth = notification?.WeightAtBirth,
                DeliveryTypeOr = notification?.DeliveryTypeLookup?.Value?.Value<string>("or") ?? deliveryType?.Value?.Value<string>("or"),
                DeliveryTypeAm = notification?.DeliveryTypeLookup?.Value?.Value<string>("am") ?? deliveryType?.Value?.Value<string>("am"),
                SkilledProfessionalOr = notification?.SkilledProfLookup?.Value?.Value<string>("or") ?? skilledProfessional?.Value?.Value<string>("or"),
                SkilledProfessionalAm = notification?.SkilledProfLookup?.Value?.Value<string>("am") ?? skilledProfessional?.Value?.Value<string>("am"),
                NotificationSerialNumber = notification?.NotficationSerialNumber,
            };
        }


        private RegistrarArchive? GetRegistrar(Registrar? reg, bool isCorrection = false)
        {
            if (reg is null) return new RegistrarArchive();
            RegistrarArchive regInfo = CustomMapper.Mapper.Map<RegistrarArchive>(ReturnPerson.GetPerson(reg?.RegistrarInfo, _dateAndAddressService, _lookupService, _reportRepostory, isCorrection));
            var relationship = _lookupService.GetLookup(reg?.RelationshipLookupId);
            regInfo.RelationShipOr = reg?.RelationshipLookup?.Value?.Value<string>("or") ?? relationship?.Value?.Value<string>("or");
            regInfo.RelationShipAm = reg?.RelationshipLookup?.Value?.Value<string>("am") ?? relationship?.Value?.Value<string>("am");
            return regInfo;           
        }

        public BirthArchiveDTO GetBirthArchive(Event? birth, string? BirthCertNo,bool IsCorectionRequest=false)
        {
            var convertor = new CustomDateConverter();
            var birthInfo = new BirthArchiveDTO()
            {
                Child = CustomMapper.Mapper.Map<Child>
                                            (ReturnPerson.GetPerson(birth?.EventOwener, _dateAndAddressService, _lookupService,_reportRepostory)),
                Mother = ReturnPerson.GetPerson(birth?.BirthEvent?.Mother, _dateAndAddressService, _lookupService,_reportRepostory),
                Father = ReturnPerson.GetPerson(birth?.BirthEvent?.Father, _dateAndAddressService, _lookupService,_reportRepostory),
                EventInfo = GetEventInfo(birth),
                Notification = GetNotification(birth?.BirthEvent?.BirthNotification!),
                Registrar = GetRegistrar(birth?.EventRegistrar),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(birth?.CivilRegOfficer, _dateAndAddressService, _lookupService,_reportRepostory)),
                EventSupportingDocuments = _supportingDocument.GetAll().Where(s => s.EventId == birth!.Id)
                                                    .ProjectTo<SupportingDocumentDTO>(CustomMapper.Mapper.ConfigurationProvider).ToList(),


            };
            birthInfo.PaymentExamptionSupportingDocuments = birth?.PaymentExamption?.Id == null ? null
                : _supportingDocument.GetAll().Where(s => s.PaymentExamptionId == birth.PaymentExamption.Id)
                                        .ProjectTo<SupportingDocumentDTO>(CustomMapper.Mapper.ConfigurationProvider).ToList();
            return birthInfo;

        }

        public BirthArchiveDTO GetBirthPreviewArchive(BirthEvent? birth, string? BirthCertNo,bool IsCorectionRequest=true)
        {
            birth!.Event.BirthEvent = birth;
            if (birth?.Event.CivilRegOfficer == null && birth?.Event.CivilRegOfficerId != null)
            {
                birth.Event.CivilRegOfficer = _person.GetAll().Where(p => p.Id == birth!.Event!.CivilRegOfficerId).FirstOrDefault()!;
            }
            return new BirthArchiveDTO()
            {
                Child = CustomMapper.Mapper.Map<Child>
                                            (ReturnPerson.GetPerson(birth?.Event?.EventOwener, _dateAndAddressService, _lookupService,_reportRepostory,IsCorectionRequest)),
                Mother = ReturnPerson.GetPerson(birth?.Mother, _dateAndAddressService, _lookupService,_reportRepostory,IsCorectionRequest),
                Father = ReturnPerson.GetPerson(birth?.Father, _dateAndAddressService, _lookupService,_reportRepostory,IsCorectionRequest),
                EventInfo = GetEventInfo(birth?.Event),
                Notification = GetNotification(birth?.BirthNotification),
                Registrar = GetRegistrar(birth?.Event?.EventRegistrar, IsCorectionRequest),
                CivilRegistrarOfficer = CustomMapper.Mapper.Map<Officer>
                                        (ReturnPerson.GetPerson(birth?.Event?.CivilRegOfficer, _dateAndAddressService, _lookupService,_reportRepostory)),
                EventSupportingDocuments = CustomMapper.Mapper.Map<IList<SupportingDocumentDTO>>(birth?.Event?.EventSupportingDocuments),
                PaymentExamptionSupportingDocuments = CustomMapper.Mapper.Map<IList<SupportingDocumentDTO>>(birth?.Event?.PaymentExamption?.SupportingDocuments),
            };

        }
    }
} 