
using System.Linq.Expressions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Enums;
using AppDiv.CRVS.Domain.Models;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Service
{
    public class EventDocumentService : IEventDocumentService
    {


        private readonly IFileService _fileService;
        private readonly ISupportingDocumentRepository _supportingDocumentRepository;
        private readonly ILookupRepository _lookupRepository;
        private readonly Guid? _webCamTypeLookupId;
        private readonly Guid? _fingerprintTypeLookupId;
        private readonly Guid? _signatureTypeLookupId;
        private readonly List<Lookup> _userDocTypeLookupId = new();



        public EventDocumentService(IFileService fileService, ISupportingDocumentRepository supportingDocumentRepository, ILookupRepository lookupRepository)
        {
            _fileService = fileService;
            _supportingDocumentRepository = supportingDocumentRepository;
            _lookupRepository = lookupRepository;
            _webCamTypeLookupId = _lookupRepository.GetAll()
                    .Where(l => l.ValueStr.ToLower()
                    .Contains("webcam"))
                    .Select(l => l.Id)
                    .FirstOrDefault();
            _fingerprintTypeLookupId = _lookupRepository.GetAll()
                    .Where(l => l.ValueStr.ToLower()
                    .Contains("fingerprint"))
                    .Select(l => l.Id)
                    .FirstOrDefault();
            _signatureTypeLookupId = _lookupRepository.GetAll()
                        .Where(l => l.ValueStr.ToLower()
                        .Contains("signature"))
                        .Select(l => l.Id)
                        .FirstOrDefault();
            _userDocTypeLookupId = _lookupRepository.GetAll()
                        .Where(l => l.ValueStr.ToLower()
                        .Contains("passport") || l.ValueStr.ToLower()
                        .Contains("nationalid") || l.ValueStr.ToLower()
                        .Contains("id"))
                        .ToList();
        }
        public bool saveSupportingDocuments(ICollection<SupportingDocument> eventDocs, ICollection<SupportingDocument>? examptionDocs, string eventType)
        {

            var supportingDocFolder = Path.Combine("Resources", "SupportingDocuments", eventType);
            var examptiondocFolder = Path.Combine("Resources", "ExamptionDocuments", eventType);
            var fullPathSupporting = Path.Combine(Directory.GetCurrentDirectory(), supportingDocFolder);
            var fullPathExamption = Path.Combine(Directory.GetCurrentDirectory(), examptiondocFolder);


            eventDocs?.ToList().ForEach(doc =>
            {
                _fileService.UploadBase64FileAsync(doc.base64String, doc.Id.ToString(), fullPathSupporting, FileMode.Create);
            });
            examptionDocs?.ToList().ForEach(doc =>
            {
                _fileService.UploadBase64FileAsync(doc.base64String, doc.Id.ToString(), fullPathExamption, FileMode.Create);
            });
            return true;
        }
        public bool MoveSupportingDocuments(ICollection<SupportingDocument> eventDocs, ICollection<SupportingDocument>? examptionDocs, string eventType)
        {

            var sourceFolderSupp = Path.Combine("Resources", "CorrectionRequestSupportingDocuments", eventType);
            var sourceFolederExa = Path.Combine("Resources", "CorrectionRequestExamptionDocuments", eventType);
            var supportingDocFolder = Path.Combine("Resources", "SupportingDocuments", eventType);
            var examptionDocFolder = Path.Combine("Resources", "ExamptionDocuments", eventType);
            if (!Directory.Exists(supportingDocFolder))
            {
                // If folder does not exist, create it
                Directory.CreateDirectory(supportingDocFolder);
            }
            if (!Directory.Exists(examptionDocFolder))
            {
                // If folder does not exist, create it
                Directory.CreateDirectory(examptionDocFolder);
            }
            var fullPathSupporting = Path.Combine(Directory.GetCurrentDirectory(), supportingDocFolder);
            var fullPathExamption = Path.Combine(Directory.GetCurrentDirectory(), examptionDocFolder);
            // string[] filesToMove1 = Directory.GetFiles(sourceFolderSupp);
            // string[] filesToMove2 = Directory.GetFiles(sourceFolederExa);

            eventDocs?.ToList().ForEach(doc =>
            {
                var file = Directory.GetFiles(sourceFolderSupp, doc.Id + "*").FirstOrDefault();
                if (file != null)
                {
                    var destFile = Path.Combine(fullPathSupporting, Path.GetFileName(file));
                    File.Move(file, destFile);
                }
                // _fileService.UploadBase64FileAsync(doc.base64String, doc.Id.ToString(), fullPathSupporting, FileMode.Create);
            });
            examptionDocs?.ToList().ForEach(doc =>
            {
                var file = Directory.GetFiles(sourceFolederExa, doc.Id + "*").FirstOrDefault();
                if (file != null)
                {
                    var destFile = Path.Combine(fullPathExamption, Path.GetFileName(file));
                    File.Move(file, destFile);
                }
                // _fileService.UploadBase64FileAsync(doc.base64String, doc.Id.ToString(), fullPathExamption, FileMode.Create);
            });
            return true;
        }

        public bool SaveCorrectionRequestSupportingDocuments(ICollection<AddSupportingDocumentRequest> eventDocs, ICollection<AddSupportingDocumentRequest>? examptionDocs, string eventType)
        {

            var supportingDocFolder = Path.Combine("Resources", "CorrectionRequestSupportingDocuments", eventType);
            var examptiondocFolder = Path.Combine("Resources", "CorrectionRequestExamptionDocuments", eventType);
            var fullPathSupporting = Path.Combine(Directory.GetCurrentDirectory(), supportingDocFolder);
            var fullPathExamption = Path.Combine(Directory.GetCurrentDirectory(), examptiondocFolder);

            if (eventDocs != null)
            {
                eventDocs?.ToList().ForEach(doc =>
            {
                _fileService.UploadBase64FileAsync(doc.base64String, doc.Id.ToString(), fullPathSupporting, FileMode.Create);
            });
            }
            if (examptionDocs != null)
            {

                examptionDocs?.ToList().ForEach(doc =>
            {
                _fileService.UploadBase64FileAsync(doc.base64String, doc.Id.ToString(), fullPathExamption, FileMode.Create);
            });
            }
            return true;
        }

        public async Task<(Dictionary<string, string> userPhotos, Dictionary<string, List<BiometricImagesAtt>?> fingerPrint)> SaveSupportingDocumentsAsync(Event savedEvent, List<AddSupportingDocumentRequest>? eventSupportingDocs, List<AddSupportingDocumentRequest>? exapmtionSupportingDocs, Guid? paymentExapmtionId, CancellationToken cancellationToken)
        {
            var personIds = new PersonIdObj
            {
                WifeId = savedEvent.EventType.ToLower() == "marriage"
                                ? savedEvent.MarriageEvent?.BrideInfoId
                                : savedEvent.EventType.ToLower() == "divorce"
                                ? savedEvent.DivorceEvent?.DivorcedWifeId
                                : null,
                HusbandId = savedEvent.EventType.ToLower() == "marraige" || savedEvent.EventType.ToLower() == "divorce"
                                ? savedEvent.EventOwenerId
                                : null,
                WitnessIds = savedEvent.MarriageEvent?.Witnesses.Select(w => w.WitnessPersonalInfo.Id).ToList(),
                FatherId = savedEvent.EventType.ToLower() == "birth"
                                ? savedEvent.BirthEvent?.FatherId
                                : savedEvent.EventType.ToLower() == "adoption"
                                ? savedEvent.AdoptionEvent?.AdoptiveFatherId
                                : null,
                MotherId = savedEvent.EventType.ToLower() == "birth"
                                ? savedEvent.BirthEvent?.MotherId
                                : savedEvent.EventType.ToLower() == "adoption"
                                ? savedEvent.AdoptionEvent?.AdoptiveMotherId
                                : null,
                ChildId = savedEvent.EventType.ToLower() == "birth" || savedEvent.EventType.ToLower() == "adoption"
                                ? savedEvent.EventOwenerId
                                : null,
                DeceasedId = savedEvent.EventType.ToLower() == "death"
                                ? savedEvent.EventOwenerId
                                : null,
                RegistrarId = savedEvent.EventRegisteredAddressId

            };
            var docs = await createSupportingDocumentsAsync(eventSupportingDocs, exapmtionSupportingDocs, savedEvent.Id, paymentExapmtionId, cancellationToken);
            await _supportingDocumentRepository.SaveChangesAsync(cancellationToken);
            var separatedDocs = extractSupportingDocs(personIds, docs.supportingDocs);
            savePhotos(separatedDocs.UserPhoto);
            savePhotos(separatedDocs.Signatures, "Signatures");
            saveSupportingDocuments((ICollection<SupportingDocument>)separatedDocs.OtherDocs, (ICollection<SupportingDocument>)docs.examptionDocs, savedEvent.EventType);
            saveFingerPrints(separatedDocs.FingerPrints);
            return (userPhotos: separatedDocs.UserPhoto, fingerPrint: separatedDocs.FingerPrints);
        }

        public async Task<(IEnumerable<SupportingDocument> supportingDocs, IEnumerable<SupportingDocument> examptionDocs)> createSupportingDocumentsAsync(IEnumerable<AddSupportingDocumentRequest>? supportingDocs, IEnumerable<AddSupportingDocumentRequest>? examptionDocs, Guid EventId, Guid? examptionId, CancellationToken cancellationToken)
        {
            IEnumerable<SupportingDocument> mappedDocs = new List<SupportingDocument>();
            IEnumerable<SupportingDocument> mappedExamptionDocs = new List<SupportingDocument>();
            if (supportingDocs != null)
            {
                mappedDocs = CustomMapper.Mapper.Map<IEnumerable<SupportingDocument>>(supportingDocs);
                mappedDocs.ToList().ForEach(doc =>
                {
                    doc.EventId = EventId;
                });
                await _supportingDocumentRepository.InsertAsync(mappedDocs, cancellationToken);
            }

            if (examptionDocs != null)
            {
                mappedExamptionDocs = CustomMapper.Mapper.Map<IEnumerable<SupportingDocument>>(examptionDocs);
                mappedExamptionDocs.ToList().ForEach(doc =>
                {
                    doc.PaymentExamptionId = examptionId;
                });
                await _supportingDocumentRepository.InsertAsync(mappedExamptionDocs, cancellationToken);
            }
            await _supportingDocumentRepository.SaveChangesAsync(cancellationToken);
            return (supportingDocs: mappedDocs, examptionDocs: mappedExamptionDocs);
        }

        public void savePhotos(Dictionary<string, string> personPhotos, string folderName = "PersonPhotos")
        {
            var folder = Path.Combine("Resources", folderName);

            personPhotos.ToList().ForEach(async p =>
           {
               var fullPath = Path.Combine(Directory.GetCurrentDirectory(), folder);
               await _fileService.UploadBase64FileAsync(p.Value, p.Key, fullPath, FileMode.Create);
           });

        }
        public void saveFingerPrints(Dictionary<string, List<BiometricImagesAtt>?> fingerprints)
        {
            if (fingerprints.Values != null)
            {
                var mainFolder = Path.Combine("Resources", "fingerprints");
                fingerprints.ToList().ForEach(fingerPrintData =>
                {
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), mainFolder, fingerPrintData.Key);//fingerPrintData.key is personId
                    if (fingerPrintData.Value != null)
                    {
                        fingerPrintData.Value.ForEach(async finger =>
                        {
                            await _fileService.UploadBase64FileAsync(finger.base64Image, finger.position.ToString(), fullPath, FileMode.Create);
                        });
                    }

                });
            }
        }
        public void MovePhotos(Dictionary<string, string> personPhotos, string eventType)
        {
            var folder = Path.Combine("Resources", "PersonPhotos");
            if (!Directory.Exists(folder))
            {
                // If folder does not exist, create it
                Directory.CreateDirectory(folder);
            }
            var sourceFolder = Path.Combine("Resources", "CorrectionRequestSupportingDocuments", eventType);
            string[] filesToMove = Directory.GetFiles(sourceFolder);
            personPhotos.ToList().ForEach(p =>
            {
                var file = Directory.GetFiles(sourceFolder, p.Value + "*").FirstOrDefault();
                // var sourceFullPath = Path.Combine(Directory.GetCurrentDirectory(), sourceFolder, p.Value);
                if (file != null)
                {
                    var destFullPath = Path.Combine(Directory.GetCurrentDirectory(), folder, p.Key + Path.GetExtension(file));
                    File.Move(file, destFullPath);
                }
                // _fileService.UploadBase64FileAsync(p.Value, p.Key, fullPath, FileMode.Create);
            });

        }
        public ExtractedDocDTO extractSupportingDocs(PersonIdObj idObj, IEnumerable<SupportingDocument> supportingDocs)
        {
            Dictionary<string, string> userPhotos = new();
            Dictionary<string, string> signatures = new();
            Dictionary<string, List<(string name, string value)>> userDocs = new();

            Dictionary<string, List<BiometricImagesAtt>?> fingerPrint = new Dictionary<string, List<BiometricImagesAtt>?>();
            supportingDocs.Where(d => d.Type == _webCamTypeLookupId || d.Type == _fingerprintTypeLookupId || d.Type == _signatureTypeLookupId || _userDocTypeLookupId.Where(ul => ul.Id == d.Id).Any() || d.Type == new Guid("8f735355-8ee8-43a7-8c09-c50b40497152")).ToList().ForEach(doc =>
            {
                if (idObj.WifeId != null && doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Bride)!.ToLower() || doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Wife)!.ToLower())
                {
                    if (doc.Type == _webCamTypeLookupId)
                    {
                        userPhotos.Add(idObj.WifeId.ToString()!, doc.base64String);
                    }
                    else if (doc.Type == _fingerprintTypeLookupId)
                    {
                        fingerPrint.Add(idObj.WifeId?.ToString()!, doc.FingerPrint);
                    }
                    else if (_userDocTypeLookupId.Where(ul => ul.Id == doc.Type).Any())
                    {
                        var docType = _userDocTypeLookupId.Where(ul => ul.Id == doc.Type).Select(ul => ul.Value.Value<string>("en")).First();
                        userDocs.TryGetValue(idObj.WifeId.ToString(), out List<(string name, string value)> existingList);
                        (existingList ??= new List<(string name, string value)>()).Add((name: docType, value: doc.base64String));

                        userDocs[idObj.WifeId.ToString()!] = existingList;
                    }
                    else
                    {
                        signatures.Add(idObj.WifeId.ToString()!, doc.base64String);
                    }

                    supportingDocs.ToList().Remove(doc);
                }
                else if (idObj.MotherId != null && doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Mother)!.ToLower())
                {
                    if (doc.Type == _webCamTypeLookupId)
                    {
                        userPhotos.Add(idObj.MotherId.ToString()!, doc.base64String);
                    }
                    else if (doc.Type == _fingerprintTypeLookupId)
                    {
                        fingerPrint.Add(idObj.MotherId?.ToString()!, doc.FingerPrint);
                    }
                    else if (_userDocTypeLookupId.Where(ul => ul.Id == doc.Type).Any())
                    {
                        var docType = _userDocTypeLookupId.Where(ul => ul.Id == doc.Type).Select(ul => ul.Value.Value<string>("en")).First();
                        userDocs.TryGetValue(idObj.MotherId.ToString(), out List<(string name, string value)> existingList);
                        (existingList ??= new List<(string name, string value)>()).Add((name: docType, value: doc.base64String));

                        userDocs[idObj.MotherId.ToString()!] = existingList;
                    }
                    else
                    {
                        signatures.Add(idObj.MotherId.ToString()!, doc.base64String);
                    }

                    supportingDocs.ToList().Remove(doc);
                }
                else if (idObj.FatherId != null && doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Father)!.ToLower())
                {
                    if (doc.Type == _webCamTypeLookupId)
                    {
                        userPhotos.Add(idObj.FatherId.ToString()!, doc.base64String);
                    }
                    else if (doc.Type == _fingerprintTypeLookupId)
                    {
                        fingerPrint.Add(idObj.FatherId?.ToString()!, doc.FingerPrint);
                    }
                    else if (_userDocTypeLookupId.Where(ul => ul.Id == doc.Type).Any())
                    {
                        var docType = _userDocTypeLookupId.Where(ul => ul.Id == doc.Type).Select(ul => ul.Value.Value<string>("en")).First();
                        userDocs.TryGetValue(idObj.FatherId.ToString(), out List<(string name, string value)> existingList);
                        (existingList ??= new List<(string name, string value)>()).Add((name: docType, value: doc.base64String));

                        userDocs[idObj.FatherId.ToString()!] = existingList;
                    }
                    else
                    {
                        signatures.Add(idObj.FatherId.ToString()!, doc.base64String);

                    }

                    supportingDocs.ToList().Remove(doc);
                }
                else if (idObj.ChildId != null && doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Child)!.ToLower())
                {
                    if (doc.Type == _webCamTypeLookupId)
                    {
                        userPhotos.Add(idObj.ChildId.ToString()!, doc.base64String);
                    }
                    else if (doc.Type == _fingerprintTypeLookupId)
                    {
                        fingerPrint.Add(idObj.ChildId?.ToString()!, doc.FingerPrint);
                    }
                    else if (_userDocTypeLookupId.Where(ul => ul.Id == doc.Type).Any())
                    {
                        var docType = _userDocTypeLookupId.Where(ul => ul.Id == doc.Type).Select(ul => ul.Value.Value<string>("en")).First();
                        userDocs.TryGetValue(idObj.ChildId.ToString(), out List<(string name, string value)> existingList);
                        (existingList ??= new List<(string name, string value)>()).Add((name: docType, value: doc.base64String));

                        userDocs[idObj.ChildId.ToString()!] = existingList;
                    }
                    else
                    {
                        signatures.Add(idObj.ChildId.ToString()!, doc.base64String);

                    }

                    supportingDocs.ToList().Remove(doc);
                }
                else if (idObj.HusbandId != null && doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Groom)!.ToLower() || doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Husband)!.ToLower())
                {
                    if (doc.Type == _webCamTypeLookupId)
                    {
                        userPhotos.Add(idObj.HusbandId.ToString()!, doc.base64String);

                    }
                    else if (doc.Type == _fingerprintTypeLookupId)
                    {
                        fingerPrint.Add(idObj.HusbandId.ToString()!, doc.FingerPrint);

                    }
                    else if (_userDocTypeLookupId.Where(ul => ul.Id == doc.Type).Any())
                    {
                        var docType = _userDocTypeLookupId.Where(ul => ul.Id == doc.Type).Select(ul => ul.Value.Value<string>("en")).First();
                        userDocs.TryGetValue(idObj.HusbandId.ToString(), out List<(string name, string value)> existingList);
                        (existingList ??= new List<(string name, string value)>()).Add((name: docType, value: doc.base64String));

                        userDocs[idObj.HusbandId.ToString()!] = existingList;
                    }
                    else
                    {
                        signatures.Add(idObj.HusbandId.ToString()!, doc.base64String);

                    }
                    supportingDocs.ToList().Remove(doc);
                }
                else if (idObj.RegistrarId != null && doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Registrar)!.ToLower())
                {
                    Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ registerar");

                    if (doc.Type == _webCamTypeLookupId)
                    {
                        userPhotos.Add(idObj.RegistrarId.ToString()!, doc.base64String);

                    }
                    else if (doc.Type == _fingerprintTypeLookupId)
                    {
                        fingerPrint.Add(idObj.RegistrarId.ToString()!, doc.FingerPrint);

                    }
                    else if (_userDocTypeLookupId.Where(ul => ul.Id == doc.Type).Any())
                    {
                        var docType = _userDocTypeLookupId.Where(ul => ul.Id == doc.Type).Select(ul => ul.Value.Value<string>("en")).First();
                        userDocs.TryGetValue(idObj.RegistrarId.ToString(), out List<(string name, string value)> existingList);
                        (existingList ??= new List<(string name, string value)>()).Add((name: docType, value: doc.base64String));

                        userDocs[idObj.RegistrarId.ToString()!] = existingList;
                    }
                    else
                    {
                        signatures.Add(idObj.RegistrarId.ToString()!, doc.base64String);

                    }
                    supportingDocs.ToList().Remove(doc);
                }
                else if (idObj.DeceasedId != null && doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Deceased)!.ToLower())
                {
                    if (doc.Type == _webCamTypeLookupId)
                    {
                        userPhotos.Add(idObj.DeceasedId.ToString()!, doc.base64String);
                    }
                    else if (doc.Type == _fingerprintTypeLookupId)
                    {
                        fingerPrint.Add(idObj.DeceasedId.ToString()!, doc.FingerPrint);

                    }
                    else if (_userDocTypeLookupId.Where(ul => ul.Id == doc.Type).Any())
                    {
                        var docType = _userDocTypeLookupId.Where(ul => ul.Id == doc.Type).Select(ul => ul.Value.Value<string>("en")).First();
                        userDocs.TryGetValue(idObj.DeceasedId.ToString(), out List<(string name, string value)> existingList);
                        (existingList ??= new List<(string name, string value)>()).Add((name: docType, value: doc.base64String));

                        userDocs[idObj.DeceasedId.ToString()!] = existingList;
                    }
                    else
                    {
                        signatures.Add(idObj.DeceasedId.ToString()!, doc.base64String);
                    }
                    supportingDocs.ToList().Remove(doc);
                }
            });
            if (idObj.WitnessIds != null)
            {

                for (int i = 0; i < idObj.WitnessIds?.Count; i++)
                {

                    var witnessDoc = supportingDocs
                                .Where(doc => (doc.Type == _webCamTypeLookupId || doc.Type == _fingerprintTypeLookupId)
                                && doc.Label.ToLower() == $"{Enum.GetName<DocumentLabel>(DocumentLabel.Witness)?.ToLower()}{i}")
                                .FirstOrDefault();

                    if (witnessDoc != null)
                    {
                        var witnessId = idObj.WitnessIds.ToList()[i].ToString();
                        if (witnessDoc.Type == _webCamTypeLookupId)
                        {
                            userPhotos.Add(witnessId, witnessDoc.base64String);
                        }
                        else if (witnessDoc.Type == _fingerprintTypeLookupId)
                        {
                            fingerPrint.Add(witnessId, witnessDoc.FingerPrint);

                        }
                        else if (_userDocTypeLookupId.Where(ul => ul.Id == witnessDoc.Type).Any())
                        {
                            var docType = _userDocTypeLookupId.Where(ul => ul.Id == witnessDoc.Type).Select(ul => ul.Value.Value<string>("en")).First();
                            userDocs.TryGetValue(witnessId, out List<(string name, string value)> existingList);
                            (existingList ??= new List<(string name, string value)>()).Add((name: docType, value: witnessDoc.base64String));

                            userDocs[witnessId!] = existingList;
                        }
                        else
                        {
                            signatures.Add(witnessId, witnessDoc.base64String);
                        }
                        supportingDocs.ToList().Remove(witnessDoc);
                    }
                }
            }
            return new ExtractedDocDTO
            {
                UserPhoto = userPhotos,
                FingerPrints = fingerPrint,
                Signatures = signatures,
                OtherDocs = supportingDocs,
                UserDocs = userDocs

            };

        }
        public (Dictionary<string, string> userPhotos, IEnumerable<SupportingDocument> otherDocs) ExtractOldSupportingDocs(PersonIdObj idObj, IEnumerable<SupportingDocument> supportingDocs)
        {
            Dictionary<string, string> userPhotos = new Dictionary<string, string>();
            supportingDocs.Where(d => d.Type == _webCamTypeLookupId).ToList().ForEach(doc =>
            {

                if (idObj.WifeId != null && doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Bride)!.ToLower() || doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Wife)!.ToLower())
                {
                    userPhotos.Add(idObj.WifeId.ToString()!, doc.Id.ToString());
                    supportingDocs.ToList().Remove(doc);
                }
                else if (idObj.HusbandId != null && doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Groom)!.ToLower() || doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Husband)!.ToLower())
                {
                    userPhotos.Add(idObj.HusbandId.ToString()!, doc.Id.ToString());
                    supportingDocs.ToList().Remove(doc);
                }
                else if (idObj.RegistrarId != null && doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Registrar)!.ToLower())
                {
                    userPhotos.Add(idObj.RegistrarId.ToString()!, doc.Id.ToString());
                    supportingDocs.ToList().Remove(doc);
                }
                else if (idObj.ChildId != null && doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Child)!.ToLower())
                {
                    userPhotos.Add(idObj.ChildId.ToString()!, doc.Id.ToString());
                    supportingDocs.ToList().Remove(doc);
                }
                else if (idObj.DeceasedId != null && doc.Label.ToLower() == Enum.GetName<DocumentLabel>(DocumentLabel.Deceased)!.ToLower())
                {
                    userPhotos.Add(idObj.DeceasedId.ToString()!, doc.Id.ToString());
                    supportingDocs.ToList().Remove(doc);
                }
            });
            if (idObj.WitnessIds != null)
            {

                for (int i = 0; i < idObj.WitnessIds?.Count; i++)
                {

                    var witnessDoc = supportingDocs
                                .Where(doc => doc.Type == _webCamTypeLookupId
                                && doc.Label.ToLower() == $"{Enum.GetName<DocumentLabel>(DocumentLabel.Witness)?.ToLower()}{i}")
                                .FirstOrDefault();

                    if (witnessDoc != null)
                    {
                        var witnessId = idObj.WitnessIds.ToList()[i].ToString();
                        userPhotos.Add(witnessId, witnessDoc.Id.ToString());
                        supportingDocs.ToList().Remove(witnessDoc);
                    }
                }
            }
            return (userPhotos: userPhotos, otherDocs: supportingDocs);
        }

        public List<string> getFingerprintUrls(List<string> personIds)
        {
            List<string> fingerPrintUrls = new List<string>();
            personIds.ForEach(id =>
            {
                fingerPrintUrls.AddRange(_fileService.GetFileNamesInfolder(Path.Combine("Resources", "fingerprints", id)));
            });
            return fingerPrintUrls;
        }
        public List<Dictionary<string, string>> getSingleFingerprintUrls(string? personId)
        {
            return personId == null ? new List<Dictionary<string, string>>()
                        : _fileService.GetFileNamesInfolder(Path.Combine("Resources", "fingerprints", personId))
                        .Select(url => new Dictionary<string, string>{
                                {
                                    Path.GetFileNameWithoutExtension(url)
                                     , url
                                }

                            }).ToList();
        }

    }
}
