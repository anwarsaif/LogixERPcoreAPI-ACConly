using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccBalanceSheetPostOrNotConfig : IEntityTypeConfiguration<AccBalanceSheetPostOrNot>
    {
        public void Configure(EntityTypeBuilder<AccBalanceSheetPostOrNot> entity)
        {
            entity.ToView("ACC_Balance_Sheet_PostOrNot");
        }
    }
}
