using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccSettlementScheduleDVwConfig : IEntityTypeConfiguration<AccSettlementScheduleDVw>
    {
        public void Configure(EntityTypeBuilder<AccSettlementScheduleDVw> entity)
        {
            entity.ToView("Acc_Settlement_Schedule_D_VW");
        }
    }
}
