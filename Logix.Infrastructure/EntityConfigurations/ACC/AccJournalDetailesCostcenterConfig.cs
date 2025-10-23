using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccJournalDetailesCostcenterConfig : IEntityTypeConfiguration<AccJournalDetailesCostcenter>
    {
        public void Configure(EntityTypeBuilder<AccJournalDetailesCostcenter> entity)
        {
            entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");

            entity.Property(e => e.InsertDate).HasDefaultValueSql("(getdate())");
        }
    }
}
