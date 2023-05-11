
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configuration
{
    public class WitnessEntityConfiguration : IEntityTypeConfiguration<Witness>
    {
        public void Configure(EntityTypeBuilder<Witness> builder)
        {
            builder.HasOne(m =>m.MarriageEvent)
            .WithMany(n => n.Witnesses )
            .HasForeignKey(m => m.MarriageEventId);

            builder.HasOne(m => m.WitnessPersonalInfo)
            .WithMany(n => n.Witness)
            .HasForeignKey(m => m.WitnessPersonalInfoId);

            


        }
    }
}
