using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccCashonhandListVWConfig : IEntityTypeConfiguration<AccCashOnHandListVw>
    {
        public void Configure(EntityTypeBuilder<AccCashOnHandListVw> entity)
        {
            entity.ToView("Acc_Cash_on_hand_List_VW");
        }
    }
}
