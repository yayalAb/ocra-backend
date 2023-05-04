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
                 .WithOne(n => n.BeforeAdoptionAddressNavigation)
                 .HasForeignKey<AdoptionEvent>(m => m.BeforeAdoptionAddressId);

            builder.HasOne(m => m.AdoptiveMother)
                 .WithOne(n => n.AdoptiveMotherNavigation)
                 .HasForeignKey<AdoptionEvent>(m => m.AdoptiveMotherId);

            builder.HasOne(m => m.AdoptiveFather)
                 .WithOne(n => n.AdoptiveFatherNavigation)
                 .HasForeignKey<AdoptionEvent>(m => m.AdoptiveFatherId);
            builder.HasOne(m => m.CourtCase)
                 .WithOne(n => n.AdoptionEventCourtCase)
                 .HasForeignKey<AdoptionEvent>(m => m.CourtCaseId);

            builder.HasOne(m => m.Event)
            .WithOne(n => n.AdoptionEvent)
            .HasForeignKey<AdoptionEvent>(m => m.EventId);

        }
    }

}
