using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccRequestBalanceStatusVwConfig : IEntityTypeConfiguration<AccRequestBalanceStatusVw>
    {
        public void Configure(EntityTypeBuilder<AccRequestBalanceStatusVw> entity)
        {
            entity.ToView("AccRequestBalanceStatusVw");
        }
    }

}
