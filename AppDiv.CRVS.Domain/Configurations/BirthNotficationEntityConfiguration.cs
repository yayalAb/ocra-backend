using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configurations
{
    public class BirthNotficationEntityConfiguration : IEntityTypeConfiguration<BirthNotification>
    {
        public void Configure(EntityTypeBuilder<BirthNotification> builder)
        {
            builder.HasOne(m => m.DeliveryTypeLookup)
               .WithMany(n => n.DeliveryTypeNavigation)
               .HasForeignKey(m => m.DeliveryTypeLookupId);
        }
    }
}