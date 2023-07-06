
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configuration
{
    public class WorkHistoryEntityConfiguration : IEntityTypeConfiguration<WorkHistory>
    {
        public void Configure(EntityTypeBuilder<WorkHistory> builder)
        {
            builder.HasMany(m => m.UserGroups)
               .WithMany(m => m.WorkHistories);
            builder.HasOne(h => h.Address)
            .WithMany(a => a.WorkHistories)
            .HasForeignKey(h => h.AddressId);
            // builder.HasOne(h => h.User)
            // .WithMany(u => u.WorkerHistories)
            // .HasForeignKey(h => h.UserId);
        }
    }
}
