using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccCashOnHandVwConfig : IEntityTypeConfiguration<AccCashOnHandVw>
    {
        public void Configure(EntityTypeBuilder<AccCashOnHandVw> entity)
        {
            entity.ToView("Acc_Cash_on_hand_VW");
        }
    }
}
