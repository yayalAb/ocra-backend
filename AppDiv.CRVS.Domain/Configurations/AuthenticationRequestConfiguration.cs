using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configurations
{
    public class AuthenticationRequestConfiguration : IEntityTypeConfiguration<AuthenticationRequest>
    {
        public void Configure(EntityTypeBuilder<AuthenticationRequest> builder)
        {
            builder.HasOne(m => m.Request)
                    .WithOne(n => n.AuthenticationRequest)
                    .HasForeignKey<AuthenticationRequest>(n => n.RequestId);


        }

    }
}

