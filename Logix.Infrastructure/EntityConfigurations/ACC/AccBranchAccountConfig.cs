using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccBranchAccountConfig : IEntityTypeConfiguration<AccBranchAccount>
    {
        public void Configure(EntityTypeBuilder<AccBranchAccount> entity)
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
        }
    }
}
