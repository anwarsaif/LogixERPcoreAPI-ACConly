using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccPettyCashExpensesTypeConfig : IEntityTypeConfiguration<AccPettyCashExpensesType>
    {
        public void Configure(EntityTypeBuilder<AccPettyCashExpensesType> entity)
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

        }
    }
}
