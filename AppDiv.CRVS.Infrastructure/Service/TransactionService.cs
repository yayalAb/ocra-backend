

using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Infrastructure.Service
{
    public class TransactionService : ITransactionService
    {
        private readonly CRVSDbContext _context;

        public TransactionService(CRVSDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> CreateTransaction(TransactionRequestDTO transactionObj)
        {
            
            var transaction = CustomMapper.Mapper.Map<Transaction>(transactionObj);
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return transaction.Id;
        }
        public IQueryable<Transaction> GetAllGrid()
        {
            return _context.Transactions
                            .AsNoTracking()
                            .Include(t => t.Request)
                            .Include(t => t.Request!.CivilRegOfficer)
                            .Include(t => t.Request!.CorrectionRequest.Event)
                            .Include(t => t.Request!.AuthenticationRequest.Certificate.Event)
                            .Include(t => t.Request!.VerficationRequest.Event)
                            .Include(t => t.Request!.PaymentRequest.Event)
                            .Include(t => t.Workflow!.Steps)
                            .Include(t => t.CivilRegOfficer!.PersonalInfo);
        }

        public IQueryable<Transaction> GetAll()
        {
            return _context.Transactions
                            .AsNoTracking()
                            .Include(t => t.Request)
                            .Include(t => t.Request!.CivilRegOfficer)
                            // .Include(t => t.Request!.CorrectionRequest.Event.EventOwener)
                            // .Include(t => t.Request!.AuthenticationRequest.Certificate)
                            // .Include(t => t.Request!.VerficationRequest.Event.EventOwener)
                            // .Include(t => t.Request!.PaymentRequest.Event)
                            .Include(t => t.Workflow!.Steps)
                            .Include(t => t.CivilRegOfficer!.PersonalInfo).AsQueryable();
        }
        //     public async Task<List<Transaction>> GetTransactions(TransactionRequestDTO transactionObj){
        //     var transaction = CustomMapper.Mapper.Map<Transaction>(transactionObj);
        //     await _context.Transactions.AddAsync(transaction);
        //     await _context.SaveChangesAsync();
        //     return transaction.Id;
        //    }

        public Transaction GetTransaction(Guid? id)
        {
            return _context.Transactions
                .Include(t => t.CivilRegOfficer.PersonalInfo)
                .Include(t => t.Request)
                // .ThenInclude(r => r.CivilRegOfficer.ApplicationUser)
                .SingleOrDefault(t => t.Id == id);
        }
    }
}

