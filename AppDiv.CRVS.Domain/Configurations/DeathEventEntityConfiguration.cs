using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Domain.Configurations
{
    public class DeathEventEntityConfiguration : IEntityTypeConfiguration<DeathEvent>
    {
        public void Configure(EntityTypeBuilder<DeathEvent> builder)
        {
            builder.HasOne(m => m.FacilityType)
               .WithMany(n => n.DeathFacilityTypeNavigation)
               .HasForeignKey(m => m.FacilityTypeId);

            builder.HasOne(m => m.Facility)
               .WithMany(n => n.DeathFacilityNavigation)
               .HasForeignKey(m => m.FacilityId);

            builder.HasOne(d => d.Event)
            .WithOne(n => n.DeathEventNavigation)
            .HasForeignKey<DeathEvent>(m => m.EventId);


            builder.HasOne(d => d.DeathNotification)
            .WithOne(n => n.DeathEvent)
            .HasForeignKey<DeathNotification>(m => m.DeathEventId);

        }
    }
}