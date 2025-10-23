using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccJournalMasterVwConfig : IEntityTypeConfiguration<AccJournalMasterVw>
    {
        public void Configure(EntityTypeBuilder<AccJournalMasterVw> entity)
        {
            entity.ToView("ACC_Journal_Master_VW");

            entity.Property(e => e.ChequDateHijri).IsFixedLength();

            entity.Property(e => e.JDateGregorian).IsFixedLength();

            entity.Property(e => e.JDateHijri).IsFixedLength();
        }
    }

}
