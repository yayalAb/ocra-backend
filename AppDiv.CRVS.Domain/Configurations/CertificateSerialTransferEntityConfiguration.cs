using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configurations
{
    public class CertificateSerialTransferEntityConfiguration : IEntityTypeConfiguration<CertificateSerialTransfer>
    {
        public void Configure(EntityTypeBuilder<CertificateSerialTransfer> builder)
        {
            builder.HasOne(m => m.Sender)
               .WithMany(n => n.SenderCertificateSerialTransfers)
               .HasForeignKey(m => m.SenderId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(m => m.Reciever)
               .WithMany(n => n.RecieverCertificateSerialTransfers)
               .HasForeignKey(m => m.RecieverId)
               .OnDelete(DeleteBehavior.Restrict);


        }

    }
}
