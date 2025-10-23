using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccSettlementScheduleConfig : IEntityTypeConfiguration<AccSettlementSchedule>
    {
        public void Configure(EntityTypeBuilder<AccSettlementSchedule> entity)
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
        }
    }
}
