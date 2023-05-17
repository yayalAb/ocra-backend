using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configurations
{
    public class PaymentExamptionRequestEntityConfiguration : IEntityTypeConfiguration<PaymentExamptionRequest>
    {
        public void Configure(EntityTypeBuilder<PaymentExamptionRequest> builder)
        {
            builder.HasOne(m => m.Address)
               .WithMany(n => n.ExamptionRequestAddresses)
               .HasForeignKey(m => m.AddressId);
        }
    }


}
