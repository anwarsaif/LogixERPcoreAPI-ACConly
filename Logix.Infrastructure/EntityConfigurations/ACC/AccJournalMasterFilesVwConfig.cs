using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccJournalMasterFilesVwConfig : IEntityTypeConfiguration<AccJournalMasterFilesVw>
    {
        public void Configure(EntityTypeBuilder<AccJournalMasterFilesVw> entity)
        {
            entity.ToView("ACC_Journal_Master_Files_VW");

            entity.Property(e => e.JDateGregorian).IsFixedLength();

            entity.Property(e => e.JDateHijri).IsFixedLength();
        }
    }

}
