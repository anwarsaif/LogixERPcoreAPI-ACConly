using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccAccountsLevelConfig : IEntityTypeConfiguration<AccAccountsLevel>
    {
        public void Configure(EntityTypeBuilder<AccAccountsLevel> entity)
        {
            entity.Property(e => e.LevelId).ValueGeneratedNever();

        }
    }
}
