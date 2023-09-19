
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configurations
{
    public class CertificateEntityConfiguration : IEntityTypeConfiguration<Certificate>
    {
        public void Configure(EntityTypeBuilder<Certificate> builder)
        {
            builder.HasOne(m => m.Event)
                .WithMany(n => n.EventCertificates)
                .HasForeignKey(m => m.EventId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(m => m.AuthenticationRequests)
                .WithOne(n => n.Certificate)
                .HasForeignKey(n => n.CertificateId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}