using Logix.Domain.ACC;
using Logix.Domain.Main;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccPeriodsConfig : IEntityTypeConfiguration<AccPeriods>
    {
        public void Configure(EntityTypeBuilder<AccPeriods> entity)
        {
            entity.Property(e => e.FlagDelete).HasDefaultValue(false);
            entity.Property(e => e.PeriodEndDateGregorian).IsFixedLength();
            entity.Property(e => e.PeriodEndDateHijri).IsFixedLength();
            entity.Property(e => e.PeriodStartDateGregorian).IsFixedLength();
            entity.Property(e => e.PeriodStartDateHijri).IsFixedLength();
        }
    }
}
