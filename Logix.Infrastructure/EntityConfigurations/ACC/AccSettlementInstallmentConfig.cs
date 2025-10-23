using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccSettlementInstallmentConfig : IEntityTypeConfiguration<AccSettlementInstallment>
    {
        public void Configure(EntityTypeBuilder<AccSettlementInstallment> entity)
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
        }
    }
}
