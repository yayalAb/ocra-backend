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
            builder.HasOne(m => m.CauseOfDeathInfoType)
               .WithMany(n => n.CauseOfDeathInfoTypeNavigation)
               .HasForeignKey(m => m.CauseOfDeathInfoTypeId);
        }
    }
}