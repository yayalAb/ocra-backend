using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configurations
{
    public class AdoptionEventEntityConfiguration : IEntityTypeConfiguration<AdoptionEvent>
    {
        public void Configure(EntityTypeBuilder<AdoptionEvent> builder)
        {

            builder.HasOne(m => m.BeforeAdoptionAddress)
                 .WithMany(n => n.BeforeAdoptionAddressNavigation)
                 .HasForeignKey(m => m.BeforeAdoptionAddressId);

            builder.HasOne(m => m.AdoptiveMother)
                 .WithMany(n => n.AdoptiveMotherNavigation)
                 .HasForeignKey(m => m.AdoptiveMotherId);

            builder.HasOne(m => m.AdoptiveFather)
                 .WithMany(n => n.AdoptiveFatherNavigation)
                 .HasForeignKey(m => m.AdoptiveFatherId);
            builder.HasOne(m => m.CourtCase)
                 .WithOne(n => n.AdoptionEventCourtCase)
                 .HasForeignKey<AdoptionEvent>(m => m.CourtCaseId);

            builder.HasOne(m => m.Event)
            .WithOne(n => n.AdoptionEvent)
            .HasForeignKey<AdoptionEvent>(m => m.EventId);

        }
    }

}
