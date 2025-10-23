using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccJournalCommentConfig : IEntityTypeConfiguration<AccJournalComment>
    {
        public void Configure(EntityTypeBuilder<AccJournalComment> entity)
        {

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
        }
    }
}
