
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configuration
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasMany(m => m.UserGroups)
               .WithMany(m => m.ApplicationUsers);
            builder.HasOne(m => m.Address)
            .WithMany(n => n.ApplicationuserAddresses)
            .HasForeignKey(m => m.AddressId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
