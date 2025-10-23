using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccPettyCashVwConfig : IEntityTypeConfiguration<AccPettyCashVw>
    {
        public void Configure(EntityTypeBuilder<AccPettyCashVw> entity)
        {
            entity.ToView("Acc_Petty_Cash_VW");
        }
    }

}
