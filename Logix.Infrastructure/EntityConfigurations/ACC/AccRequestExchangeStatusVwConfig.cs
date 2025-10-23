using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccRequestExchangeStatusVwConfig : IEntityTypeConfiguration<AccRequestExchangeStatusVw>
    {
        public void Configure(EntityTypeBuilder<AccRequestExchangeStatusVw> entity)
        {
            entity.ToView("AccRequestExchangeStatusVw");
        }
    }

}
