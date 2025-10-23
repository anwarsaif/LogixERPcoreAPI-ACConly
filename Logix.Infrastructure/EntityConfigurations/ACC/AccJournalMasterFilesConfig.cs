using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccJournalMasterFilesConfig : IEntityTypeConfiguration<AccJournalMasterFile>
    {
        public void Configure(EntityTypeBuilder<AccJournalMasterFile> entity)
        {
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
        }
    }

}
