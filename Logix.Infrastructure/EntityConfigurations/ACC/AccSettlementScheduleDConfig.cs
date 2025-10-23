using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccSettlementScheduleDConfig : IEntityTypeConfiguration<AccSettlementScheduleD>
    {
        public void Configure(EntityTypeBuilder<AccSettlementScheduleD> entity)
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

        }
    }
}
