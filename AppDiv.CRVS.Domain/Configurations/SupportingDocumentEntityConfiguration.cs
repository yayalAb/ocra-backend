using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configurations
{
    public class SupportingDocumentEntityConfiguration : IEntityTypeConfiguration<SupportingDocument>
    {
        public void Configure(EntityTypeBuilder<SupportingDocument> builder)
        {
            builder.HasOne(m => m.Event)
                .WithMany(n => n.EventSupportingDocuments)
                .HasForeignKey(m => m.EventId);
        }
    }
}