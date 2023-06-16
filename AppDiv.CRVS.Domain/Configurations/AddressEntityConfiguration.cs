
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configuration
{
    public class AddressEntityConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasOne(m => m.AreaTypeLookup)
                .WithMany(n => n.AddressAreaTypeNavigation)
                .HasForeignKey(m => m.AreaTypeLookupId)
                .IsRequired(false);

            builder.HasOne(m => m.AdminTypeLookup)
                .WithMany(n => n.AdminTypeNavigation)
                .HasForeignKey(m => m.AdminTypeLookupId)
                .IsRequired(false);
            builder.HasOne(m => m.ParentAddress)
                .WithMany(n => n.ChildAddresses)
                .HasForeignKey(m => m.ParentAddressId)
                .IsRequired(false);
            builder.HasMany(m => m.CertificateSerialRanges)
                .WithOne(n => n.Address)
                .HasForeignKey(n => n.AddressId);
        }
    }
}
