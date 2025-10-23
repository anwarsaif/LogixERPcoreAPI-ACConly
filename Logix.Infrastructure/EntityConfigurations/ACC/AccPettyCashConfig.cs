using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccPettyCashConfig : IEntityTypeConfiguration<AccPettyCash>
    {
        public void Configure(EntityTypeBuilder<AccPettyCash> entity)
        {
            entity.Property(e => e.BankId).HasDefaultValueSql("((0))");

            entity.Property(e => e.ChequDateHijri).IsFixedLength();

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

            entity.Property(e => e.PaymentTypeId).HasDefaultValueSql("((0))");
        }
    }
}
