using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDiv.CRVS.Domain.Configurations
{
    public class TransactionEntityConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasOne(m => m.CivilRegOfficer)
               .WithMany(n => n.CivilRegOfficerTransactions)
               .HasForeignKey(m => m.CivilRegOfficerId);

            builder.HasOne(m => m.Workflow)
                .WithMany(n => n.Transactions)
                .HasForeignKey(m => m.WorkflowId);
            builder.HasOne(m => m.Request)
                .WithMany(n => n.Transactions)
                .HasForeignKey(m => m.RequestId);


        }

    }
}
