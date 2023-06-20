
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configuration
{
    public class MessageEntityConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasOne(n => n.Sender)
                .WithMany(m => m.SentMessages )
                .HasForeignKey(n => n.SenderId);
            builder.HasOne(n => n.Receiver)
                .WithMany(m => m.ReceivedMessages )
                .HasForeignKey(n => n.ReceiverId);
            builder.HasOne(n => n.ParentMessage)
                .WithMany(m => m.ChildMessages )
                .HasForeignKey(n => n.ParentMessageId)
                .IsRequired(false);
            
        }
    }
}
