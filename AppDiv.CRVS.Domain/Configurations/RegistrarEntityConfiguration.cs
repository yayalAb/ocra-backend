using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configurations
{
    public class RegistrarEntityConfiguration : IEntityTypeConfiguration<Registrar>
    {
        public void Configure(EntityTypeBuilder<Registrar> builder)
        {
            builder.HasOne(m => m.RegistrarInfo)
               .WithMany(n => n.RegistrarPersonalInfoNavigation)
               .HasForeignKey(m => m.RegistrarInfoId)
               .OnDelete(DeleteBehavior.Restrict);

        }
    }


}
