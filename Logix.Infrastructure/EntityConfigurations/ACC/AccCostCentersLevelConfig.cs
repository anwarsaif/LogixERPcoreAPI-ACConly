using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccCostCentersLevelConfig : IEntityTypeConfiguration<AccCostCentersLevel>
    {
        public void Configure(EntityTypeBuilder<AccCostCentersLevel> entity)
        {
            entity.Property(e => e.LevelId).ValueGeneratedNever();

        }
    }
}
