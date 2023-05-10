using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configurations
{
    public class PaymentExamptionEntityConfiguration : IEntityTypeConfiguration<PaymentExamption>
    {
        public void Configure(EntityTypeBuilder<PaymentExamption> builder)
        {
            builder.HasOne(m => m.ExamptionRequest)
               .WithMany(n => n.ExamptionRequestNavigation)
               .HasForeignKey(m => m.ExamptionRequestId);

        }
    }


}
