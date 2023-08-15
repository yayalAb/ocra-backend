using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configurations
{
    public class EventEntityConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasOne(m => m.CivilRegOfficer)
               .WithMany(n => n.EventCivilRegOfficers)
               .HasForeignKey(m => m.CivilRegOfficerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.EventOwener)
                .WithMany(n => n.Events)
                .HasForeignKey(m => m.EventOwenerId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.EventAddress)
               .WithMany(n => n.EventAddresses)
               .HasForeignKey(m => m.EventAddressId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(m => m.PaymentExamption)
            .WithOne(n => n.Event)
            .HasForeignKey<PaymentExamption>(n => n.EventId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.NewEventDuplicatesNavigation)
            .WithOne(m => m.NewEvent)
            .HasForeignKey(m => m.NewEventId)
            .IsRequired(false)
             .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(m => m.OldEventDuplicatesNavigation)
            .WithOne(m => m.OldEvent)
            .HasForeignKey(m => m.OldEventId)
            .IsRequired(false)
             .OnDelete(DeleteBehavior.Restrict);


        }
    }


}
