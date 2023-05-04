
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configuration
{
    public class PersonalInfoEntityConfiguration : IEntityTypeConfiguration<PersonalInfo>
    {
        public void Configure(EntityTypeBuilder<PersonalInfo> builder)
        {

            builder.HasOne(m => m.Address)
               .WithMany(n => n.PersonalInfos)
               .HasForeignKey(m => m.AddressId);
            builder.HasOne(m => m.SexLookup)
               .WithMany(n => n.PersonSexNavigation)
               .HasForeignKey(m => m.SexLookupId);
            builder.HasOne(m => m.PlaceOfBirthLookup)
               .WithMany(n => n.PersonPlaceOfBirthNavigation)
               .HasForeignKey(m => m.PlaceOfBirthLookupId)
               .IsRequired(false);
            builder.HasOne(m => m.NationalityLookup)
               .WithMany(n => n.PersonNationalityNavigation)
               .HasForeignKey(m => m.NationalityLookupId);
            builder.HasOne(m => m.TitleLookup)
               .WithMany(n => n.PersonTitleNavigation)
               .HasForeignKey(m => m.TitleLookupId)
               .IsRequired(false);
            builder.HasOne(m => m.ReligionLookup)
               .WithMany(n => n.PersonReligionNavigation)
               .HasForeignKey(m => m.ReligionLookupId)
               .IsRequired(false);
            builder.HasOne(m => m.EducationalStatusLookup)
               .WithMany(n => n.PersonEducationalStatusNavigation)
               .HasForeignKey(m => m.EducationalStatusLookupId)
               .IsRequired(false);
            builder.HasOne(m => m.TypeOfWorkLookup)
               .WithMany(n => n.PersonTypeOfWorkNavigation)
               .HasForeignKey(m => m.TypeOfWorkLookupId)
               .IsRequired(false);
            builder.HasOne(m => m.MarraigeStatusLookup)
               .WithMany(n => n.PersonMarriageStatusNavigation)
               .HasForeignKey(m => m.MarriageStatusLookupId)
               .IsRequired(false);
            builder.HasOne(m => m.NationLookup)
               .WithMany(n => n.PersonNationNavigation)
               .HasForeignKey(m => m.NationLookupId)
               .IsRequired(false);
            //    builder.HasOne(m => m.ContactInfo)
            //   .WithOne(n => n.PersonalInfo)
            //   .HasForeignKey<PersonalInfo>(m => m.ContactInfoId)
            //   ;


        }
    }
}