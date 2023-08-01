using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
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
        public JObject GetContent(JArray? content)
        {
            JObject oldData = new();
            foreach (var (columnName, originalValue) in
                            from change in content
                            let changeObject = (JObject)change
                            let columnName = changeObject?.GetValue("ColumnName")?.ToString()
                            let originalValue = changeObject?.GetValue("OriginalValue")
                            where columnName != null
                            select (columnName, originalValue))
            {
                oldData.Add(columnName, originalValue);
            }

            return oldData;
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
                        obj = JObject.Parse((string)(property?.Value ?? "{}"));
                        newProperty = new(newName, obj);
                    }
                    catch (System.Exception)
                    {
                        objArray = JArray.Parse((string)(property?.Value ?? "[]"));
                        newProperty = new(newName, objArray);
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

    }
}