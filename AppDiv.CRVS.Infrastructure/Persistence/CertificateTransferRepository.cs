
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Application.Features.CertificateStores.CertificateTransfers.Command.Create;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class CertificateTransferRepository : BaseRepository<CertificateSerialTransfer>, ICertificateTransferRepository
    {
        private readonly CRVSDbContext _dbContext;
        private DatabaseFacade Database => _dbContext.Database;
        public CertificateTransferRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task InsertWithRangeAsync(CertificateSerialTransfer transfer, CancellationToken cancellationToken)
        {
            var executionStrategy = this.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = this.Database.BeginTransaction())
                {
                    try
                    {
                        Address? recieverAddress = _dbContext.Users.Include(i => i.Address).ThenInclude(a => a.CertificateSerialRanges)
                                                        .Where(u => u.Id == transfer.RecieverId)
                                                        .FirstOrDefault()?.Address;
                        if (transfer.SenderId == null)
                        {
                            var recieverRange = new CertificateSerialRange
                            {
                                From = transfer.From,
                                To = transfer.To,
                                AddressId = recieverAddress.Id
                            };
                            transfer.Status = true;
                            _dbContext.CertificateSerialRanges.Add(recieverRange);
                        }


                        await base.InsertAsync(transfer, cancellationToken);
                        await base.SaveChangesAsync(cancellationToken);
                        await transaction.CommitAsync();
                    }
                    catch (System.Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            });
        }


        public async Task UpdateWithRangeAsync(CertificateSerialTransfer transfer, CancellationToken cancellationToken)
        {
            var executionStrategy = this.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = this.Database.BeginTransaction())
                {
                    try
                    {
                        Address? recieverAddress = _dbContext.Users.Include(i => i.Address).ThenInclude(a => a.CertificateSerialRanges)
                                                        .Where(u => u.Id == transfer.RecieverId)
                                                        .FirstOrDefault()?.Address;
                        Address? senderAddress = (transfer.SenderId != null)
                                                    ? _dbContext.Users.Include(i => i.Address).ThenInclude(a => a.CertificateSerialRanges)
                                                        .Where(u => u.Id == transfer.SenderId)
                                                        .FirstOrDefault()?.Address : null;
                        var senderRange = senderAddress?.CertificateSerialRanges?
                                                        .Where(r => r.From <= transfer.From)
                                                        .Where(r => r.To >= transfer.To)
                                                        .FirstOrDefault();
                        // CertificateSerialRange? senderRange = (recieverAddress != null) ? _dbContext.CertificateSerialRanges.Where(r => r.AddressId == senderAddress.Id)
                        //                                 .Where(r => r.From >= transfer.From)
                        //                                 .Where(r => r.To <= transfer.To)
                        //                                 .FirstOrDefault() : null;
                        // _dbContext.CertificateSerialRanges.Add(range);
                        // CertificateSerialRange? recieverRange = (recieverAddress != null) ? _dbContext.CertificateSerialRanges.Where(r => r.AddressId == recieverAddress.Id)
                        // .Where(r => r.From >= transfer.From)
                        // .Where(r => r.To <= transfer.To)
                        // .FirstOrDefault() : null;
                        var recieverRange = new CertificateSerialRange
                        {
                            From = transfer.From,
                            To = transfer.To,
                            AddressId = recieverAddress.Id
                        };
                        _dbContext.CertificateSerialRanges.Add(recieverRange);
                        // recieverAddress.CertificateSerialRanges.Add(recieverRange);
                        // 
                        // senderAddress.CertificateSerialRanges = new List<CertificateSerialRange>();
                        if (!string.IsNullOrEmpty(transfer.SenderId))
                        {

                            if (transfer.From > senderRange?.From && transfer.To < senderRange?.To)
                            {
                                var senderRange1 = new CertificateSerialRange();
                                senderRange1.To = transfer.To - 1;
                                senderRange1.From = senderRange.From;
                                senderRange1.AddressId = senderAddress.Id;
                                var senderRange2 = new CertificateSerialRange();
                                senderRange2.From = transfer.From + 1;
                                senderRange2.To = senderRange.To;
                                senderRange2.AddressId = senderAddress.Id;
                                // senderAddress.CertificateSerialRanges = new List<CertificateSerialRange>();
                                _dbContext?.CertificateSerialRanges.Add(senderRange1);
                                _dbContext?.CertificateSerialRanges.Add(senderRange2);
                            }
                            else if (transfer.From == senderRange?.From && transfer.To < senderRange?.To)
                            {
                                var senderRange1 = new CertificateSerialRange();
                                senderRange1.From = transfer.To + 1;
                                senderRange1.To = senderRange.To;
                                senderRange1.AddressId = senderAddress.Id;
                                _dbContext?.CertificateSerialRanges.Add(senderRange1);
                            }
                            else if (transfer.To == senderRange?.To && transfer.From > senderRange?.From)
                            {
                                var senderRange1 = new CertificateSerialRange();
                                senderRange1.To = transfer.To - 1;
                                senderRange1.From = senderRange.From;
                                senderRange1.AddressId = senderAddress.Id;
                                _dbContext?.CertificateSerialRanges.Add(senderRange1);
                            }
                            _dbContext?.CertificateSerialRanges.Remove(senderRange);
                        }
                        else
                        {
                            transfer.Status = true;
                        }

                        await base.UpdateAsync(transfer, tt => tt.Id);
                    //    _dbContext.Addresses.Update(recieverAddress);
                    //     if(senderAddress != null){
                    //         _dbContext.Addresses.Update(senderAddress);
                    //     }
                        
                        await base.SaveChangesAsync(cancellationToken);
                        await transaction.CommitAsync();
                    }
                    catch (System.Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            });
        }

    }
}