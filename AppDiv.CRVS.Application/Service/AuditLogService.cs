using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities.Audit;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Tls;

namespace AppDiv.CRVS.Application.Service
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditlog;
        public AuditLogService(IAuditLogRepository auditLog)
        {
            _auditlog = auditLog;
        }
        public JObject GetContent(JObject? content)
        {
            if (content is null) return new JObject();
            var changes = content?.Value<JArray?>("Changes")?.ToList();
            var columnValues = content?.Value<JObject>("ColumnValues");
            if (changes is not null)
            {
                foreach (var prop in columnValues?.Properties().ToList()!)
                {
                    if (changes.Select(c => c.Value<string>("ColumnName")).Contains(prop.Name))
                    {
                        var oldValue = changes.Where(c => c.Value<string>("ColumnName") == prop.Name && 
                                                        c.Value<string?>("OriginalValue") != Guid.Empty.ToString() && 
                                                        c.Value<string?>("OriginalValue") != null)
                                                        .FirstOrDefault()?
                                                        .Value<string?>("OriginalValue");
                        if (oldValue is not null)
                        {
                            prop.Value = oldValue;
                        }
                    }
                }
            }
            // foreach (var (columnName, originalValue, newValue) in
            //                 from change in content
            //                 let changeObject = (JObject)change
            //                 let columnName = changeObject?.GetValue("ColumnName")?.ToString()
            //                 let originalValue = changeObject?.GetValue("OriginalValue")
            //                 let newValue = changeObject?.GetValue("NewValue")
            //                 where columnName != null
            //                 select (columnName, originalValue, newValue))
            // {
            //     var oldValue = originalValue;
            //     // Console.WriteLine($"{columnName} old Val= {oldValue} is empty {Object.Equals(oldValue, Guid.Empty)} || {oldValue.ToString() == Guid.Empty.ToString()}");
            //     if (oldValue.ToString() == Guid.Empty.ToString() || (oldValue.ToString() == ""))
            //         oldValue = newValue;
            //     oldData.Add(columnName, oldValue);
            // }

            return columnValues;
        }
        public JObject GetNestedElements(JObject? content)
        {
            var result = new JObject();
            if (content != null)
            {
                foreach (var property in content.Properties())
                {
                    string key = property.Name.TrimEnd("Id".ToCharArray());
                    JToken value = property.Value;
                    result[property.Name] = property.Value;
                    if (!string.IsNullOrWhiteSpace(key) && property.Name.EndsWith("Id"))
                    {
                        result[key] = _auditlog.GetAll().OrderByDescending(a => a.AuditDate).FirstOrDefault(a => a.TablePk == value.ToString())?.AuditDataJson?.Value<JObject>("ColumnValues");
                    }
                    
                }
            }
            return ConvertStringToObject(result);
        }
        public JObject GetAuditArchive(JObject content)
        {
            var result = GetNestedElements(content);
            foreach (var property in result.Properties().ToList())
            {
                if (property.Value.Type == JTokenType.Object)
                {
                    result[property.Name] = GetNestedElements((JObject)property.Value);
                }
            }
            return result;
        }

        public static JObject Filter(JObject single)
        {
            if (single.Value<string>("ColumnName").EndsWith("Str"))
            {
                single["ColumnName"] = single.Value<string>("ColumnName")[..^3];
                try
                {
                    single["NewValue"] = JObject.Parse((string)(single.Value<string>("NewValue") ?? "{}"));
                    single["OriginalValue"] = JObject.Parse((string)(single.Value<string>("OriginalValue") ?? "{}"));
                    // objOld = JObject.Parse((string)(single.Value<string>("OriginalValue") ?? "{}"));
                    // newProperty = new(newName, obj);
                }
                catch (System.Exception)
                {
                    try
                    {
                        single["NewValue"] = JArray.Parse((string)(single.Value<string>("NewValue") ?? "{}"));
                        single["OriginalValue"] = JArray.Parse((string)(single.Value<string>("OriginalValue") ?? "{}"));
                    }
                    catch (System.Exception)
                    {
                        
                        // throw;
                    }
                    // newProperty = new(newName, objArray);
                }
            }
            return single;
        }
        private JObject ConvertStringToObject(JObject content)
        {
            if (content is null) return new JObject();
            foreach (JProperty property in content.DescendantsAndSelf().OfType<JProperty>().ToList())
            {
                if (property.Name.EndsWith("Str"))
                {
                    string newName = property.Name[..^3];
                    JObject obj;
                    JArray objArray;
                    JProperty newProperty;
                    try
                    {
                        obj = JObject.Parse((string)(property?.Value ?? "{}")!);
                        newProperty = new(newName, obj);
                    }
                    catch (System.Exception)
                    {
                        try
                        {
                            objArray = JArray.Parse((string)(property?.Value ?? "[]")!);
                            newProperty = new(newName, objArray);
                        }
                        catch (System.Exception)
                        {
                            newProperty = new(newName, property?.Value);
                            // throw;
                        }
                    }
                    property?.Parent?.Add(newProperty);
                    property?.Remove();
                }
            }
            return content;
        }
        public static JArray GetChanges(JArray? content)
        {
            JArray oldData = new();
            if (content != null)
            {
                foreach (var changeObject in content)
                {
                    var change = (JObject)changeObject;
                    Console.WriteLine("xxxxxxx : {0}{1}{2}",change?.GetValue("OriginalValue"), change?.GetValue("NewValue") ,Guid.TryParse(change?.GetValue("NewValue")?.ToString(), out _));
                    if (!Object.Equals(change?.GetValue("OriginalValue"), change?.GetValue("NewValue")) 
                            || Guid.TryParse(change?.GetValue("NewValue")?.ToString(), out _))
                    {
                        oldData.Add(Filter(change));
                    }
                }
            }
            return oldData;
        }
        public JArray GetAllChanges(JArray content, DateTime auditDate, string entityType)
        {
            var changes = GetChanges(content);
            foreach (JObject change in changes.ToList())
            {
                if (change.Value<string>("ColumnName")!.EndsWith("Id") && !change.Value<string>("ColumnName")!.Equals("Id"))
                {
                    
                    var newData = this.GetAuditById(change.Value<string>("NewValue"), auditDate);
                    // var newData = GetAllNestedIds(change.Value<string>("NewValue"), auditDate);
                    if (newData == null)
                        continue;
                    // if (entityType.EndsWith("Event"))
                        newData = this.GetAll((newData), auditDate);
                    newData = this.GetAll((newData), auditDate);
                    var obj = new JObject
                    {
                        // obj.Add("Old" + change.Value<string>("ColumnName"), oldData);
                        { change.Value<string>("ColumnName")![..^2], newData == null ? newData : (newData) }
                    };
                    changes.Add(obj);
                    // changes.Add(newData);
                    // changes = GetAllChanges(changes, auditDate);
                }

            }
            return changes;
        }

        public JArray GetAll(JArray single, DateTime auditDate)
        {
            var innerChanges = GetChanges(single);
            foreach (var item in innerChanges.ToList())
            {
                if (item.Value<string>("ColumnName")!.EndsWith("Id") && !item.Value<string>("ColumnName")!.Equals("Id"))
                {
                    var data = this.GetAuditById(item.Value<string>("NewValue"), auditDate);
                    var obj = new JObject
                    {
                        { item.Value<string>("ColumnName")![..^2], data != null ? GetChanges(data) : data }
                    };
                    innerChanges.Add(obj);
                    // getAll(single);
                }
            }
            return innerChanges;
        }
        public JArray? GetAuditById(string id, DateTime auditDate)
        {
            return _auditlog.GetAll()
                    .OrderBy(a => a.AuditDate)
                    .Where(a => a.AuditDate >= auditDate.AddMinutes(-1) && a.AuditDate <= auditDate.AddMinutes(1))
                    .FirstOrDefault(a => a.TablePk == id)?
                    .AuditDataJson?.Value<JArray>("Changes");
        }

        // for event 

        public JObject EventAudit(AuditLog audit)
        {
            var events = audit.EntityType switch
            {
                "BirthEvent" => BirthAudit(audit.AuditDataJson, audit.Action, audit),
                "DeathEvent" => DeathAudit(audit.AuditDataJson, audit.Action, audit),
                "MarriageEvent" => MarriageAudit(audit.AuditDataJson, audit.Action, audit),
                "DivorceEvent" => DivorceAudit(audit.AuditDataJson, audit.Action, audit),
                "AdoptionEvent" => AdoptionAudit(audit.AuditDataJson, audit.Action, audit),
                _ => new JObject()
            };
            return events;
        }
        public JObject BirthAudit(JObject content, string action,AuditLog audit)
        {
            JObject? oldData = null;
            JObject newData;
            var birthIncludes = new List<string> { "EventId", "FatherId", "MotherId" };

            if (action == "Update")
            {
                oldData = Include(GetContent(content), birthIncludes, audit.AuditDate, true);
                oldData["Event"] = EventIncludes(oldData.Value<JObject>("Event")!, null!, audit.AuditDate, true);
            }
            newData = Include(content?.Value<JObject>("ColumnValues")!, birthIncludes, audit.AuditDate, false);
            newData["Event"] = EventIncludes(newData?.Value<JObject>("Event")!, null, audit.AuditDate, false);
            
            return new JObject() { ["newData"] = ConvertStringToObject(newData), ["oldData"] = ConvertStringToObject(oldData) };
        }
        public JObject DeathAudit(JObject content, string action,AuditLog audit)
        {
            JObject? oldData = null;
            JObject newData;
            var deathIncludes = new List<string> { "EventId" };

            if (action == "Update")
            {
                oldData = Include(GetContent(content), deathIncludes, audit.AuditDate, true);
                oldData["Event"] = EventIncludes(oldData.Value<JObject>("Event")!, null!, audit.AuditDate, true);
            }
            newData = Include(content?.Value<JObject>("ColumnValues")!, deathIncludes, audit.AuditDate, false);
            newData["Event"] = EventIncludes(newData?.Value<JObject>("Event")!, null, audit.AuditDate, false);
            
            return new JObject() { ["newData"] = ConvertStringToObject(newData), ["oldData"] = ConvertStringToObject(oldData) };
        }
        public JObject MarriageAudit(JObject content, string action,AuditLog audit)
        {
            JObject? oldData = null;
            JObject newData;
            var marriageIncludes = new List<string> { "EventId", "BrideInfoId" };

            if (action == "Update")
            {
                oldData = Include(GetContent(content), marriageIncludes, audit.AuditDate, true);
                oldData["Event"] = EventIncludes(oldData.Value<JObject>("Event")!, null!, audit.AuditDate, true);
            }
            newData = Include(content?.Value<JObject>("ColumnValues")!, marriageIncludes, audit.AuditDate, false);
            newData["Event"] = EventIncludes(newData?.Value<JObject>("Event")!, null, audit.AuditDate, false);
            
            return new JObject() { ["newData"] = ConvertStringToObject(newData), ["oldData"] = ConvertStringToObject(oldData) };
        }
        public JObject DivorceAudit(JObject content, string action,AuditLog audit)
        {
            JObject? oldData = null;
            JObject newData;
            var divorceIncludes = new List<string> { "EventId", "CourtCaseId", "DivorcedWifeId" };

            if (action == "Update")
            {
                oldData = Include(GetContent(content), divorceIncludes, audit.AuditDate, true);
                oldData["Event"] = EventIncludes(oldData.Value<JObject>("Event")!, null!, audit.AuditDate, true);
            }
            newData = Include(content?.Value<JObject>("ColumnValues")!, divorceIncludes, audit.AuditDate, false);
            newData["Event"] = EventIncludes(newData?.Value<JObject>("Event")!, null, audit.AuditDate, false);
            
            return new JObject() { ["newData"] = ConvertStringToObject(newData), ["oldData"] = ConvertStringToObject(oldData) };
        }
        public JObject AdoptionAudit(JObject content, string action,AuditLog audit)
        {
            JObject? oldData = null;
            JObject newData;
            var adoptionIncludes = new List<string> { "EventId", "AdoptiveFatherId", "AdoptiveMotherId", "CourtCaseId" };

            if (action == "Update")
            {
                oldData = Include(GetContent(content), adoptionIncludes, audit.AuditDate, true);
                oldData["Event"] = EventIncludes(oldData.Value<JObject>("Event")!, null!, audit.AuditDate, true);
            }
            newData = Include(content?.Value<JObject>("ColumnValues")!, adoptionIncludes, audit.AuditDate, false);
            newData["Event"] = EventIncludes(newData?.Value<JObject>("Event")!, null, audit.AuditDate, false);
            
            return new JObject() { ["newData"] = ConvertStringToObject(newData), ["oldData"] = ConvertStringToObject(oldData) };
        }

        public JObject Include(JObject content, List<string> keys, DateTime auditDate, bool changes = false)
        {
            if (content is null) return new JObject();
            foreach (var property in content.Properties().ToList())
            {
                if (keys.Contains(property.Name))
                {
                    string key = property.Name.TrimEnd("Id".ToCharArray());
                    if (!changes)
                    {
                        content[key] = _auditlog.GetAll()
                                                .OrderByDescending(a => a.AuditDate)
                                                .Where(a => a.AuditDate >= auditDate.AddMinutes(-1) && a.AuditDate <= auditDate.AddMinutes(1))
                                                .FirstOrDefault(a => a.TablePk == property.Value.ToString())?
                                                .AuditDataJson?
                                                .Value<JObject>("ColumnValues");
                    }
                    else
                    {
                        content[key] = GetContent(
                                                _auditlog.GetAll()
                                                    .OrderByDescending(a => a.AuditDate)
                                                    .Where(a => a.AuditDate >= auditDate.AddMinutes(-1) && a.AuditDate <= auditDate.AddMinutes(1))
                                                    .FirstOrDefault(a => a.TablePk == property.Value.ToString())?
                                                    .AuditDataJson!);
                    }
                }
            }
            return content;
        }
        public JObject EventIncludes(JObject content, List<string>? keys, DateTime auditDate, bool changes = false)
        {
            if (keys is null)
            {
                keys = new List<string>() { "EventOwenerId" };
            }
            if (content is null) return new JObject();
            foreach (var property in content.Properties().ToList())
            {
                if (keys.Contains(property.Name))
                {
                    string key = property.Name.TrimEnd("Id".ToCharArray());
                    if (!changes)
                    {
                        content[key] = _auditlog.GetAll()
                                                .OrderByDescending(a => a.AuditDate)
                                                .FirstOrDefault(a => a.TablePk == property.Value.ToString())?
                                                .AuditDataJson?
                                                .Value<JObject>("ColumnValues");
                    }
                    else 
                    {
                        content[key] = GetContent(
                                                 _auditlog.GetAll()
                                                    .OrderByDescending(a => a.AuditDate)
                                                    .FirstOrDefault(a => a.TablePk == property.Value.ToString())?
                                                    .AuditDataJson);
                    }
                }
            }
            if (!changes)
            {
                content["Registrar"] = Include(
                                            _auditlog.GetAll()
                                                .OrderByDescending(a => a.AuditDate)
                                                .FirstOrDefault(a => a.EntityType == "Registrar" && 
                                                    a.AuditData.Contains($"\"EventId\": \"{content.Value<string>("Id")}\""))?
                                                .AuditDataJson?
                                                .Value<JObject>("ColumnValues")!, 
                                            new List<string> { "RegistrarInfoId" },
                                            auditDate,
                                            changes);
            }
            else
            {
                content["Registrar"] = Include(
                                            GetContent(
                                                _auditlog.GetAll()
                                                .OrderByDescending(a => a.AuditDate)
                                                .FirstOrDefault(a => a.EntityType == "Registrar" && 
                                                    a.AuditData.Contains($"\"EventId\": \"{content.Value<string>("Id")}\""))?
                                                .AuditDataJson), 
                                            new List<string> { "RegistrarInfoId" },
                                            auditDate,
                                            changes);
            }
            return content;
        }

    }

}