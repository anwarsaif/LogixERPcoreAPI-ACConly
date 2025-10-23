using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccBalanceSheetConfig : IEntityTypeConfiguration<AccBalanceSheet>
    {
        public void Configure(EntityTypeBuilder<AccBalanceSheet> entity)
        {
            entity.ToView("ACC_Balance_Sheet", "dbo");

            entity.Property(e => e.JDateGregorian).IsFixedLength();

            entity.Property(e => e.JDateHijri).IsFixedLength();
        }
    }

}
