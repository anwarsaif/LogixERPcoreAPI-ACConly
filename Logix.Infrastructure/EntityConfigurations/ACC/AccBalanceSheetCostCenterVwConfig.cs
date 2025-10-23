using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{

    public class AccBalanceSheetCostCenterVwConfig : IEntityTypeConfiguration<AccBalanceSheetCostCenterVw>
    {
        public void Configure(EntityTypeBuilder<AccBalanceSheetCostCenterVw> entity)
        {
            entity.ToView("ACC_Balance_Sheet_CostCenter_VW");
        }
    }

}