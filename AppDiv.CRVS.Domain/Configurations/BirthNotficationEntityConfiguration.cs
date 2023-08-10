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
               .HasForeignKey(m => m.DeliveryTypeLookupId)
               .OnDelete(DeleteBehavior.Restrict);

            // builder.HasOne(m => m.BirthEvent)
            //    .WithOne(n => n.BirthNotification)
            //    .HasForeignKey<BirthNotification>(m => m.BirthEventId)
            //    .IsRequired(false);

        }
    }
}