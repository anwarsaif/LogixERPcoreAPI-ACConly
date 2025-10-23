using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccPaymentTypeConfig : IEntityTypeConfiguration<AccPaymentType>
    {
        public void Configure(EntityTypeBuilder<AccPaymentType> entity)
        {
            entity.Property(e => e.PaymentTypeId).ValueGeneratedNever();

            entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");
        }
    }

}
