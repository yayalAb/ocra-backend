
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;

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
                        // Address? recieverAddress = _dbContext.Users.Include(i => i.Address).ThenInclude(a => a.CertificateSerialRanges)
                        //                                 .Where(u => u.Id == transfer.RecieverId)
                        //                                 .FirstOrDefault()?.Address;
                        // Address? senderAddress = (transfer.SenderId != null)
                        //                             ? _dbContext.Users.Include(i => i.Address).ThenInclude(a => a.CertificateSerialRanges)
                        //                                 .Where(u => u.Id == transfer.SenderId)
                        //                                 .FirstOrDefault()?.Address : null;
                        // CertificateSerialRange? senderRange = (recieverAddress != null) ? _dbContext.CertificateSerialRanges.Where(r => r.AddressId == senderAddress.Id)
                        //                                 .Where(r => r.From >= transfer.From)
                        //                                 .Where(r => r.To <= transfer.To)
                        //                                 .FirstOrDefault() : null;
                        // // _dbContext.CertificateSerialRanges.Add(range);
                        // // CertificateSerialRange? recieverRange = (recieverAddress != null) ? _dbContext.CertificateSerialRanges.Where(r => r.AddressId == recieverAddress.Id)
                        // // .Where(r => r.From >= transfer.From)
                        // // .Where(r => r.To <= transfer.To)
                        // // .FirstOrDefault() : null;

                        // var recieverRange = new CertificateSerialRange
                        // {
                        //     From = transfer.From,
                        //     To = transfer.To,
                        //     AddressId = recieverAddress.Id
                        // };
                        // recieverAddress.CertificateSerialRanges.Add(recieverRange);
                        // // 
                        // // senderAddress.CertificateSerialRanges = new List<CertificateSerialRange>();
                        // if (!string.IsNullOrEmpty(transfer.SenderId))
                        // {

                        //     if (senderRange?.From < transfer.From && senderRange?.To > transfer.To)
                        //     {
                        //         var senderRange1 = senderRange;
                        //         senderRange1.To = transfer.To - 1;
                        //         var senderRange2 = senderRange;
                        //         senderRange2.From = transfer.From + 1;
                        //         // senderAddress.CertificateSerialRanges = new List<CertificateSerialRange>();
                        //         senderAddress?.CertificateSerialRanges.Add(senderRange1);
                        //         senderAddress.CertificateSerialRanges.Add(senderRange2);
                        //     }
                        //     else if (senderRange.From >= transfer.From)
                        //     {
                        //         var senderRange1 = senderRange;
                        //         senderRange1.From = transfer.To + 1;
                        //         senderAddress.CertificateSerialRanges.Add(senderRange1);
                        //     }
                        //     else if (senderRange.To >= transfer.To)
                        //     {
                        //         var senderRange1 = senderRange;
                        //         senderRange1.To = transfer.To - 1;
                        //         senderAddress.CertificateSerialRanges.Add(senderRange1);
                        //     }
                        // }
                        // else
                        // {
                        //     transfer.Status = true;
                        // }
                        await base.InsertAsync(transfer, cancellationToken);
                        await base.SaveChangesAsync(cancellationToken);
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