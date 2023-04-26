
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
                .HasForeignKey(m => m.AreaTypeLookupId);
            builder.HasOne(m => m.ParentAddress)
                .WithMany(n => n.ChildAddresses)
                .HasForeignKey(m => m.ParentAddressId)
                .IsRequired(false);


        }
    }
}
