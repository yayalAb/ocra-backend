using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configurations
{
    public class DivorceEventEntityConfiguration : IEntityTypeConfiguration<DivorceEvent>
    {
        public void Configure(EntityTypeBuilder<DivorceEvent> builder)
        {

            builder.HasOne(m => m.DivorcedWife)
                .WithMany(n => n.DivorceWifeNavigation)
                .HasForeignKey(m => m.DivorcedWifeId);
            builder.HasOne(m => m.CourtCase)
               .WithOne(n => n.DivorceEventCourtCase)
               .HasForeignKey<DivorceEvent>(m => m.CourtCaseId);

            builder.HasOne(m => m.Event)
                .WithOne(n => n.DivorceEvent)
                .HasForeignKey<DivorceEvent>(m => m.EventId);

        }
    }

}