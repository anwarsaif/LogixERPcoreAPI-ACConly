using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public partial class AccCostCenterVwsConfig : IEntityTypeConfiguration<AccCostCenterVws>
    {
        public void Configure(EntityTypeBuilder<AccCostCenterVws> entity)
        {
            entity.ToView("ACC_CostCenter_VW");
        }
    }
}
