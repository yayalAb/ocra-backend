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
               .HasForeignKey(m => m.CivilRegOfficerId);

            builder.HasOne(m => m.CorrectionRequest)
               .WithOne(n => n.Request)
               .HasForeignKey<CorrectionRequest>(n => n.RequestId);
               
            builder.HasOne(m => m.AuthenticationRequest)
                .WithOne(n => n.Request)
                .HasForeignKey<AuthenticationRequest>(n => n.RequestId);


        }

    }
}
