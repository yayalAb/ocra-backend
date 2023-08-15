using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configurations
{
    public class DeathNotificationEntityConfiguration : IEntityTypeConfiguration<DeathNotification>
    {
        public void Configure(EntityTypeBuilder<DeathNotification> builder)
        {
            builder.HasOne(m => m.CauseOfDeathInfoTypeLookup)
               .WithMany(n => n.CauseOfDeathInfoTypeNavigation)
               .HasForeignKey(m => m.CauseOfDeathInfoTypeLookupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.DeathEvent)
               .WithOne(n => n.DeathNotification)
               .HasForeignKey<DeathNotification>(m => m.DeathEventId)
               .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}