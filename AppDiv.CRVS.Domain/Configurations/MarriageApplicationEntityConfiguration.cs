
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configuration
{
    public class MarriageApplicationEntityConfiguration : IEntityTypeConfiguration<MarriageApplication>
    {
        public void Configure(EntityTypeBuilder<MarriageApplication> builder)
        {
            builder.HasOne(m =>m.ApplicationAddress)
            .WithMany(n => n.MarriageApplications )
            .HasForeignKey(m => m.ApplicationAddressId)
            .IsRequired(false)
             .OnDelete(DeleteBehavior.Restrict);
            //with personalInfo table
            builder.HasOne(m =>m.BrideInfo)
            .WithMany(n => n.MarriageApplicationBrideInfo )
            .HasForeignKey(m => m.BrideInfoId)
             .OnDelete(DeleteBehavior.Restrict);

            //with personalInfo table
            builder.HasOne(m =>m.GroomInfo)
            .WithMany(n => n.MarriageApplicationGroomInfo )
            .HasForeignKey(m => m.GroomInfoId)
             .OnDelete(DeleteBehavior.Restrict); 

            //with personalInfo table
            builder.HasOne(m =>m.CivilRegOfficer)
            .WithMany(n => n.MarriageApplicationCivilRegOfficer )
            .HasForeignKey(m => m.CivilRegOfficerId)
             .OnDelete(DeleteBehavior.Restrict);





        }
    }
}
