using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using FluentValidation;

namespace AppDiv.CRVS.Application.Service
{
    public interface ICustomValidator<T>
    {
        Task<bool> CheckForForeignKeyAsync(T command, Guid nullable, ValidationContext<T> context, CancellationToken cancellationToken);
        bool IsGuidNullOrEmpty(Guid? id);
        // public static AbstractValidator<T> Validator;
    }
    // public class Validator : AbstractValidator<T>
    // {

    // }
}