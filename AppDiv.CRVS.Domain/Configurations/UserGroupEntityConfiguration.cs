using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configurations
{
    public class UserGroupEntityConfiguration : IEntityTypeConfiguration<UserGroup>
    {
        public void Configure(EntityTypeBuilder<UserGroup> builder)
        {
            builder.HasMany(m => m.ApplicationUsers)
               .WithMany(n => n.UserGroups);

            builder.HasMany(m => m.Steps)
               .WithOne(n => n.UserGroup).HasForeignKey(n => n.UserGroupId);
            builder.HasMany(m => m.Notifications)
                .WithOne(n => n.UserGroup)
                .HasForeignKey(n => n.GroupId);
        }

    }
}
