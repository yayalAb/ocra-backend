using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Newtonsoft.Json.Linq;

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
            foreach (JProperty property in content?.Properties())
            {
                string key = property.Name.TrimEnd("Id".ToCharArray());
                JToken value = property.Value;
                result[property.Name] = property.Value;
                if (!string.IsNullOrWhiteSpace(key))
                {
                    result[key] = _auditlog.GetAll().OrderByDescending(a => a.AuditDate).FirstOrDefault(a => a.TablePk == value.ToString())?.AuditDataJson?.Value<JObject>("ColumnValues");
                }
            }
            return ConvertStringToObject(result);
        }
        private JObject ConvertStringToObject(JObject content)
        {
            foreach (JProperty property in content.DescendantsAndSelf().OfType<JProperty>().ToList())
            {
                if (property.Name.EndsWith("Str"))
                {
                    string newName = property.Name.Substring(0, property.Name.Length - 3);
                    JObject obj = JObject.Parse((string)property.Value);
                    JProperty newProperty = new JProperty(newName, obj);
                    property?.Parent?.Add(newProperty);
                    property.Remove();
                }
            }
            return content;
        }

    
    }
}