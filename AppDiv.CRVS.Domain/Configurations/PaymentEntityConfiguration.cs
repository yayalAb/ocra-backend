
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configuration
{
    public class PaymentEntityConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasOne(m =>m.PaymentRequest)
            .WithOne(n => n.Payment )
            .HasForeignKey<Payment>(m => m.PaymentRequestId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.PaymentWayLookup)
            .WithMany(n => n.PaymentNavigation)
            .HasForeignKey(m => m.PaymentWayLookupId)
             .OnDelete(DeleteBehavior.Restrict);

            


        }
    }
}
