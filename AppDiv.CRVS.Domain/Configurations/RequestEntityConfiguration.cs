using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configurations
{
    public class RequestEntityConfiguration : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            builder.HasOne(m => m.CivilRegOfficer)
               .WithMany(n => n.CivilRegOfficerRequests)
               .HasForeignKey(m => m.CivilRegOfficerId)
               .OnDelete(DeleteBehavior.Restrict);

            // builder.HasOne(m => m.CorrectionRequest)
            //    .WithOne(n => n.Request)
            //    .HasForeignKey<CorrectionRequest>(n => n.RequestId);

            // builder.HasOne(m => m.AuthenticationRequest)
            //     .WithOne(n => n.Request)
            //     .HasForeignKey<AuthenticationRequest>(n => n.RequestId);

            builder.HasOne(m => m.Workflow)
               .WithMany(n => n.Requests)
               .HasForeignKey(n => n.WorkflowId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(m => m.Notification)
                .WithOne(n => n.Request)
                .HasForeignKey<Notification>(n => n.RequestId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(m => m.ProfileChangeRequest)
                    .WithOne(n => n.Request)
                    .HasForeignKey<ProfileChangeRequest>(n => n.RequestId)
                    .OnDelete(DeleteBehavior.Cascade);

        }

    }
}
