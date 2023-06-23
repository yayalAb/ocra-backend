
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
                        Address? recieverAddress = _dbContext.Users.Include(i => i.Address)
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
                        // Get the reciever address from the user id.
                        Address? recieverAddress = _dbContext.Users.Include(i => i.Address).ThenInclude(a => a.CertificateSerialRanges)
                                                        .Where(u => u.Id == transfer.RecieverId)
                                                        .FirstOrDefault()?.Address;
                        // Get the sender address from the user id.
                        Address? senderAddress = (transfer.SenderId != null)
                                                    ? _dbContext.Users.Include(i => i.Address).ThenInclude(a => a.CertificateSerialRanges)
                                                        .Where(u => u.Id == transfer.SenderId)
                                                        .FirstOrDefault()?.Address : null;
                        // Get the sender serial number range between the given numbers.
                        var senderRange = senderAddress?.CertificateSerialRanges?
                                                        .Where(r => r.From.CompareTo(transfer.From) <= 0)
                                                        .Where(r => r.To.CompareTo(transfer.To) >= 0)
                                                        .FirstOrDefault();

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

                            if (transfer.From.CompareTo(senderRange?.From) > 0 && transfer.To.CompareTo(senderRange?.To) < 0)
                            {
                                var senderRange1 = new CertificateSerialRange();
                                senderRange1.To = substractOneFrom(transfer.To);
                                senderRange1.From = senderRange.From;
                                senderRange1.AddressId = senderAddress.Id;
                                var senderRange2 = new CertificateSerialRange();
                                senderRange2.From = addOneTo(transfer.From);
                                senderRange2.To = senderRange.To;
                                senderRange2.AddressId = senderAddress.Id;
                                // senderAddress.CertificateSerialRanges = new List<CertificateSerialRange>();
                                _dbContext?.CertificateSerialRanges.Add(senderRange1);
                                _dbContext?.CertificateSerialRanges.Add(senderRange2);
                            }
                            else if (transfer.From == senderRange?.From && transfer.To.CompareTo(senderRange?.To) < 0)
                            {
                                var senderRange1 = new CertificateSerialRange();
                                senderRange1.From = addOneTo(transfer.To);
                                senderRange1.To = senderRange.To;
                                senderRange1.AddressId = senderAddress.Id;
                                _dbContext?.CertificateSerialRanges.Add(senderRange1);
                            }
                            else if (transfer.To == senderRange?.To && transfer.From.CompareTo(senderRange?.From) > 0)
                            {
                                var senderRange1 = new CertificateSerialRange();
                                senderRange1.To = substractOneFrom(transfer.To);
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

        private string substractOneFrom(string serialNumber)
        {
            int numIndex = -1;

            // loop through each character in the string to find the index where the numeric part starts
            for (int i = 0; i < serialNumber.Length; i++)
            {
                if (char.IsDigit(serialNumber[i]))
                {
                    numIndex = i;
                    break;
                }
            }

            if (numIndex != -1)
            {
                string alphabets = serialNumber.Substring(0, numIndex); // extract the alphabetic part
                string numbers = serialNumber.Substring(numIndex); // extract the numeric part

                int num = int.Parse(numbers); // convert the numeric part to integer
                num--; // decrement the integer

                string newNumbers = num.ToString().PadLeft(numbers.Length, '0'); // convert the integer back to string and pad with leading zeros if necessary

                string newStr = alphabets + newNumbers; // concatenate the alphabetic and numeric parts

                return newStr; // return the new string
            }
            else
            {
                return ("Numeric part not found in the string.");
            }
        }
        private string addOneTo(string serialNumber)
        {
            int numIndex = -1;

            // loop through each character in the string to find the index where the numeric part starts
            for (int i = 0; i < serialNumber.Length; i++)
            {
                if (char.IsDigit(serialNumber[i]))
                {
                    numIndex = i;
                    break;
                }
            }

            if (numIndex != -1)
            {
                string alphabets = serialNumber.Substring(0, numIndex); // extract the alphabetic part
                string numbers = serialNumber.Substring(numIndex); // extract the numeric part

                int num = int.Parse(numbers); // convert the numeric part to integer
                num++; // decrement the integer

                string newNumbers = num.ToString().PadLeft(numbers.Length, '0'); // convert the integer back to string and pad with leading zeros if necessary

                string newStr = alphabets + newNumbers; // concatenate the alphabetic and numeric parts

                return newStr; // return the new string
            }
            else
            {
                return ("Numeric part not found in the string.");
            }
        }

    }
}