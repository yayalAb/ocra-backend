

using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Service
{
    public class TransactionService : ITransactionService
    {
        private readonly CRVSDbContext _context;

        public TransactionService(CRVSDbContext context)
        {
            _context = context;
        }
       public async Task<Guid> CreateTransaction(TransactionRequestDTO transactionObj){
        var transaction = CustomMapper.Mapper.Map<Transaction>(transactionObj);
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
        return transaction.Id;
       }
    //     public async Task<List<Transaction>> GetTransactions(TransactionRequestDTO transactionObj){
    //     var transaction = CustomMapper.Mapper.Map<Transaction>(transactionObj);
    //     await _context.Transactions.AddAsync(transaction);
    //     await _context.SaveChangesAsync();
    //     return transaction.Id;
    //    }

    }
}

