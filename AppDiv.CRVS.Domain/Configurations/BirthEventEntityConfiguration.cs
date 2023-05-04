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

            builder.HasOne(m => m.Facility)
                 .WithMany(n => n.BirthFacilityNavigation)
                 .HasForeignKey(m => m.FacilityId);
            builder.HasOne(m => m.FacilityType)
                .WithMany(n => n.BirthFacilityTypeNavigation)
                .HasForeignKey(m => m.FacilityTypeId);

            builder.HasOne(m => m.Father)
                .WithOne(n => n.BirthFatherNavigation)
                .HasForeignKey<BirthEvent>(m => m.FatherId);

            builder.HasOne(m => m.Mother)
               .WithOne(n => n.BirthMotherNavigation)
               .HasForeignKey<BirthEvent>(m => m.MotherId);
        }
    }
}