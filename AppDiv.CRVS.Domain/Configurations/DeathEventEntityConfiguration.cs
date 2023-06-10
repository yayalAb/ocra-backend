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
            builder.HasOne(m => m.FacilityTypeLookup)
               .WithMany(n => n.DeathFacilityTypeNavigation)
               .HasForeignKey(m => m.FacilityTypeLookupId);

            builder.HasOne(m => m.FacilityLookup)
               .WithMany(n => n.DeathFacilityNavigation)
               .HasForeignKey(m => m.FacilityLookupId);

            builder.HasOne(m => m.DuringDeathLookup)
               .WithMany(n => n.DuringDeathNavigation)
               .HasForeignKey(m => m.DuringDeathId)
               .IsRequired(false);

            builder.HasOne(m => m.DeathPlace)
                .WithMany(n => n.DeathPlaceOfDeathNavigation)
                .HasForeignKey(m => m.DeathPlaceId);

            builder.HasOne(d => d.Event)
            .WithOne(n => n.DeathEventNavigation)
            .HasForeignKey<DeathEvent>(m => m.EventId);

            builder.HasOne(d => d.DeathNotification)
            .WithOne(n => n.DeathEvent)
            .HasForeignKey<DeathNotification>(m => m.DeathEventId)
            .IsRequired(false);

        }
    }
}