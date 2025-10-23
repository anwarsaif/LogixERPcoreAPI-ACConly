using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccBankChequeBookConfig : IEntityTypeConfiguration<AccBankChequeBook>
    {
        public void Configure(EntityTypeBuilder<AccBankChequeBook> entity)
        {
            entity.Property(e => e.Count).HasComputedColumnSql("(([TOChequeNo]-[FromChequeNo])+(1))", false);

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

        }
    }

}
