using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccJournalMasterConfig : IEntityTypeConfiguration<AccJournalMaster>
    {
        public void Configure(EntityTypeBuilder<AccJournalMaster> entity)
        {
            entity.Property(e => e.Amount).HasDefaultValueSql("((0))");

            entity.Property(e => e.BankId).HasDefaultValueSql("((0))");

            entity.Property(e => e.ChequDateHijri).IsFixedLength();

            entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");

            entity.Property(e => e.JDateGregorian).IsFixedLength();

            entity.Property(e => e.JDateHijri).IsFixedLength();

            entity.Property(e => e.PaymentTypeId).HasDefaultValueSql("((0))");

            entity.Property(e => e.ReferenceMappingId).HasDefaultValueSql("((0))");
            
            //entity.Property(b => b.IsDeleted)
            //.HasColumnName("FlagDelete")
            //.HasColumnType("bit")
            //.IsRequired(false);
        }
    }

}
