using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Application.Service;
using System.Reflection;

namespace AppDiv.CRVS.Application.Features.Certificates.Command.Create
{
    public class CreateCertificateCommandValidator : AbstractValidator<CreateCertificateCommand>
    {
        private readonly ICertificateRepository _repo;
        private readonly ILogger<CreateCertificateCommand> log;
        public CreateCertificateCommandValidator(ICertificateRepository repo, ILogger<CreateCertificateCommand> log)
        {

            _repo = repo;
            this.log = log;
            // RuleFor(p => p.Certificate.EventId).MustAsync(validator.CheckForForeignKeyAsync);
            // validator.CheckForForeignKeyAsync)
            //     (x, y, z) =>
            // {
            //     // string typeName = "MyNamespace.MyClass"; // Type.FullName
            //     // string assemblyName = "MyAssemblyName"; // MyAssembly.FullName or MyAssembly.GetName().Name
            //     // string assemblyQualifiedName = Assembly.CreateQualifiedName(assemblyName , typeName);
            //     // Type myClassType = Type.GetType(assemblyQualifiedName);
            //     string yo = typeof(Certificate).AssemblyQualifiedName;
            //     Type type = Type.GetType($"{yo}");
            // AppDiv.CRVS.Application, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
            // AppDiv.CRVS.Domain.Entities.Certificate, AppDiv.CRVS.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
            //     log.LogCritical(type?.ToString());
            //     log.LogCritical(yo);
            //     log.LogCritical(z.DisplayName);
            //     log.LogCritical(Assembly.GetExecutingAssembly().ToString());
            //     if (type != null)
            //     {
            //         int dotIndex = z.PropertyName.IndexOf('.');
            //         // if (dotIndex >= 0)
            //         // {
            //         string propertyName = z.PropertyName.Substring(dotIndex + 1, z.PropertyName.Length - dotIndex - 3);
            //         // Console.WriteLine(outputString); // Output: Event
            //         // }
            //         // var propertyName = z.DisplayName.Substring(0, z.PropertyName.Length - 2);
            //         PropertyInfo property = type.GetProperty(propertyName);

            //         if (property != null)
            //         {
            //             Type propertyType = property.PropertyType;
            //             log.LogCritical(propertyType.ToString());
            //             // Do something with the property type
            //         }
            //         else
            //         {
            //             // Property not found
            //         }
            //     }
            //     else
            //     {
            //         // Type not found
            //     }
            //     return false;
            // });

            // _mediator = mediator;
            // RuleFor(p => p.Certificate.)
            //     .Must(x => x != Guid.Empty).WithMessage("Payment Type must not be empty.");
            // // .NotEmpty().WithMessage("{PropertyName} is required.")
            // // .NotNull().
            // // .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
            // RuleFor(p => p.Certificate.EventLookupId)
            //     .Must(x => x != Guid.Empty).WithMessage("Event must not be empty.");

            // RuleFor(p => p.Certificate.AddressId)
            //     .Must(x => x != Guid.Empty).WithMessage("Address must not be empty.");
            // RuleFor(pr => pr.Certificate.Amount)
            //     .NotEmpty().WithMessage("{PropertyName} is required.")
            //     .NotNull();

            // RuleFor(e => e)
            //   .MustAsync(phoneNumberUnique)
            //   .WithMessage("A Customer phoneNumber already exists.");
        }
        public async Task<bool> CheckForForeignKeyAsync(CreateCertificateCommand command, Guid id, ValidationContext<CreateCertificateCommand> context, CancellationToken cancellationToken)
        {
            var prop = context.PropertyName;
            log.LogCritical(prop.ToString());
            var assembly = "AppDiv.CRVS.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            // string yo = typeof(Certificate).AssemblyQualifiedName;
            int dotIndex = context.PropertyName.IndexOf('.');
            string className = context.PropertyName.Substring(0, dotIndex);
            Type classType = Type.GetType($"AppDiv.CRVS.Domain.Entities.{className}, {assembly}");
            log.LogCritical(classType.ToString());
            if (classType != null)
            {
                // int dotIndex = context.PropertyName.IndexOf('.');
                if (dotIndex >= 0)
                {
                    string propertyName = context.PropertyName.Substring(dotIndex + 1, context.PropertyName.Length - dotIndex - 3);
                    PropertyInfo property = classType.GetProperty(propertyName);
                    log.LogCritical(property.ToString());
                    if (property != null)
                    {
                        Type propertyType = property.PropertyType;
                        log.LogCritical(propertyType.ToString());
                        // int lastDotIndex = propertyType.To.LastIndexOf('.');
                        // string outputString = inputString.Substring(lastDotIndex + 1);
                        // await _dbContext.Set<propertyType>().FindAsync(id);
                        object entitySet = _repo.GetType().GetMethod("Set").MakeGenericMethod(propertyType).Invoke(_repo, null);
                        object entity = await ((dynamic)entitySet).FindAsync(id);
                        // log.LogCritical(entity.ToString());
                        Console.WriteLine(entity);
                        // Do something with the property type
                        return entity != null ? true : false;
                    }
                }
            }
            // log.LogCritical(name?.ToString());

            return false;
        }
        //private async Task<bool> phoneNumberUnique(CreateCustomerCommand request, CancellationToken token)
        //{
        //    var member = await _repo.GetByIdAsync(request.FirstName);
        //    if (member == null)
        //        return true;
        //    else return false;
        //}

    }
}