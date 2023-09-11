using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface ITransactionService
    {
        public Task<Guid> CreateTransaction(TransactionRequestDTO transactionObj);
        IQueryable<Transaction> GetAllGrid();

    }
}