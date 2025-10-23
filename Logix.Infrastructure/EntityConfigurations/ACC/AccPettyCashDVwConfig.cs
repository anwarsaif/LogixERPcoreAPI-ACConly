using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccPettyCashDVwConfig : IEntityTypeConfiguration<AccPettyCashDVw>
    {
        public void Configure(EntityTypeBuilder<AccPettyCashDVw> entity)
        {
            entity.ToView("Acc_Petty_Cash_D_VW");
        }
    }
}
