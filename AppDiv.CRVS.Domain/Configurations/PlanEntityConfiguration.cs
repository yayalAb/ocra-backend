
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configuration
{
    public class PlanEntityConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.HasOne(p => p.Address)
            .WithMany(a => a.AddressPlans)
            .HasForeignKey(p => p.AddressId)
            .IsRequired(true);

            builder.HasOne(p => p.PlannedBy)
            .WithMany(a => a.UserPlans)
            .HasForeignKey(p => p.PlannedById)
            .IsRequired(true);

            builder.HasOne(m => m.ParentPlan)
                .WithMany(n => n.ChildPlans)
                .HasForeignKey(m => m.ParentPlanId)
                .IsRequired(false);


        }
    }
}
