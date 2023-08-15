
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configuration
{
    public class PaymentRequestEntityConfiguration : IEntityTypeConfiguration<PaymentRequest>
    {
        public void Configure(EntityTypeBuilder<PaymentRequest> builder)
        {
            builder.HasOne(m => m.PaymentRate)
            .WithMany(n => n.PaymentRatePaymentRequests)
            .HasForeignKey(m => m.PaymentRateId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
