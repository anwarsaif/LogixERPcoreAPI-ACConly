using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccBranchAccountTypeConfig : IEntityTypeConfiguration<AccBranchAccountType>
    {
        public void Configure(EntityTypeBuilder<AccBranchAccountType> entity)
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
        }
    }

}
