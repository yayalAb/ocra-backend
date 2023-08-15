
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configuration
{
    public class MarriageEventEntityConfiguration : IEntityTypeConfiguration<MarriageEvent>
    {
        public void Configure(EntityTypeBuilder<MarriageEvent> builder)
        {
            builder.HasOne(m =>m.Event)
            .WithOne(n => n.MarriageEvent )
            .HasForeignKey<MarriageEvent>(m => m.EventId)
            .OnDelete(DeleteBehavior.Cascade)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Application)
            .WithOne(n => n.MarriageEvent)
            .HasForeignKey<MarriageEvent>(n => n.ApplicationId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.BrideInfo)
            .WithMany(n => n.MarriageEventBrideInfo)
            .HasForeignKey(n => n.BrideInfoId)
            .OnDelete(DeleteBehavior.Restrict)
             .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
