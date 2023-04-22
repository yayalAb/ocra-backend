
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configuration
{
    public class ContactInfoEntityConfiguration : IEntityTypeConfiguration<ContactInfo>
    {
        public void Configure(EntityTypeBuilder<ContactInfo> builder)
        {
           
            builder.HasOne(m => m.PersonalInfo)
            .WithOne(n => n.ContactInfo)
            .HasForeignKey<ContactInfo>(m => m.PersonalInfoId);
        }           
    }
}
