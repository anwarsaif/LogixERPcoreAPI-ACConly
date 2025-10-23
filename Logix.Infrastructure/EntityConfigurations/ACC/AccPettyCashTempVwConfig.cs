using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccPettyCashTempVwConfig : IEntityTypeConfiguration<AccPettyCashTempVw>
    {
        public void Configure(EntityTypeBuilder<AccPettyCashTempVw> entity)
        {
            entity.ToView("Acc_Petty_Cash_Temp_VW");
        }
    }
}
