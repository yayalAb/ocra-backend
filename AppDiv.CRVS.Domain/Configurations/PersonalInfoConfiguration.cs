
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

            builder.HasOne(m => m.BirthAddress)
               .WithMany(n => n.PersonalInfoBirthAddresses)
               .HasForeignKey(m => m.BirthAddressId)
               .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(m => m.ResidentAddress)
               .WithMany(n => n.PersonalInfoResidentAddresses)
               .HasForeignKey(m => m.ResidentAddressId)
               .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(m => m.SexLookup)
               .WithMany(n => n.PersonSexNavigation)
               .HasForeignKey(m => m.SexLookupId)
               .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(m => m.PlaceOfBirthLookup)
               .WithMany(n => n.PersonPlaceOfBirthNavigation)
               .HasForeignKey(m => m.PlaceOfBirthLookupId)
               .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(m => m.NationalityLookup)
               .WithMany(n => n.PersonNationalityNavigation)
               .HasForeignKey(m => m.NationalityLookupId)
               .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(m => m.TitleLookup)
               .WithMany(n => n.PersonTitleNavigation)
               .HasForeignKey(m => m.TitleLookupId)
               .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(m => m.ReligionLookup)
               .WithMany(n => n.PersonReligionNavigation)
               .HasForeignKey(m => m.ReligionLookupId)
               .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(m => m.EducationalStatusLookup)
               .WithMany(n => n.PersonEducationalStatusNavigation)
               .HasForeignKey(m => m.EducationalStatusLookupId)
               .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(m => m.TypeOfWorkLookup)
               .WithMany(n => n.PersonTypeOfWorkNavigation)
               .HasForeignKey(m => m.TypeOfWorkLookupId)
               .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(m => m.MarraigeStatusLookup)
               .WithMany(n => n.PersonMarriageStatusNavigation)
               .HasForeignKey(m => m.MarriageStatusLookupId)
               .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(m => m.NationLookup)
               .WithMany(n => n.PersonNationNavigation)
               .HasForeignKey(m => m.NationLookupId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);
            //    builder.HasOne(m => m.ContactInfo)
            //   .WithOne(n => n.PersonalInfo)
            //   .HasForeignKey<PersonalInfo>(m => m.ContactInfoId)
            //   ;

            builder.HasMany(m => m.NewPersonDuplicatesNavigation)
            .WithOne(m => m.NewPerson)
            .HasForeignKey(m => m.NewPersonId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
            


            builder.HasMany(m => m.OldPersonDuplicatesNavigation)
            .WithOne(m => m.OldPerson)
            .HasForeignKey(m => m.OldPersonId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);



        }
    }
}