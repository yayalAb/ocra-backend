
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
            .HasForeignKey(m => m.ApplicationAddressId);
            //with personalInfo table
            builder.HasOne(m =>m.BrideInfo)
            .WithOne(n => n.MarriageApplicationBrideInfo )
            .HasForeignKey<MarriageApplication>(m => m.BrideInfoId);

            //with personalInfo table
            builder.HasOne(m =>m.GroomInfo)
            .WithOne(n => n.MarriageApplicationGroomInfo )
            .HasForeignKey<MarriageApplication>(m => m.GroomInfoId); 

            //with personalInfo table
            builder.HasOne(m =>m.CivilRegOfficer)
            .WithOne(n => n.MarriageApplicationCivilRegOfficer )
            .HasForeignKey<MarriageApplication>(m => m.CivilRegOfficerId);





        }
    }
}
