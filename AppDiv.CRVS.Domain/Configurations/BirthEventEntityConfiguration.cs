using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AppDiv.CRVS.Domain.Configurations
{
    public class BirthEventEntityConfiguration : IEntityTypeConfiguration<BirthEvent>
    {
        public void Configure(EntityTypeBuilder<BirthEvent> builder)
        {

            builder.HasOne(m => m.FacilityLookup)
                 .WithMany(n => n.BirthFacilityNavigation)
                 .HasForeignKey(m => m.FacilityLookupId)
                 .IsRequired(false);

            builder.HasOne(m => m.FacilityTypeLookup)
                .WithMany(n => n.BirthFacilityTypeNavigation)
                .HasForeignKey(m => m.FacilityTypeLookupId)
                .IsRequired(false);

            builder.HasOne(m => m.Father)
                .WithMany(n => n.BirthFatherNavigation)
                .HasForeignKey(m => m.FatherId);

            builder.HasOne(m => m.Mother)
               .WithMany(n => n.BirthMotherNavigation)
               .HasForeignKey(m => m.MotherId);

            builder.HasOne(m => m.BirthPlace)
                .WithMany(n => n.BirthPlaceOfBirthNavigation)
                .HasForeignKey(m => m.BirthPlaceId)
                .IsRequired(false);

            builder.HasOne(m => m.TypeOfBirthLookup)
                .WithMany(n => n.BirthTypeOfBirthNavigation)
                .HasForeignKey(m => m.TypeOfBirthLookupId)
                .IsRequired(false);

            builder.HasOne(m => m.Event)
                .WithOne(n => n.BirthEvent)
                .HasForeignKey<BirthEvent>(m => m.EventId);

        }
    }
}