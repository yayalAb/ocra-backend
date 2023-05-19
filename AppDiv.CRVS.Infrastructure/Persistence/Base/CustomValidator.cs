using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Infrastructure.Persistence.Base
{
    public class CustomValidator<T> : ICustomValidator<T>
    {
        private readonly CRVSDbContext _dbContext;
        // private readonly ILogger logger;

        public CustomValidator(CRVSDbContext dbContext
        // , ILogger logger
        )
        {
            this._dbContext = dbContext;
            // this.logger = logger;
        }

        public async Task<bool> CheckForForeignKeyAsync(T command, Guid id, ValidationContext<T> context, CancellationToken cancellationToken)
        {
            var prop = context.PropertyName;
            Debug.WriteLine("This is a debug message." + prop);
            // logger.LogCritical(prop.ToString());
            var assembly = Assembly.GetExecutingAssembly();
            // string yo = typeof(Certificate).AssemblyQualifiedName;
            int dotIndex = context.PropertyName.IndexOf('.');
            string className = context.PropertyName.Substring(0, dotIndex);
            Type classType = Type.GetType($"AppDiv.CRVS.Domain.Entities.{className}, {assembly}");
            // logger.LogCritical(classType.ToString());
            if (classType != null)
            {
                // int dotIndex = context.PropertyName.IndexOf('.');
                if (dotIndex >= 0)
                {
                    string propertyName = context.PropertyName.Substring(dotIndex + 1, context.PropertyName.Length - dotIndex - 3);
                    PropertyInfo property = classType.GetProperty(propertyName);
                    // logger.LogCritical(property.ToString());
                    if (property != null)
                    {
                        Type propertyType = property.PropertyType;
                        // await _dbContext.Set<propertyType>().FindAsync(id);
                        object entitySet = _dbContext.GetType().GetMethod("Set").MakeGenericMethod(propertyType).Invoke(_dbContext, null);
                        object entity = await ((dynamic)entitySet).FindAsync(id);
                        Debug.WriteLine("This is a debug message." + entity);
                        // logger.LogCritical(propertyType.ToString());
                        // Do something with the property type
                        return entity != null ? true : false;
                    }
                }
            }
            // log.LogCritical(name?.ToString());

            return false;
        }
        public bool IsGuidNullOrEmpty(Guid? id)
        {
            return !id.HasValue && id.Equals(Guid.Empty);
        }

    }
}